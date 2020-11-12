using RandomStartup.Extensions.Common.Types.Exceptions;
using Flights.Business.Abstractions;
using Flights.Business.Validating;
using Flights.DAL.Abstractions;
using FluentPatchValidation.Base;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.Business
{
    /// <summary>
    /// Base class for core crud operations
    /// </summary>
    /// <typeparam name="TBusinessModel">Business model type.</typeparam>
    /// <typeparam name="TBusinessModelId">Id type of business model.</typeparam>
    /// <typeparam name="TRepository">Repository type.</typeparam>
    /// <typeparam name="TValidator">Validator type.</typeparam>
    public abstract class RecordBaseService<TBusinessModel, TBusinessModelId, TRepository, TValidator>
        : IRecordBaseService<TBusinessModel, TBusinessModelId>
        where TRepository : IBaseEntityRepository<TBusinessModelId, TBusinessModel>
        where TValidator : IValidator<TBusinessModel>
    {
        /// <summary/>
        protected readonly TRepository Repository;
        /// <summary/>
        protected readonly TValidator Validator;
        /// <summary/>
        protected readonly IPatchFieldProfile<TBusinessModel> PatchFieldValidator;

        /// <summary/>
        protected RecordBaseService(TRepository repository,
            TValidator validator, IPatchFieldProfile<TBusinessModel> patchFieldValidator)
        {
            Repository = repository;
            Validator = validator;
            PatchFieldValidator = patchFieldValidator;
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.GetListAsync"/>
        public virtual Task<IReadOnlyList<TBusinessModel>> GetListAsync()
        {
            return Repository.GetListAsync();
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.GetAsync(TBusinessModelId)"/>
        public virtual async Task<TBusinessModel> GetAsync(TBusinessModelId id)
        {
            var requestedObject = await Repository.GetAsync(id);
            if (requestedObject is null)
                throw new NotFoundException($"{typeof(TBusinessModel).Name} object is not found.", null);
            return requestedObject;
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.AddAsync(TBusinessModel)"/>
        public virtual async Task<TBusinessModel> AddAsync(TBusinessModel model)
        {
            await Task.Run(() => ValidationHelperMethods<TBusinessModel, TValidator>.ValidateObject(model, Validator)); 

            return await Repository.AddAsync(model);
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.UpdateAsync(TBusinessModelId, TBusinessModel)"/>
        public virtual async Task<TBusinessModel> UpdateAsync(TBusinessModelId id, TBusinessModel model)
        {
            await Task.Run(() => ValidationHelperMethods<TBusinessModel, TValidator>.ValidateObject(model, Validator));

            var soughtForObject = await Repository.UpdateAsync(id, model);
            if (soughtForObject is null)
                throw new NotFoundException($"{typeof(TBusinessModel).Name} object is not found.", null);
            return soughtForObject;
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.PatchRecordAsync(TBusinessModelId, string, TBusinessModel)"/>
        public virtual async Task<TBusinessModel> PatchRecordAsync(TBusinessModelId id, string property, TBusinessModel model)
        {
            await Task.Run(() => {
                ValidationHelperMethods<TBusinessModel, TValidator>.CheckPropertyForPatching(PatchFieldValidator, property);
                ValidationHelperMethods<TBusinessModel, TValidator>.ValidatePropertyForPatching(model, Validator, property);
            });

            var soughtForObject = await Repository.PatchRecordAsync(id, property, model);
            if (soughtForObject is null)
                throw new NotFoundException($"{typeof(TBusinessModel).Name} object is not found.", null);

            return soughtForObject;
        }

        /// <inheritdoc cref="IRecordBaseService{TBusinessModel,TBusinessModelId}.DeleteAsync(TBusinessModelId)"/>
        public virtual Task<int> DeleteAsync(TBusinessModelId id)
        {
            return Repository.DeleteAsync(id);
        }
    }

}
