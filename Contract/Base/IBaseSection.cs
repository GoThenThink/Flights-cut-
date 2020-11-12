using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flights.Contract.Base
{
    /// <summary>
    /// Parent for flights sections.
    /// </summary>
    /// <typeparam name="TIdentifier">Identifier type.</typeparam>
    /// <typeparam name="TSource">Source type.</typeparam>
    public interface IBaseSection<in TIdentifier, TSource>
    {
        /// <summary>
        /// Return information about <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="id">Source's identifier</param>
        /// <param name="ct">Cancellation Token</param>
        Task<TSource> GetAsync(TIdentifier id, CancellationToken ct = default);

        /// <summary>
        /// Return information about all objects
        /// </summary>
        /// <param name="ct">Cancellation Token</param>
        Task<IReadOnlyList<TSource>> GetListAsync(CancellationToken ct = default);

        /// <summary>
        /// Send request to create new <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="source">Source information</param>
        /// <param name="ct">Cancellation Token</param>
        Task<TSource> InsertAsync(TSource source, CancellationToken ct = default);

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in database.</param>
        /// <param name="model">New value.</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Updated item.</returns>
        Task<TSource> PatchRecordAsync(TIdentifier id, string property, TSource model, CancellationToken ct = default);

        /// <summary>
        /// Send request to update <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="id">Source's identifier</param>
        /// <param name="source">Source information</param>
        /// <param name="ct">Cancellation Token</param>
        Task<TSource> UpdateAsync(TIdentifier id, TSource source, CancellationToken ct = default);

        /// <summary>
        /// Send request to delete <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="id">Source's identifier</param>
        /// <param name="ct">Cancellation Token</param>
        Task DeleteAsync(TIdentifier id, CancellationToken ct = default);
    }
}
