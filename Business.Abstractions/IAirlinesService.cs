using Business.Models;

namespace Flights.Business.Abstractions
{
    /// <summary>
    /// Service for work with airlines
    /// </summary>
    public interface IAirlinesService : IRecordBaseService<Airline, int>
    {
    }
}
