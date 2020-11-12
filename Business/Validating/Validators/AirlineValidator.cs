using FluentValidation;
using Bm = Business.Models.Airline;

namespace Flights.Business.Validating.Validators
{
    /// <summary>
    /// Validator for Airline model.
    /// </summary>
    public sealed class AirlineValidator : AbstractValidator<Bm>
    {
        /// <summary/>
        public AirlineValidator()
        {
            RuleFor(airline => airline.Name).NotEmpty().WithMessage("Name should not be null or empty.");
            RuleFor(airline => airline.Iata).NotEmpty().MaximumLength(4).WithMessage("Iata should not be null, empty or more than 4 symbols.");
        }
    }

}
