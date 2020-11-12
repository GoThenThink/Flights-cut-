using RandomStartup.Extensions.AspNetCore.IdentityClient;
using RandomStartup.Extensions.AspNetCore.Middleware;
using Flights.Business;
using Flights.DAL;
using Flights.Extensions;
using Flights.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace Flights
{
    /// <summary/>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary/>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary/>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Point)));
                    options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(MultiPolygon)));
                })
                .AddNewtonsoftJson(options =>
                {
                    foreach (var converter in NetTopologySuite.IO.GeoJsonSerializer.Create(new GeometryFactory(new PrecisionModel(), 4326)).Converters)
                    {
                        options.SerializerSettings.Converters.Add(converter);
                    }
                })
                .AddErrorHandlerOptions(mapper => mapper.RegisterMapper());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = _configuration.GetSection("SecurityConfig").GetValue<string>("Authority");
                    options.RequireHttpsMetadata = _configuration.GetSection("SecurityConfig").GetValue<bool>("RequireHttpsMetadata");
                    options.ApiName = _configuration.GetSection("SecurityConfig").GetValue<string>("ApiResourceName");
                    options.ApiSecret = _configuration.GetSection("SecurityConfig").GetValue<string>("ApiResourceSecret");
                });

            services
                .AddDataAccessLayer(_configuration.GetConnectionString("DataBase"))
                .AddBusinessLayer()
                .AddAutoMapper()
                .AddAuthorizationHandler(
                    _configuration.GetSection("SecurityConfig"),
                    _configuration.GetSection("IdentityClient"))
                .AddSwagger(_configuration.GetSection("SwaggerConfig"));    
        }

        /// <summary/>
        public void Configure(IApplicationBuilder app)
        {
            app
                .UseRouting()
                .UseAuthSwagger(_configuration.GetSection("SwaggerConfig"))
                .UseAuthentication()
                .UseAuthorization()
                .UseErrorHandlerMiddleware()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
