namespace Business.Models
{
    /// <summary>
    /// Model for Airlines.
    /// </summary>
    public class Airline
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
