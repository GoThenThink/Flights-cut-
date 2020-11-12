using Business.Models;
using FluentPatchValidation;

namespace Flights.Business.Validating.PatchFieldValidating.Profiles
{
    internal sealed class AirlinePatchPropertyProfile : PatchFieldProfile<Airline>
    {
        public AirlinePatchPropertyProfile()
        {
            AllowProperties(
                c => c.Name,
                c => c.Iata);

            AllowAllProperties();
        }
    }
}