using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Scouty.Backend.Extensions;

public static class IdentityServiceExtensions
{
    // "this IServiceCollection services" permet d'ajouter une méthode à la variable builder.Services
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),

                ValidateIssuer = true,
                ValidIssuer = config["JwtSettings:Issuer"],

                ValidateAudience = true,
                ValidAudience = config["JwtSettings:Audience"],

                ValidateLifetime = true
            };
        });

        return services;
    }
}