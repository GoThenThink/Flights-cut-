using Flights.Contract.Client.Base;
using Flights.Contract.Dto;
using Flights.Contract.Sections;
using System.Net.Http;

namespace Flights.Contract.Client.Sections
{
    internal sealed class AirlineSection : BaseSection<int, AirlineDto>, IAirlineSection
    {
        public AirlineSection(HttpClient httpClient)
            : base(httpClient, "api/airlines")
        {
        }
    }
}
