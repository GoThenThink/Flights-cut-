using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.Business.Abstractions
{
    /// <summary>
    /// Base service for work with records with id
    /// </summary>
    public interface IRecordBaseService<TBusinessModel, in TBusinessModelId>
    {
        /// <summary>
        /// Shows all records from the database table
        /// </summary>
        /// <returns>List of requested items.</returns>
        Task<IReadOnlyList<TBusinessModel>> GetListAsync();

        /// <summary>
        /// Returns record information by specified id
        /// </summary>
        /// <param name="id">An id of sought record.</param>
        /// <returns>Requested item.</returns>
        Task<TBusinessModel> GetAsync(TBusinessModelId id);

        /// <summary>
        /// Adds a new item to DB
        /// </summary>
        /// <param name="model">New item to be added.</param>
        /// <returns>Newly created item.</returns>
        Task<TBusinessModel> AddAsync(TBusinessModel model);

        /// <summary>
        /// Updates content of an existed record
        /// </summary>
        /// <param name="id">The id of an item to be updated.</param>
        /// <param name="model">The information to update content of an existed record.</param>
        /// <returns>Updated item.</returns>
        Task<TBusinessModel> UpdateAsync(TBusinessModelId id, TBusinessModel model);

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in a database.</param>
        /// <param name="model">New value.</param>
        /// <returns>Updated item.</returns>
        Task<TBusinessModel> PatchRecordAsync(TBusinessModelId id, string property, TBusinessModel model);

        /// <summary>
        /// Delete a specific item
        /// </summary>
        /// <param name="id">The id of an item to be deleted.</param>
        /// <returns>"0" if item was not deleted, otherwise >"0".</returns>
        Task<int> DeleteAsync(TBusinessModelId id);
    }

}
