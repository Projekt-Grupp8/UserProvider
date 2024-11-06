﻿namespace Infrastructure.Test.Services;

using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics;

public class UserService_Test
{
    private readonly UserService _userService;
    private readonly DataContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<ServiceBusHandler> _mockServiceBusHandler;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ITokenService> _mockTokenService;

    public UserService_Test()
    {
        // Mockar en InMemory-databas
        var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

        _context = new DataContext(options);

        // Mockar UserManager med alla dess beroenden.
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            new IUserValidator<ApplicationUser>[0],
            new IPasswordValidator<ApplicationUser>[0],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        // Mockar SignInManager med alla dess beroenden.
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<ApplicationUser>>().Object);

        var logger = new Mock<ILogger<UserService>>().Object;
        var sbhLogger = new Mock<ILogger<ServiceBusHandler>>().Object;

        _mockJwtService = new Mock<IJwtService>();
        var jwtService = new Mock<JwtService>().Object;

        _mockTokenService = new Mock<ITokenService>();

        _mockServiceBusHandler = new Mock<ServiceBusHandler>(
            null!,
            sbhLogger,
            null!,
            null!
            );

        // Mockar AddToRoleAsync för att simulera att roll satts korrekt.
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        _userService = new UserService(_mockUserManager.Object, _context, logger, _mockSignInManager.Object, _mockJwtService.Object, _mockServiceBusHandler.Object, _mockTokenService.Object);
    }

    public static SignUpUser CreateSignUpUserModel()
    {
        return new SignUpUser
        {
            UserName = "ted.pieplow@gmail.com",
            Email = "ted.pieplow@gmail.com",
            Password = "P@ssword123!",
            ConfirmPassword = "P@ssword123!",
            TermsConfirmed = false,
            IsVerified = false,
        };
    }

    public static SignInUser CreateSignInModel()
    {
        return new SignInUser
        {
            Email = "ted.pieplow@gmail.com",
            Password = "P@ssword123!",
            RememberMe = false,
        };
    }

    [Fact]
    public async Task CreateUserAsync_UserDoesNotExist_CreatesUserSuccessfully()
    {
        // Arrange
        var signUpUser = CreateSignUpUserModel();

        var user = new ApplicationUser
        {
            UserName = signUpUser.Email,
            Email = signUpUser.Email
        };

        // Mocka FindByEmailAsync för att returnera null, dvs användaren finns inte.
        _mockUserManager.Setup(x => x.FindByEmailAsync(signUpUser.Email))
                        .ReturnsAsync((ApplicationUser)null!);

        // Mockar CreateAsync för att simulera att användaren har skapats korrekt.
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(signUpUser);
        await _mockServiceBusHandler.Object.SendServiceBusMessageAsync(signUpUser.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.ContentResult!.ToString());
        Assert.Equal(ResponseFactory.Ok(user).StatusCode, result.StatusCode);
        Assert.Equal(ResponseFactory.Ok(user).Succeeded, result.Succeeded);

        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_DoesExist_DoesntCreateUser_ReturnsExists()
    {
        // Arrange
        var signUpUser = CreateSignUpUserModel();

        var existingUser = new ApplicationUser
        {
            UserName = signUpUser.Email,
            Email = signUpUser.Email,
        };

        // Mockar FindByEmailAsync för att simulera att användaren existerar.
        _mockUserManager.Setup(x => x.FindByEmailAsync(signUpUser.Email))
                        .ReturnsAsync(existingUser);

        // Act
        await _mockServiceBusHandler.Object.SendServiceBusMessageAsync(signUpUser.Email);
        var result = await _userService.CreateUserAsync(signUpUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Already exists.", ResponseFactory.Exists().Message);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SignInUser_WithValidCredentials_SignsInUser()
    {
        // Arrange 
        var signInUser = CreateSignInModel();

        var user = new ApplicationUser
        {
            UserName = signInUser.Email,
            Email = signInUser.Email
        };

        _mockUserManager.Setup(x => x.Users)
                   .Returns(new List<ApplicationUser> { user }.AsQueryable());

        // Mockar PasswordSignInAsync för att simulera success.
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(signInUser.Email, signInUser.Password, signInUser.RememberMe, false))
             .ReturnsAsync(SignInResult.Success);

        // Mockar GenerateToken för att simulera generering av token.
        string token = "simulated-jwt-token";
        _mockTokenService.Setup(x => x.GenerateTokenAsync(user.Email)).ReturnsAsync(token);

        // Act
        var result = await _userService.SignInUserAsync(signInUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResponseFactory.Ok(new { user.Email, Token = token }, "Succeeded").Message, result.Message);
        Assert.Equal(ResponseFactory.Ok(new { user.Email, Token = token }, "Succeeded").StatusCode, result.StatusCode);
    }

    [Fact]
    public async Task SignInUser_WithValidCredentials_GeneratesValidToken()
    {
        // Arrange 
        var signInUser = CreateSignInModel();
        var user = new ApplicationUser
        {
            UserName = signInUser.Email,
            Email = signInUser.Email,
        };

        // Mockar vi simulering av token-generering.
        string expectedToken = "simulated-jwt-token";
        _mockTokenService.Setup(x => x.GenerateTokenAsync(user.Email)).ReturnsAsync(expectedToken);

        // Act
        var token = await _mockTokenService.Object.GenerateTokenAsync(user.Email);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public async Task SignInUser_WithInvalidCredentials_ReturnsNotFound()
    {
        // Arrange 
        var newUser = CreateSignInModel();

        _mockUserManager.Setup(x => x.Users).Returns(new List<ApplicationUser>().AsQueryable());

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(newUser.Email, newUser.Password, newUser.RememberMe, false))
             .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _userService.SignInUserAsync(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Not found.", result.Message);  
        Assert.Equal(404, (int)result.StatusCode);
    }
}