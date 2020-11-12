using Business.Models;

namespace Flights.DAL.Abstractions
{
    /// <summary>
    /// Repository for work with "Airlines"
    /// </summary>
    public interface IAirlinesRepo : IBaseEntityRepository<int, Airline>
    {
    }
}
