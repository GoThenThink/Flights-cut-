using Dapper;
using Flights.DAL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bm = Business.Models.Airline;

namespace Flights.DAL
{
    internal sealed class AirlinesRepo : IAirlinesRepo
    {
        private readonly IDbConnectionFactory _conn;
        private readonly Dictionary<string, string> AllowedColumns;

        public AirlinesRepo(IDbConnectionFactory conn)
        {
            _conn = conn;

            AllowedColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Bm.Name)] = "name",
                [nameof(Bm.Iata)] = "iata"
            };
        }

        /// <summary>
        /// Adds an airline item
        /// </summary>
        /// <param name="source">New item to be added.</param>
        /// <returns>The id of the newly created item.</returns>
        public async Task<Bm> AddAsync(Bm source)
        {
            const string sqlQuery = "INSERT INTO airlines (name, iata) VALUES (@Name, @Iata) RETURNING id AS Id";
            using var conn = _conn.GetConnection();
            source.Id = await conn.QuerySingleAsync<int>(sqlQuery, source);
            return source;
        }

        /// <summary>
        /// Delete a specific airline
        /// </summary>
        /// <param name="id">The id of an item to be deleted.</param>
        /// <returns>"0" if item was not deleted, otherwise >"0".</returns>
        public async Task<int> DeleteAsync(int id)
        {
            const string sqlQuery = "DELETE FROM airlines WHERE id=@id";
            using var conn = _conn.GetConnection();
            return await conn.ExecuteAsync(sqlQuery, new { id });
        }

        /// <summary>
        /// Find a specific airline in the database table
        /// </summary>
        /// <param name="id">An id of sought airline.</param>
        /// <returns>Requested airline.</returns>
        public async Task<Bm> GetAsync(int id)
        {
            const string sqlQuery = "SELECT id AS Id, iata AS Iata, name AS Name FROM airlines WHERE id = @id";
            using var conn = _conn.GetConnection();
            return await conn.QuerySingleOrDefaultAsync<Bm>(sqlQuery, new { id });
        }

        /// <summary>
        /// Show all airlines from database table
        /// </summary>
        /// <returns>All records from airlines table.</returns>
        public async Task<IReadOnlyList<Bm>> GetListAsync()
        {
            const string sqlQuery = "SELECT id AS Id, iata AS Iata, name AS Name FROM airlines";
            using var conn = _conn.GetConnection();
            return (await conn.QueryAsync<Bm>(sqlQuery)).ToList();
        }

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in a class.</param>
        /// <param name="model">New value.</param>
        /// <returns>Updated item.</returns>
        public async Task<Bm> PatchRecordAsync(int id, string property, Bm model)
        {
            var tablePropertyName = AllowedColumns.GetValueOrDefault(property);

            model.Id = id;

            string sqlQuery = $"UPDATE airlines SET {tablePropertyName}=@{property} WHERE id=@id RETURNING id AS Id, iata AS Iata, name AS Name;";
            using var conn = _conn.GetConnection();
            return await conn.QuerySingleOrDefaultAsync<Bm>(sqlQuery, model);
        }

        /// <summary>
        /// Updates content of an existed airline
        /// </summary>
        /// <param name="id">The id of an item to be updated.</param>
        /// <param name="source">The information to update content of an existed airline.</param>
        /// <returns>"0" if item was not updated, otherwise >"0".</returns>
        public async Task<Bm> UpdateAsync(int id, Bm source)
        {
            source.Id = id;

            const string sqlQuery = "UPDATE airlines SET name=@Name, iata=@Iata WHERE id=@Id RETURNING id AS Id, iata AS Iata, name AS Name;";
            using var conn = _conn.GetConnection();
            return await conn.QuerySingleOrDefaultAsync<Bm>(sqlQuery, source);
        }
    }
}
