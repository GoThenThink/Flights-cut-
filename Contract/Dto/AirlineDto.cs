namespace Flights.Contract.Dto
{
    /// <summary>
    /// Dto for Airlines model.
    /// </summary>
    public sealed class AirlineDto
    {
        /// <summary>
        /// Id of an airline.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of an airline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Iata of an airline.
        /// </summary>
        public string Iata { get; set; }
    }
}
