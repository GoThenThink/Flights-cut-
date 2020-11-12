using Business.Models;
using FluentPatchValidation;

namespace Flights.Business.Validating.PatchFieldValidating.Profiles
{
    internal sealed class FlightPatchPropertyProfile : PatchFieldProfile<Flight>
    {
        public FlightPatchPropertyProfile()
        {
            AllowProperties(
                c => c.Name,
                c => c.From,
                c => c.To,
                c => c.AirlineId,
                c => c.FlightTypeId,
                c => c.TouristInfoEditLock,
                c => c.ReturnFlightSameAirlineRequired,
                c => c.AlternativeSearch,
                c => c.FlightDuration,
                c => c.AirplaneModelId
                );


        }
    }
}