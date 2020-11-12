namespace Flights.DAL.Entities
{
    /// <summary>
    /// Entity for Flight.
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// Id of a flight.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Name of a flight.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of a station from Taxonomy service.
        /// Defines the place which tourist depart from.
        /// </summary>
        public long? From { get; set; }

        /// <summary>
        /// Id of a station from Taxonomy service.
        /// Defines the place which tourist arrive to.
        /// </summary>
        public long? To { get; set; }

        /// <summary>
        /// Id of an airline.
        /// </summary>
        public int? AirlineId { get; set; }

        /// <summary>
        /// Id of a flight type.
        /// </summary>
        public short? FlightTypeId { get; set; }

        /// <summary>
        /// Days before tour start editing of tourist info becomes locked.
        /// </summary>
        public int? TouristInfoEditLock { get; set; }

        /// <summary>
        /// Marks whether tourist must have return flight with the same airline or not. 
        /// </summary>
        public bool? ReturnFlightSameAirlineRequired { get; set; }

        /// <summary>
        /// Allows to use this flight for alternative search.
        /// </summary>
        public bool? AlternativeSearch { get; set; }

        /// <summary>
        /// Flight duration in days.
        /// </summary>
        public int? FlightDuration { get; set; }

        /// <summary>
        /// Id of an airplane model.
        /// </summary>
        public int? AirplaneModelId { get; set; }

        /// <summary>
        /// Flight status (Active, Archived, Disabled).
        /// </summary>
        public short? StatusId { get; set; }
    }
}
