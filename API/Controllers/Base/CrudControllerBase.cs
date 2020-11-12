using AutoMapper;
using Flights.Business.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.Controllers
{
    /// <summary>
    /// Controller base class for core crud operations
    /// </summary>
    [ApiController]
    public abstract class CrudControllerBase<TBusinessModel, TBusinessModelId, TService, TDto> : ControllerBase
        where TService : IRecordBaseService<TBusinessModel, TBusinessModelId>
    {
        /// <summary/>
        protected readonly TService Service;
        /// <summary/>
        protected readonly IMapper Mapper;

        /// <summary/>
        protected CrudControllerBase(
            TService service,
            IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        /// <summary>
        /// Returns record information by specified id
        /// </summary>
        /// <param name="id">An id of sought record.</param>
        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TDto>> GetAsync(TBusinessModelId id)
        {
            return Ok(Mapper.Map<TDto>(await Service.GetAsync(id)));
        }

        /// <summary>
        /// Shows all records from the database table
        /// </summary>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<IReadOnlyList<TDto>>> GetListAsync()
        {
            var objects = await Service.GetListAsync();
            return Ok(Mapper.Map<List<TDto>>(objects));
        }

        /// <summary>
        /// Adds a new item to DB
        /// </summary>
        /// <param name="model">New item to be added.</param>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Post))]
        public virtual async Task<ActionResult<TDto>> PostAsync([FromBody] TDto model)
        {            
            var bm = await Service.AddAsync(Mapper.Map<TBusinessModel>(model));
            return Ok(Mapper.Map<TDto>(bm));
        }

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in a class.</param>
        /// <param name="model">New value.</param>
        [HttpPatch("{id}/{property}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public virtual async Task<ActionResult<TDto>> PatchAsync(TBusinessModelId id, string property, [FromBody] TDto model)
        {
            var bm = Mapper.Map<TBusinessModel>(model);
            bm = await Service.PatchRecordAsync(id, property, bm);
            return Ok(Mapper.Map<TDto>(bm));
        }

        /// <summary>
        /// Updates content of an existed record
        /// </summary>
        /// <param name="id">The id of an item to be updated.</param>
        /// <param name="model">The information to update content of an existed record.</param>
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Put))]
        public virtual async Task<TDto> PutAsync(TBusinessModelId id, [FromBody] TDto model)
        {
            var bm = await Service.UpdateAsync(id, Mapper.Map<TBusinessModel>(model));
            return Mapper.Map<TDto>(bm);
        }

        /// <summary>
        /// Delete a specific item
        /// </summary>
        /// <param name="id">The id of an item to be deleted.</param>
        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Delete))]
        public virtual async Task<IActionResult> DeleteAsync(TBusinessModelId id)
        {
            return Ok(await Service.DeleteAsync(id));
        }
    }
}
