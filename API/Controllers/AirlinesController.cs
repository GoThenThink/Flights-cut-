using AutoMapper;
using Flights.Business.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Bm = Business.Models.Airline;
using Dto = Flights.Contract.Dto.AirlineDto;

namespace Flights.Controllers
{
    /// <summary>
    /// Controller for managing airline
    /// </summary>
    [Route("api/airlines")]
    [ApiController]
    public sealed class AirlinesController : CrudControllerBase<Bm, int, IAirlinesService, Dto>
    {
        /// <summary/>
        public AirlinesController(IMapper mapper, IAirlinesService service)
            :base(service, mapper)
        {
        }
    }
}