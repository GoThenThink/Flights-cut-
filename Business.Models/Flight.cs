using System;
using System.Collections.Generic;

namespace Business.Models
{
    /// <summary>
    /// Model for Flight.
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
        /// Nearest flight date: departure.
        /// </summary>
        public DateTimeOffset? NearestDepartureDate { get; set; }

        /// <summary>
        /// Nearest flight date: arrival.
        /// </summary>
        public DateTimeOffset? NearestArrivalDate { get; set; }

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

        /// <summary>
        /// Array of ids of related return flights.
        /// </summary>
        public List<long> ReturnFlightIds { get; set; } 

        /// <summary>
        /// If this flight is return one, this field shows whether this return flight is native or not.
        /// </summary>
        public bool? IsReturnFlight { get; set; }

        /// <summary>
        /// If this flight is return one, this field shows related direct flight.
        /// </summary>
        public long? FlightToId { get; set; } 

        /// <summary>
        /// If this flight is return one, this field shows priority of this return flight.
        /// </summary>
        public short? Priority { get; set; } 

        /// <summary>
        /// If this flight is return one, this field shows availability of this return flight.
        /// </summary>
        public bool? IsAvailable { get; set; } 

        /// <summary>
        /// If this flight is return one, this field shows whether this return flight is native or not.
        /// </summary>
        public bool? IsNativeReturnFlight { get; set; } 
    }
}
