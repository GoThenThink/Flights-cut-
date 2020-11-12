using Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.DAL.Abstractions
{
    /// <summary>
    /// Repository for work with return flights
    /// </summary>
    public interface IReturnFlightsRepo
    {
        /// <summary>
        /// Binds a specific return flight to a specific direct flight
        /// </summary>
        /// <param name="directFlightId">Id of a direct flight.</param>
        /// <param name="source">Information (including return flight id) of return flight
        /// to bind the direct one to.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if connection was not set, otherwise >"0".</returns>
        Task<int> AddAsync(long directFlightId, Flight source, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Delete the connection between direct and return flights.
        /// </summary>
        /// <param name="directFlightId">The id of a direct flight.</param>
        /// <param name="returnFlightId">The id of a return flight.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>IsNativeReturnFlight property to indicate whether native return flight or not was deleted.</returns>
        Task<bool> DeleteAsync(long directFlightId, long returnFlightId, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Finds specific return flight related to specific direct flight.
        /// </summary>
        /// <param name="directFlightId">Id of a direct flight which return one to be found.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <returns>Return flight.</returns>
        Task<Flight> GetAsync(long directFlightId, long returnFlightId);

        /// <summary>
        /// Finds all return flights related to specific direct flight.
        /// </summary>
        /// <param name="directFlightId">Direct flight which return ones to be found.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<Flight>> GetListAsync(long directFlightId);

        /// <summary>
        /// Finds all return flights of requested direct flights.
        /// </summary>
        /// <param name="directFlightIds">Direct flights which return ones to be found.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<Flight>> GetListAsync(IReadOnlyList<long> directFlightIds);

        /// <summary>
        /// Recalculates priority values of return flights related to specified flight.
        /// </summary>
        /// <param name="directFlightId">Flight id related to return flights which priority is recalculated.</param>
        /// <param name="priority">Priority value.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if recalculation was not performed, otherwise >"0".</returns>
        Task<int> RecalculatePriorityAsync(long directFlightId, int? priority, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Assign a user-specific priority to a specific return flight related to specified flight.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <param name="priority">Priority value.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetPriorityAsync(long directFlightId, long returnFlightId, int priority, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Sets column is_native_return_flight of return flight to false.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <returns>"0" if connection was not set, otherwise >"0".</returns>
        Task<int> UnsetNativeAsync(long directFlightId, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Sets column is_native_return_flight of return flight to true. 
        /// If return flight is not defined then native one becomes of first priority.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetNativeAsync(long directFlightId, System.Data.IDbConnection conn = null);

        /// <summary>
        /// Set column is_available of return flight to user-specific value.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        /// <param name="newStatus">New status to be set.</param>
        /// <param name="conn">Outter connection.</param>
        /// <returns>"0" if assignment was not performed, otherwise >"0".</returns>
        Task<int> SetStatusAsync(long? directFlightId, long returnFlightId, bool newStatus, System.Data.IDbConnection conn = null);
    }
}
