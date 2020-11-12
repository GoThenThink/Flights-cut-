using RandomStartup.Extensions.Common.Types;
using Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.Business.Abstractions
{
    /// <summary>
    /// Service for work with direct flights and return flights
    /// </summary>
    public interface IFlightsService : IRecordBaseService<Flight, long>
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
        /// <returns>"0" if item was not updated, otherwise >"0".</returns>
        Task<int> SetStatusAsync(long id, short statusId);

        #region Return flights functions
        /// <summary>
        /// Binds a specific return flight to a specific direct flight
        /// </summary>
        /// <param name="directFlightId">Id of a direct flight.</param>
        /// <param name="source">Information (including return flight id) of return flight
        /// to bind the direct one to.</param>
        /// <returns>"0" if connection was not set, otherwise >"0".</returns>
        Task<int> AddReturnAsync(long directFlightId, Flight source);

        /// <summary>
        /// Delete the connection between direct and return flights.
        /// </summary>
        /// <param name="directFlightId">The id of a direct flight.</param>
        /// <param name="returnFlightId">The id of a return flight.</param>
        /// <returns>"0" if connection was not deleted, otherwise >"0".</returns>
        Task<int> DeleteReturnAsync(long directFlightId, long returnFlightId);

        /// <summary>
        /// Finds specific return flight related to specific direct flight.
        /// </summary>
        /// <param name="directFlightId">Id of a direct flight which return one to be found.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <returns>Return flight.</returns>
        Task<Flight> GetReturnAsync(long directFlightId, long returnFlightId);

        /// <summary>
        /// Finds all return flights related to specific direct flight.
        /// </summary>
        /// <param name="directFlightId">Direct flight which return ones to be found.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<Flight>> GetListReturnAsync(long directFlightId);

        /// <summary>
        /// Finds all return flights of requested direct flights.
        /// </summary>
        /// <param name="directFlightIds">Direct flights which return ones to be found.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<Flight>> GetListReturnAsync(IReadOnlyList<long> directFlightIds);

        /// <summary>
        /// Assign a user-specific priority to a specific return flight related to specified flight.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <param name="priority">Priority value.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetPriorityAsync(long directFlightId, long returnFlightId, int priority);

        /// <summary>
        /// Assign column is_native_return_flight of return flight to true.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetNativeAsync(long directFlightId, long returnFlightId);

        /// <summary>
        /// Set column is_available of return flight to user-specific value.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        /// <param name="newStatus">New status to be set.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetStatusReturnAsync(long directFlightId, long returnFlightId, bool newStatus);
        #endregion
    }
}
