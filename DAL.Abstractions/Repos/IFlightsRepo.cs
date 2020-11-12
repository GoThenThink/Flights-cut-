using RandomStartup.Extensions.Common.Types;
using Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.DAL.Abstractions
{
    /// <summary>
    /// Repository for work with "Flight"
    /// </summary>
    public interface IFlightsRepo : IBaseEntityRepository<long, Flight>
    {
        /// <summary>
        /// Returns information of all record that fits search criteria
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="pagination">Pagination.</param>
        /// <returns>List of requested items.</returns>
        Task<IReadOnlyList<Flight>> GetListAsync(
            FlightSearchFilters filters,
            Pagination pagination);

        /// <summary>
        /// Set status of a flight
        /// </summary>
        /// <param name="id">Id of a flight.</param>
        /// <param name="statusId">Statuses Id to be set.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if item was not updated, otherwise >"0".</returns>
        Task<int> SetStatusAsync(long id, short statusId, System.Data.IDbConnection conn);
    }
}
