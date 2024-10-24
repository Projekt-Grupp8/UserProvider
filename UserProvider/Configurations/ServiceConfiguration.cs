﻿using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserProvider.Configurations;

public static class ServiceConfiguration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging();
        services.AddAuthorization();
        services.ValidateJWT(configuration);

        var connectionString = configuration.GetConnectionString("AzureSqlServer")
            ?? throw new ArgumentNullException(nameof(configuration), "Connection string for 'SqlServer' not found.");

        services.AddDbContext<DataContext>(x => x.UseSqlServer(connectionString));
        services.AddDefaultIdentity<ApplicationUser>(x =>
        {
            x.User.RequireUniqueEmail = true;
            x.SignIn.RequireConfirmedAccount = false;
            x.Password.RequiredLength = 8;
            x.Lockout.MaxFailedAccessAttempts = 3;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DataContext>();


        services.AddScoped<UserService>();
        services.AddScoped<JwtService>();
        services.AddSingleton<EmailService>();
    }
}
