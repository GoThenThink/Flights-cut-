using Common.Types;
using Flights.Contract.Base;
using Flights.Contract.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flights.Contract.Sections
{
    /// <summary>
    /// Section for work with <see cref="FlightDto"/>
    /// </summary>
    public interface IFlightSection : IBaseSection<long, FlightDto>
    {
        /// <summary>
        /// Binds a specific return flight to a specific direct flight
        /// </summary>
        /// <param name="id">Id of a direct flight.</param>
        /// <param name="dto">Return flight.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task PostReturnAsync(long id, FlightDto dto, CancellationToken ct = default);

        /// <summary>
        /// Delete the connection between direct and return flights.
        /// </summary>
        /// <param name="id">The id of a direct flight.</param>
        /// <param name="returnFlightId">The id of a return flight.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task DeleteReturnAsync(long id, long returnFlightId, CancellationToken ct = default);

        /// <summary>
        /// Finds specific return flight related to specific direct flight.
        /// </summary>
        /// <param name="id">Id of a direct flight which return one to be found.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>Requested return flight from database in DTO format.</returns>
        Task<FlightDto> GetReturnAsync(long id, long returnFlightId, CancellationToken ct = default);

        /// <summary>
        /// Finds all return flights related to specific direct flight.
        /// </summary>
        /// <param name="id">Direct flight which return ones to be found.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<FlightDto>> GetListReturnAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Finds all return flights of requested direct flights.
        /// </summary>
        /// <param name="flightIds">Direct flights which return ones to be found.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>List of return flights.</returns>
        Task<IReadOnlyList<FlightDto>> GetListReturnAsync(IReadOnlyList<long> flightIds, CancellationToken ct = default);

        /// <summary>
        /// Assign a user-specific priority to a specific return flight related to specified flight.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <param name="priority">Priority value.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task SetPriorityAsync(long directFlightId, long returnFlightId, int priority, CancellationToken ct = default);

        /// <summary>
        /// Assign column is_native_return_flight of return flight to true.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task SetNativeAsync(long directFlightId, long returnFlightId, CancellationToken ct = default);

        /// <summary>
        /// Set column is_available of return flight to user-specific value.
        /// </summary>
        /// <param name="directFlightId">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        /// <param name="newStatus">New status to be set.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task SetStatusReturnAsync(long directFlightId, long returnFlightId, bool newStatus, CancellationToken ct = default);

        /// <summary>
        /// Sets status of a flight.
        /// </summary>
        /// <param name="id">Id of a flight.</param>
        /// <param name="statusId">Statuses Id to be set.</param>
        /// <param name="ct">Cancellation Token.</param>
        Task SetStatusAsync(long id, short statusId, CancellationToken ct = default);

        /// <summary>
        /// Search specific flights with defined filters and pagination. 
        /// </summary>
        /// <param name="pagination">Pagination params.</param>
        /// <param name="filters">Filter params.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>List of requested flights.</returns>
        Task<IReadOnlyList<FlightDto>> SearchFlightsAsync(Pagination pagination, FlightSearchFiltersDto filters, CancellationToken ct = default);
    }
}
