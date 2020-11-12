using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.DAL.Abstractions
{
    /// <summary>
    /// Base interface for repositories
    /// </summary>
    public interface IBaseEntityRepository<in TBusinessModelId, TBusinessModel>
    {
        /// <summary>
        /// Base interface for Create method
        /// </summary>
        Task<TBusinessModel> AddAsync(TBusinessModel source);

        /// <summary>
        /// Base interface for Get method
        /// </summary>
        Task<TBusinessModel> GetAsync(TBusinessModelId id);

        /// <summary>
        /// Base interface for GetList method
        /// </summary>
        Task<IReadOnlyList<TBusinessModel>> GetListAsync();

        /// <summary>
        /// Base interface for Update method
        /// </summary>
        Task<TBusinessModel> UpdateAsync(TBusinessModelId id, TBusinessModel source);

        /// <summary>
        /// Base interface for Delete method
        /// </summary>
        Task<int> DeleteAsync(TBusinessModelId id);

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in a class.</param>
        /// <param name="model">New value.</param>
        /// <returns>Updated item.</returns>
        Task<TBusinessModel> PatchRecordAsync(TBusinessModelId id, string property, TBusinessModel model);
    }
}
