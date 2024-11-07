namespace Infrastructure.Test.IntegrationTests.Services;

public class UserService_IntegrationTest
{
    //[Fact]
    //public async Task IsUserVerifiedAsync_WithRealDatabase_Returns_True()
    //{
    //    // Arrange
    //    var options = new DbContextOptionsBuilder<DataContext>()
    //                    .UseInMemoryDatabase(databaseName: "TestDatabase")
    //                    .Options;

    //    var context = new DataContext(options);
    //    var jwtService = new JwtService();
    //    var serviceBusHandler = new ServiceBusHandler();
    //    var tokenService = new TokenService(null, null, null);

    //    var userManager = new UserManager<ApplicationUser>(
    //        new UserStore<ApplicationUser>(context),
    //        null, null, null, null, null, null, null, null);

    //    var signInManager = new SignInManager<ApplicationUser>(
    //        null, null, null, null, null, null, null
    //        );

    //    //var userService = new UserService(userManager, context, logger, signInManager, jwtService, serviceBusHandler, tokenService);  // Använd riktig UserService

    //    var user = CreateSignInModel();

    //    var newUser = new ApplicationUser
    //    {
    //        UserName = user.Email,
    //        Email = user.Email,
    //    };


    //    context.Users.Add(newUser);
    //    await context.SaveChangesAsync();

    //    // Act
    //    //var isVerified = await userService.IsUserVerifiedAsync(user.Email);

    //    // Assert
    //    Assert.True(isVerified);  // Kontrollera att användaren är verifierad
    //}
}
