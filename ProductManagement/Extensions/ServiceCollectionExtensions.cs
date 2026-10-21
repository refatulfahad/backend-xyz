using Microsoft.OpenApi;

namespace ProductManagement.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Authenticate using Keycloak.",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "profile", "profile" }
                            }
                        }
                    }
                });

                options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Keycloak", document)] = []
                });

            });
            return services;
        }
    }
}
