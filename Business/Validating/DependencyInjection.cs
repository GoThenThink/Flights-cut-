using Business.Models;
using Flights.Business.Validating.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Flights.Business.Validating
{
    internal static class DependencyInjection
    {
        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            return services
                .AddSingleton<IValidator<Airline>, AirlineValidator>()
                .AddSingleton<IValidator<Flight>, FlightValidator>();
        }
    }
}
