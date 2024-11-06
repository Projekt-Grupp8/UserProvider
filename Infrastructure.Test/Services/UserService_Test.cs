namespace Infrastructure.Test.Services;

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
        //var jwtService = new Mock<JwtService>().Object;

        _mockServiceBusHandler = new Mock<ServiceBusHandler>(
            null!,
            sbhLogger,
            null!,
            null!
            );

        // Mockar AddToRoleAsync för att simulera att roll satts korrekt.
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        _userService = new UserService(_mockUserManager.Object, _context, logger, _mockSignInManager.Object, _mockJwtService.Object, _mockServiceBusHandler.Object);
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
                .ReturnsAsync(IdentityResult.Success)
                .Callback(() => Debug.WriteLine("CreateAsync called"));

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

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(signInUser.Email, signInUser.Password, signInUser.RememberMe, false))
             .ReturnsAsync(SignInResult.Success);


        // Vi måste mocka JWT service också, annars går det inte!!
        string token = "simulated-jwt-token";
        _mockJwtService.Setup(x => x.GetToken(signInUser.Email, "User")).Returns(token);
        var tokenResult = await _userService.GenerateTokenAsync(user.Email);

        // Act
        var result = await _userService.SignInUserAsync(signInUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResponseFactory.Ok(user).Message, result.Message);
        Assert.Equal(ResponseFactory.Ok(user).StatusCode, result.StatusCode);
    }

    [Fact]
    public async Task SignInUser_WithValidCredentials_GeneratesValidToken()
    {
        // Arrange 

        // Act

        // Assert
    }

    [Fact]
    public async Task SignInUser_WithInvalidCredentials_ReturnsNotFound()
    {
        // Arrange 

        // Act

        // Assert
    }
}