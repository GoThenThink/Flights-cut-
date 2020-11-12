using Business.Models;
using Flights.Business.Validating.PatchFieldValidating.Profiles;
using FluentPatchValidation.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Flights.Business.Validating.PatchFieldValidating
{
    internal static class DependencyInjection
    {
        public static IServiceCollection RegisterPatchPropertiesValidators(this IServiceCollection services)
        {
            return services
                .AddSingleton<IPatchFieldProfile<Airline>, AirlinePatchPropertyProfile>()
                .AddSingleton<IPatchFieldProfile<Flight>, FlightPatchPropertyProfile>();
        }
    }
}
