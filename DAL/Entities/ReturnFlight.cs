namespace Flights.DAL.Entities
{
    /// <summary>
    /// Entity for return flight.
    /// </summary>
    public class ReturnFlight
    {
        /// <summary>
        /// Id of a flight that is considered as direct flight.
        /// </summary>
        public long? FlightToId { get; set; }

        /// <summary>
        /// Id of a flight that is considered as return flight.
        /// </summary>
        public long? FlightFromId { get; set; }

        /// <summary>
        /// Priority of this return flight.
        /// </summary>
        public short? Priority { get; set; } 

        /// <summary>
        /// Availability of this return flight.
        /// </summary>
        public bool? IsAvailable { get; set; }

        /// <summary>
        /// This field shows whether this return flight is native or not.
        /// </summary>
        public bool? IsNativeReturnFlight { get; set; }

    }
}
