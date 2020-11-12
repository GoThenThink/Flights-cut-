using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Flights.Extensions
{
    /// <summary>
    /// Extension for startup configuration related to swagger.
    /// </summary>
    internal static class SwaggerExtension
    {
        private const string Version = "v1";
        private const string SecurityDefinitionName = "oauth2";

        internal sealed class SwaggerConfiguration
        {
            public string AuthorizationUrl { get; set; }
            public string OAuth2RedirectUrl { get; set; }
            public string OAuthClientId { get; set; }
            public string OAuthClientSecret { get; set; }
            public string ScopeName { get; set; }
            public string TokenUrl { get; set; }
        }

        internal static IServiceCollection AddSwagger(
            this IServiceCollection services,
            IConfiguration config)
        {
            var configuration = new SwaggerConfiguration();
            config.Bind(configuration);

            if (!Uri.TryCreate(configuration.AuthorizationUrl, UriKind.Absolute, out var authorizationUri))
            {
                throw new UriFormatException($"Wrong format AuthorizationUrl: {configuration.AuthorizationUrl}");
            }

            if (!Uri.TryCreate(configuration.TokenUrl, UriKind.Absolute, out var tokenUri))
            {
                throw new UriFormatException($"Wrong format TokenUrl: {configuration.TokenUrl}");
            }

            return services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(Version, new OpenApiInfo { Title = "Flights API", Version = Version });
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                    c.AddSecurityDefinition(SecurityDefinitionName, new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = authorizationUri,
                                TokenUrl = tokenUri,
                                Scopes = new Dictionary<string, string>
                                {
                                    { configuration.ScopeName, "scope for flights" }
                                }
                            }
                        }
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = SecurityDefinitionName
                                }
                            },
                            new string[] {}
                        }
                    });
                    c.SchemaFilter<SchemaFilter>();
                });
        }

        internal static IApplicationBuilder UseAuthSwagger(
            this IApplicationBuilder builder,
            IConfiguration config)
        {
            var configuration = new SwaggerConfiguration();
            config.Bind(configuration);

            if (!Uri.TryCreate(configuration.OAuth2RedirectUrl, UriKind.Absolute, out var oAuth2RedirectUri))
            {
                throw new UriFormatException($"Wrong format OAuth2RedirectUrl: {configuration.OAuth2RedirectUrl}");
            }

            return builder
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flights API v1");
                    c.OAuthClientId(configuration.OAuthClientId);
                    c.OAuthClientSecret(configuration.OAuthClientSecret);
                    c.OAuth2RedirectUrl(oAuth2RedirectUri.ToString());
                    c.OAuthUsePkce();
                });
        }
    }
}