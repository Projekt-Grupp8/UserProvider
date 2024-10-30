namespace Infrastructure.Test.Services;

using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

public class UserService_Test
{
    private readonly UserService _userService;
    private readonly DataContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<JwtService> _jwtServiceMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<ServiceBusHandler> _serviceBusHandlerMock;

    public UserService_Test()
    {
        // In-memory databas.
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureCreated();

        // Mocka IUserStore<ApplicationUser>
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null!,
            null!,
            null!,
            null!
        );

        _jwtServiceMock = new Mock<JwtService>();
        _serviceBusHandlerMock = new Mock<ServiceBusHandler>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _userService = new UserService(
            _userManagerMock.Object,
            _context,
            _loggerMock.Object,
            _signInManagerMock.Object,
            _jwtServiceMock.Object,
            _serviceBusHandlerMock.Object
        );
    }

    public static SignUpUser CreateUser()
    {
        return new SignUpUser
        {
            UserName = "TestUser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            TermsConfirmed = true
        };
    }

    /*
       ***** TESTFALLSBESKRIVNING *****

        """Lyckad registrering"""

        Givet att användaren anger ett giltigt användarnamn och lösenord.
        När registreringen genomförs.
        Då ska en ny användare skapas.


        """Registrering av redan existerande användare"""

        Givet att användaren försöker registrera ett användarnamn som redan finns i databasen.
        När registreringen genomförs.
        Då ska ett felmeddelande returneras.
    */

    [Fact]
    public async Task CreateUserAsync_UserDoesNotExist_CreatesUserSuccessfully()
    {
        // Arrange
        var signUpUser = CreateUser();

        // Mockar för att säkerställa att ingen användare med den e-posten finns
        _userManagerMock.Setup(m => m.FindByEmailAsync(signUpUser.Email))
            .ReturnsAsync((ApplicationUser)null);

        // Mocka skapandet av användaren, simulerar dessutom sparandet då Identity inte kan spara direkt själv i inmemory. 
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), signUpUser.Password))
                        .ReturnsAsync((ApplicationUser user, string password) =>
                        {
                            _context.Users.Add(user);
                            _context.SaveChanges();
                            return IdentityResult.Success;
                        });

        // Act
        var result = await _userService.CreateUserAsync(signUpUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.OK, result.StatusCode);

        // Verifiera att CreateAsync anropades en gång
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), signUpUser.Password), Times.Once());

        // Kontrollera att användaren har sparats i databasen
        var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == signUpUser.Email);
        Assert.NotNull(createdUser);
    }

    [Fact]
    public async Task CreateUserAsync_UserAlreadyExists_ReturnsExistsResponse()
    {
        // Arrange
        var signUpUser = CreateUser();

        // Mockar att användaren redan finns
        var existingUser = new ApplicationUser { Email = signUpUser.Email };
        _userManagerMock.Setup(m => m.FindByEmailAsync(signUpUser.Email))
            .ReturnsAsync(existingUser);

        // Var tvungen att spara då UserManager inte verkar klara av att göra det.
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.CreateUserAsync(signUpUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.EXISTS, result.StatusCode);
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_UserCreationFails_ReturnsInternalError()
    {
        // Arrange
        var signUpUser = new SignUpUser
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Mocka att användaren inte redan finns
        _userManagerMock.Setup(m => m.FindByEmailAsync(signUpUser.Email))
            .ReturnsAsync((ApplicationUser)null);

        var user = new ApplicationUser { Email = signUpUser.Email };
        _userManagerMock.Setup(m => m.CreateAsync(user, signUpUser.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await _userService.CreateUserAsync(signUpUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.INTERNAL, result.StatusCode);
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), signUpUser.Password), Times.Once);
    }
}