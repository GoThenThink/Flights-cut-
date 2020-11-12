using System;
using System.Collections.Generic;

namespace Business.Models
{
    public sealed class FlightSearchFilters
    {
        /// <summary>
        /// List of ids of status filters.
        /// </summary>
        public List<long> Statuses { get; set; } = new List<long>();

        /// <summary>
        /// List of ids of airline filters.
        /// </summary>
        public List<long> Airlines { get; set; } = new List<long>();

        /// <summary>
        /// Input string for search by name.
        /// </summary>
        public string SearchByName { get; set; }

        /// <summary>
        /// Id of a departure airport.
        /// </summary>
        public List<long> From { get; set; } = new List<long>();

        /// <summary>
        /// Id of an arrival airport.
        /// </summary>
        public List<long> To { get; set; } = new List<long>();
    }
}
