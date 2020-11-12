using RandomStartup.Extensions.Common.Types;
using RandomStartup.Extensions.Common.Types.Exceptions;
using Business.Models;
using Flights.Business.Abstractions;
using Flights.Business.Services.HelperMethods;
using Flights.Business.Validating;
using Flights.Business.Validating.Validators;
using Flights.DAL.Abstractions;
using FluentPatchValidation.Base;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Flights.Business
{
    internal sealed class FlightsService : RecordBaseService<Flight, long, IFlightsRepo, IValidator<Flight>>, IFlightsService
    {
        private readonly IDbConnectionFactory _conn;
        private readonly IReturnFlightsRepo _returnFlightsRepository;

        /// <summary/>
        public FlightsService(IFlightsRepo flightsRepository,
            IReturnFlightsRepo returnFlightsRepository,
            IDbConnectionFactory conn,
            IValidator<Flight> validator,
            IPatchFieldProfile<Flight> patchFieldValidator)
            : base(flightsRepository, validator, patchFieldValidator)
        {
            _conn = conn;
            _returnFlightsRepository = returnFlightsRepository;
        }

        #region Direct flights functions

        /// <inheritdoc cref="IFlightsService.SetStatusAsync(long, short)"/>
        public async Task<int> SetStatusAsync(long id, short statusId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int operationResult;
                using (var conn = _conn.GetConnection())
                {
                    conn.Open();
                    operationResult = await Repository.SetStatusAsync(id, statusId, conn);
                    if (!operationResult.Equals(0))
                    {
                        if (statusId.Equals(1))
                            await _returnFlightsRepository.SetStatusAsync(null, id, true, conn);
                        else await _returnFlightsRepository.SetStatusAsync(null, id, false, conn);
                    }
                    else throw new InternalServerException("Failed to set status of direct flight.");
                }
                transactionScope.Complete();
                return 1;
            }
        }

        /// <inheritdoc cref="IFlightsService.GetListAsync(FlightSearchFilters, Pagination)"/>
        public async Task<IReadOnlyList<Flight>> GetListAsync(FlightSearchFilters filters, Pagination pagination)
        {
            // Replace white spaces or 0 with null values - it is needed for proper sql execution.
            await SearchFilterHelpers.FormatFilterValuesAsync(filters);

            return await Repository.GetListAsync(filters, pagination);
        }
        #endregion

        #region Return flights functions
        /// <inheritdoc cref="IFlightsService.AddReturnAsync(long, Flight)"/>
        public async Task<int> AddReturnAsync(long directFlightId, Flight source)
        {
            ValidationHelperMethods<Flight, IValidator<Flight>>.ValidateObject(source, Validator, FlightValidator.ReturnFlightRules);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int operationResult = 1;
                using (var conn = _conn.GetConnection())
                {
                    conn.Open();
                    if (source.IsNativeReturnFlight.Equals(true))
                    {
                        operationResult = await _returnFlightsRepository.UnsetNativeAsync(directFlightId, conn);
                        if (!operationResult.Equals(0))
                        {
                            source.Priority = 1;
                        }
                        else throw new InternalServerException("Failed to unset native of return flights during the operation of binding flights.");
                    }
                    operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, source.Priority, conn);

                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.AddAsync(directFlightId, source, conn);
                    else throw new InternalServerException("Failed to recalculate priorities of return flights during the operation of binding flights.");
                }
                if (!operationResult.Equals(0))
                {
                    transactionScope.Complete();
                    return 1;
                }
                else throw new InternalServerException("Failed to bind return flight to defined direct flight.");
            }
        }

        /// <inheritdoc cref="IFlightsService.DeleteReturnAsync(long, long)"/>
        public async Task<int> DeleteReturnAsync(long directFlightId, long returnFlightId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int operationResult = 1;
                using (var conn = _conn.GetConnection())
                {
                    conn.Open();
                    var isNative = await _returnFlightsRepository.DeleteAsync(directFlightId, returnFlightId, conn);
                    operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, null, conn);
                    if (!operationResult.Equals(0))
                    {
                        if (isNative.Equals(true))
                            operationResult = await _returnFlightsRepository.SetNativeAsync(directFlightId, conn);
                    }
                    else throw new InternalServerException("Failed to recalculate priorities of return flights after the operation of delete.");
                }
                if (!operationResult.Equals(0))
                {
                    transactionScope.Complete();
                    return 1;
                }
                else throw new InternalServerException("Failed to set native of return flight during the delete operation.");
            }
        }

        /// <inheritdoc cref="IFlightsService.GetReturnAsync(long, long)"/>
        public async Task<Flight> GetReturnAsync(long directFlightId, long returnFlightId)
        {
            var requestedObject = await _returnFlightsRepository.GetAsync(directFlightId, returnFlightId);
            if (requestedObject is null)
                throw new NotFoundException("Requested flight is not found.", null);

            return requestedObject;
        }

        /// <inheritdoc cref="IFlightsService.GetListReturnAsync(long)"/>
        public Task<IReadOnlyList<Flight>> GetListReturnAsync(long directFlightId)
        {
            return _returnFlightsRepository.GetListAsync(directFlightId);
        }

        /// <inheritdoc cref="IFlightsService.GetListReturnAsync(IReadOnlyList{long})"/>
        public Task<IReadOnlyList<Flight>> GetListReturnAsync(IReadOnlyList<long> directFlightIds)
        {
            return _returnFlightsRepository.GetListAsync(directFlightIds);
        }

        /// <inheritdoc cref="IFlightsService.SetPriorityAsync(long, long, int)"/>
        public async Task<int> SetPriorityAsync(long directFlightId, long returnFlightId, int priority)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int operationResult;
                using (var conn = _conn.GetConnection())
                {
                    conn.Open();
                    operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, priority, conn);
                    if (!operationResult.Equals(0))
                    {
                        if (priority.Equals(1))
                            operationResult = await _returnFlightsRepository.UnsetNativeAsync(directFlightId, conn);
                    }
                    else throw new InternalServerException("Failed to recalculate priorities of return flights during the SetPriority operation."); 

                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.SetPriorityAsync(directFlightId, returnFlightId, priority, conn);
                    else throw new InternalServerException("Failed to unset native from existing return flights during the SetPriority operation.");

                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, null, conn);
                    else throw new InternalServerException("Failed to set priority of return flight during the SetPriority operation.");

                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.SetNativeAsync(directFlightId, conn);
                    else throw new InternalServerException("Failed to recalculate priorities of return flights during the SetPriority operation.");
                }
                if (!operationResult.Equals(0))
                {
                    transactionScope.Complete();
                    return 1;
                }
                else throw new InternalServerException("Failed to set native of return flight during the SetPriority operation.");
            }
        }

        /// <inheritdoc cref="IFlightsService.SetNativeAsync(long, long)"/>
        public async Task<int> SetNativeAsync(long directFlightId, long returnFlightId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int operationResult;
                using (var conn = _conn.GetConnection())
                {
                    conn.Open();
                    operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, 1, conn);
                    if (!operationResult.Equals(0))
                        await _returnFlightsRepository.UnsetNativeAsync(directFlightId, conn);
                    else throw new InternalServerException("Failed to recalculate priorities of return flights during the SetNative operation.");
                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.SetPriorityAsync(directFlightId, returnFlightId, 1, conn);
                    else throw new InternalServerException("Failed to unset native from existing return flights during the SetNative operation.");

                    if (!operationResult.Equals(0))
                        operationResult = await _returnFlightsRepository.RecalculatePriorityAsync(directFlightId, null, conn);
                    else throw new InternalServerException("Failed to set priority of return flight during the SetNative operation.");
                }
                if (!operationResult.Equals(0))
                {
                    transactionScope.Complete();
                    return 1;
                }
                else throw new InternalServerException("Failed to recalculate priorities of return flights during the SetNative operation.");
            }
        }

        /// <inheritdoc cref="IFlightsService.SetStatusReturnAsync(long, long, bool)"/>
        public async Task<int> SetStatusReturnAsync(long directFlightId, long returnFlightId, bool newStatus)
        {
            var operationResult = await _returnFlightsRepository.SetStatusAsync(directFlightId, returnFlightId, newStatus);
            if (operationResult.Equals(0))
                throw new InternalServerException("Failed to set status of return flight.");
            return operationResult;
        }
        #endregion

    }
}
