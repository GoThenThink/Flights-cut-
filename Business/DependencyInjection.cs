using Flights.Business.Abstractions;
using Flights.Business.Validating;
using Flights.Business.Validating.PatchFieldValidating;
using Microsoft.Extensions.DependencyInjection;

namespace Flights.Business
{
    /// <summary>
    /// Class for setting up the Business Layer
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add all Business Layer components
        /// </summary>
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            return services
                .RegisterServices()
                .RegisterMapperProfiles()
                .RegisterValidators()
                .RegisterPatchPropertiesValidators();
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IAirlinesService, AirlinesService>()
                .AddScoped<IFlightsService, FlightsService>();          
        }
        private static IServiceCollection RegisterMapperProfiles(this IServiceCollection services)
        {
            return services;
        }
    }
}
