using AutoMapper;
using RandomStartup.Extensions.Common.Types;
using Business.Models;
using Flights.Business.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bm = Business.Models.Flight;
using Dto = Flights.Contract.Dto.FlightDto;

namespace Flights.Controllers
{
    /// <summary>
    /// Controller for managing flights
    /// </summary>
    [Route("api/flights")]
    [ApiController]
    public sealed class FlightsController : CrudControllerBase<Bm, long, IFlightsService, Dto>
    {
        /// <summary/>
        public FlightsController(IMapper mapper, IFlightsService service)
            : base(service, mapper)
        {
        }

        /// <summary>
        /// Returns information of all record that fits search criteria
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="pagination">Pagination.</param>
        [HttpPost("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<Dto>>> SearchAsync(
            [FromBody] FlightSearchFilters filters,
            [FromQuery] Pagination pagination)
        {
            return Ok(Mapper.Map<IReadOnlyList<Dto>>(await Service.GetListAsync(filters, pagination)));
        }

        /// <summary>
        /// Sets status of a flight.
        /// </summary>
        /// <param name="id">Id of a flight.</param>
        /// <param name="statusId">Statuses Id to be set.</param>
        [HttpPatch("{id}/status")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> SetStatusAsync(long id, [FromBody]short statusId)
        {
            return Ok(await Service.SetStatusAsync(id, statusId));
        }

        #region Return flights functions
        /// <summary>
        /// Binds a specific return flight to a specific direct flight
        /// </summary>
        /// <param name="id">Id of a direct flight.</param>
        /// <param name="dto">Return flight.</param>
        [HttpPost("{id}/returns")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> PostReturnAsync(long id,
            [FromBody] Dto dto)
        {
            return Ok(await Service.AddReturnAsync(id, Mapper.Map<Bm>(dto)));
        }

        /// <summary>
        /// Delete the connection between direct and return flights.
        /// </summary>
        /// <param name="id">The id of a direct flight.</param>
        /// <param name="returnFlightId">The id of a return flight.</param>
        /// <returns>"0" if connection was not deleted, otherwise >"0".</returns>
        [HttpDelete("{id}/returns/{returnFlightId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteReturnAsync(long id, long returnFlightId)
        {
            return Ok(await Service.DeleteReturnAsync(id, returnFlightId));
        }

        /// <summary>
        /// Finds specific return flight related to specific direct flight.
        /// </summary>
        /// <param name="id">Id of a direct flight which return one to be found.</param>
        /// <param name="returnFlightId">Id of a sought return flight.</param>
        /// <returns>Requested return flight from database in DTO format.</returns>
        [HttpGet("{id}/returns/{returnFlightId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Dto>> GetReturnAsync(long id, long returnFlightId)
        {
            return Ok(Mapper.Map<Dto>(await Service.GetReturnAsync(id, returnFlightId)));
        }

        /// <summary>
        /// Finds all return flights related to specific direct flight.
        /// </summary>
        /// <param name="id">Direct flight which return ones to be found.</param>
        /// <returns>List of return flights in DTO format.</returns>
        [HttpGet("{id}/returns")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IReadOnlyList<Dto>>> GetListReturnAsync(long id)
        {
            var returnFlights = await Service.GetListReturnAsync(id);
            return Ok(Mapper.Map<List<Dto>>(returnFlights));
        }

        /// <summary>
        /// Finds all return flights of requested direct flights.
        /// </summary>
        /// <param name="flightIds">Direct flights which return ones to be found.</param>
        /// <returns>List of return flights.</returns>
        [HttpGet("returns")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IReadOnlyList<Dto>>> GetListReturnAsync([FromQuery]IReadOnlyList<long> flightIds)
        {
            var returnFlights = await Service.GetListReturnAsync(flightIds);
            return Ok(Mapper.Map<List<Dto>>(returnFlights));
        }

        /// <summary>
        /// Sets status of a return flight.
        /// </summary>
        /// <param name="id">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        /// <param name="status">New status to be set.</param>
        [HttpPatch("{id}/returns/{returnFlightId}/status")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> SetStatusReturnAsync(long id, long returnFlightId, [FromBody]bool status)
        {
            return Ok(await Service.SetStatusReturnAsync(id, returnFlightId, status));
        }

        /// <summary>
        /// Sets nativeness of a return flight.
        /// </summary>
        /// <param name="id">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        [HttpPatch("{id}/returns/set-native")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> SetNativeAsync(long id, [FromBody]long returnFlightId)
        {
            return Ok(await Service.SetNativeAsync(id, returnFlightId));
        }

        /// <summary>
        /// Sets priority of a return flight.
        /// </summary>
        /// <param name="id">Direct flight id.</param>
        /// <param name="returnFlightId">Return flight id.</param>
        /// <param name="priority">Priority value.</param>
        [HttpPatch("{id}/returns/{returnFlightId}/priority")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> SetPriorityAsync(long id, long returnFlightId, [FromBody]int priority)
        {
            return Ok(await Service.SetPriorityAsync(id, returnFlightId, priority));
        }
        #endregion

    }
}