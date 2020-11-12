using Flights.Contract.Client.Sections;
using Flights.Contract.Sections;
using System.Net.Http;

namespace Flights.Contract.Client
{
    /// <summary>
    /// Client for work with Flights Service
    /// </summary>
    public sealed class FlightServiceClient : IFlightsServiceClient
    {
        private readonly HttpClient _httpClient;

        /// <summary/>
        public FlightServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            AirlineSection = new AirlineSection(httpClient);
            FlightSection = new FlightSection(httpClient);

        }

        /// <inheritdoc cref="IFlightsServiceClient.AirlineSection"/>
        public IAirlineSection AirlineSection { get; }

        /// <inheritdoc cref="IFlightsServiceClient.FlightSection"/>
        public IFlightSection FlightSection { get; }

    }
}
