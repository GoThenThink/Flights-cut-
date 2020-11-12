namespace Business.Models
{
    /// <summary>
    /// Represents search filter.
    /// </summary>
    public sealed class Filter
    {
        /// <summary>
        /// Id of a filter.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of a related filter group.
        /// </summary>
        public string FilterGroupName { get; set; }

        /// <summary>
        /// Name of a filter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of elements fit this filter.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Defines whether this filter should be selected by default.
        /// </summary>
        public bool? IsSelected { get; set; }
    }
}
