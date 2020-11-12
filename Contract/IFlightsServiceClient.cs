using Flights.Contract.Sections;

namespace Flights.Contract
{
    public interface IFlightsServiceClient
    {

        /// <inheritdoc cref="IAirlineSection"/>
        IAirlineSection AirlineSection { get; }


        /// <inheritdoc cref="IFlightSection"/>
        IFlightSection FlightSection { get; }


    }
}
