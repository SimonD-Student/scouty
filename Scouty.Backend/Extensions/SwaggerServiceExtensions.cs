using Microsoft.OpenApi.Models;

namespace Scouty.Backend.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Scouty API", Version = "v1" });

            // --- CHANGEMENT ICI ---
            // On utilise le type 'Http' qui gère le préfixe "Bearer" automatiquement
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Collez simplement votre token ci-dessous (sans écrire 'Bearer')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http, // <--- C'est ça qui change tout
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }
}