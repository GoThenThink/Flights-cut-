using Business.Models;
using Flights.Business.Abstractions;
using Flights.DAL.Abstractions;
using FluentPatchValidation.Base;
using FluentValidation;

namespace Flights.Business
{
    internal sealed class AirlinesService : RecordBaseService<Airline, int, IAirlinesRepo, IValidator<Airline>>, IAirlinesService
    {
        /// <summary/>
        public AirlinesService(IAirlinesRepo airlinesRepository, IValidator<Airline> validator, IPatchFieldProfile<Airline> patchFieldValidator)
            :base(airlinesRepository, validator, patchFieldValidator)
        {
        }
    }
}
