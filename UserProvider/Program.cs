using UserProvider.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);
builder.Logging.AddConsole();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
