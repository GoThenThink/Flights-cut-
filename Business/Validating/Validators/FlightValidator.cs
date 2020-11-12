using FluentValidation;
using Bm = Business.Models.Flight;

namespace Flights.Business.Validating.Validators
{
    /// <summary>
    /// Validator for Flight model.
    /// </summary>
    public sealed class FlightValidator : AbstractValidator<Bm>
    {
        public const string ReturnFlightRules = "ReturnFlightRules";

        /// <summary/>
        public FlightValidator()
        {
            RuleFor(flight => flight.Name).NotEmpty().WithMessage("Name should not be null or empty.");
            RuleFor(flight => flight.From).GreaterThan(0).WithMessage("From property should be greater than 0.");
            RuleFor(flight => flight.To).GreaterThan(0).WithMessage("To property should be greater than 0.");
            RuleFor(flight => flight.AirlineId).GreaterThan(0).WithMessage("Airline Id property should be greater than 0.");
            RuleFor(flight => flight.FlightTypeId).Must(flightTypeId => flightTypeId > 0).WithMessage("Flight type Id property should be greater than 0.");
            RuleFor(flight => flight.FlightDuration).GreaterThan(0).WithMessage("Flight duration property should be greater than 0.");
            RuleFor(flight => flight.AirplaneModelId).GreaterThan(0).WithMessage("Airplane model Id property should be greater than 0.");
            RuleFor(flight => flight.StatusId).Must(statusId => statusId < 4 && statusId > 0).WithMessage("Status Id property should be greater than 0 and less than 4.");
            RuleFor(flight => flight.ReturnFlightSameAirlineRequired).NotNull().WithMessage("ReturnFlightSameAirlineRequired property should not be null.");
            RuleFor(flight => flight.AlternativeSearch).NotNull().WithMessage("AlternativeSearch property should not be null.");

            RuleSet(ReturnFlightRules, () =>
            {
                RuleFor(flight => flight.Id).NotEmpty().WithMessage("Return flight Id should not be null or empty.");
                RuleFor(flight => flight.Priority).Must(priority => priority > 0).WithMessage("Priority property should be greater than 0.");
                RuleFor(flight => flight.IsAvailable).NotNull().WithMessage("IsAvailable property should not be null.");
                RuleFor(flight => flight.IsNativeReturnFlight).NotNull().WithMessage("IsNativeReturnFlight property should not be null.");
            });
        }
    }

}
