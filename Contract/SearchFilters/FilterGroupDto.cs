using System.Collections.Generic;

namespace Flights.Contract
{
    /// <summary>
    /// Represents a group of filters.
    /// </summary>
    public sealed class FilterGroupDto
    {
        /// <summary>
        /// Id of a filter group.
        /// </summary>
        public FilterTypesDto Id { get; set; } 

        /// <summary>
        /// Name of a filter group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total number of elements this filter group contains.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// List of filter objects that this group contains.
        /// </summary>
        public IReadOnlyList<FilterDto> Filters { get; set; }

        /// <summary>
        /// Enables multi-select.
        /// </summary>
        public bool IsMultiselect { get; set; }
    }
}
