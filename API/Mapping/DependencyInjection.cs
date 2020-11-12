using AutoMapper;
using Flights.Mapping.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Flights.Mapping
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            return services
                .AddAutoMapperProfiles()
                .AddSingleton(provider =>
                {
                    var configuration = new MapperConfiguration(cfg => { 
                        cfg.AddProfiles(provider.GetServices<Profile>()); 
                        cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                        cfg.AllowNullCollections = true;
                    }); 

                    configuration.AssertConfigurationIsValid();
                    return configuration.CreateMapper(provider.GetService);
                });
        }
        private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            return services

                .AddSingleton<Profile, AirlineDtoProfile>()
                .AddSingleton<Profile, FlightDtoProfile>();
        }
    }
}
