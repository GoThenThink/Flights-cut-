using AutoMapper;
using RandomStartup.Extensions.Common.Types;
using Business.Models;
using Dapper;
using Flights.DAL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bm = Business.Models.Flight;
using Entity = Flights.DAL.Entities.Flight;

namespace Flights.DAL
{
    internal sealed class FlightsRepo : IFlightsRepo
    {
        private readonly IDbConnectionFactory _conn;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> AllowedColumns;
        private readonly Dictionary<string, string> _sortingColumns;

        public FlightsRepo(IDbConnectionFactory conn, IMapper mapper)
        {
            _conn = conn;
            _mapper = mapper;

            _sortingColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Bm.Id)] = "id",
                [nameof(Bm.Name)] = "name",
                [nameof(Bm.From)] = "\"from\"",
                [nameof(Bm.To)] = "\"to\"",
                [nameof(Bm.AirlineId)] = "airline_id",
                [nameof(Bm.FlightTypeId)] = "flight_type_id"
            };

            AllowedColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Entity.Name)] = "name",
                [nameof(Entity.From)] = "\"from\"",
                [nameof(Entity.To)] = "\"to\"",
                [nameof(Entity.AirlineId)] = "airline_id",
                [nameof(Entity.FlightTypeId)] = "flight_type_id",
                [nameof(Entity.TouristInfoEditLock)] = "tourist_info_edit_lock",
                [nameof(Entity.ReturnFlightSameAirlineRequired)] = "return_flight_same_airline_required",
                [nameof(Entity.AlternativeSearch)] = "alternative_search",
                [nameof(Entity.FlightDuration)] = "flight_duration",
                [nameof(Entity.AirplaneModelId)] = "airplane_model_id"
            };

        }

        /// <summary>
        /// Adds a flight item
        /// </summary>
        /// <param name="source">New item to be added.</param>
        /// <returns>The id of the newly created item.</returns>
        public async Task<Bm> AddAsync(Bm source)
        {
            #region SQL-query: INSERT INTO flights
            const string sqlQuery =
                "INSERT INTO " +
                    "flights (name, \"from\", \"to\", airline_id, flight_type_id, tourist_info_edit_lock, return_flight_same_airline_required, alternative_search, flight_duration, airplane_model_id, status_id) " +
                "VALUES (@Name, @From, @To, @AirlineId, @FlightTypeId, @TouristInfoEditLock, @ReturnFlightSameAirlineRequired, @AlternativeSearch, @FlightDuration, @AirplaneModelId, @StatusId) " +
                "RETURNING id AS Id";
            #endregion

            var entity = _mapper.Map<Entity>(source);

            using var conn = _conn.GetConnection();
            entity.Id = await conn.QuerySingleAsync<long>(sqlQuery, entity);
            return _mapper.Map<Bm>(entity);
        }

        /// <summary>
        /// Delete a specific flight
        /// </summary>
        /// <param name="id">The id of an item to be deleted.</param>
        /// <returns>"0" if item was not deleted, otherwise >"0".</returns>
        public async Task<int> DeleteAsync(long id)
        {
            const string sqlQuery = "DELETE FROM flights WHERE id=@id";

            using var conn = _conn.GetConnection();
            return await conn.ExecuteAsync(sqlQuery, new { id });
        }

        /// <summary>
        /// Find a specific flight in the database table
        /// </summary>
        /// <param name="id">An id of sought flight.</param>
        /// <returns>Requested flight.</returns>
        public async Task<Bm> GetAsync(long id)
        {
            #region SQL-query: SELECT
            const string sqlQuery =
               "WITH aggtab AS( " +
                    "SELECT * " +
                    "FROM flight_dates fd " +
                    "INNER JOIN flights fl ON fd.flight_id = @soughtId " +
                    "WHERE current_timestamp<fd.departure_date " +
                    "ORDER BY fd.departure_date-current_timestamp ASC " +
                "), " +
                 "nearest_dates AS( " +
                     "SELECT fd.flight_id, fd.departure_date, fd.arrival_date " +
                     "FROM flight_dates fd " +
                     "INNER JOIN (SELECT aggtab.flight_id, MIN(aggtab.departure_date) AS NearestDepDate " +
                     "FROM aggtab " +
                     "GROUP BY aggtab.flight_id) nd ON nd.flight_id = fd.flight_id " +
                     "WHERE fd.departure_date = nd.NearestDepDate " +
                     ") " +
                "SELECT " +
                       "fl.id AS Id, " +
                       "fl.name AS Name, " +
                       "fl.from AS \"From\", " +
                       "fl.to AS \"To\", " +
                       "nd.departure_date AS NearestDepartureDate, " +
                       "nd.arrival_date AS NearestArrivalDate, " +
                       "fl.airline_id AS AirLineId, " +
                       "fl.flight_type_id AS FlightTypeId, " +
                       "fl.tourist_info_edit_lock AS TouristInfoEditLock, " +
                       "fl.return_flight_same_airline_required AS ReturnFlightSameAirlineRequired, " +
                       "fl.alternative_search AS AlternativeSearch, " +
                       "fl.flight_duration AS FlightDuration, " +
                       "fl.airplane_model_id AS AirplaneModelId, " +
                       "fl.status_id AS StatusId, " +
                       "false AS IsReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id " +
                "WHERE fl.id=@soughtId;";
            #endregion

            var dateDictionary = new Dictionary<long?, Bm>();

            using var conn = _conn.GetConnection();
            return (await conn.QueryAsync<Bm, ReturnFlightIdsHelper, Bm>(sqlQuery, (flight, retFlightId) =>
            {
                if (!dateDictionary.TryGetValue(flight.Id, out Bm flightEntry))
                {
                    flightEntry = flight;
                    flightEntry.ReturnFlightIds = new List<long>();
                    dateDictionary.Add(flightEntry.Id, flightEntry);
                }
                flightEntry.ReturnFlightIds.Add(retFlightId.ReturnId);

                return flightEntry;
            }, new { soughtId = id })).Distinct().SingleOrDefault();
        }

        /// <summary>
        /// Show all flights from database table
        /// </summary>
        /// <returns>All records from flights table.</returns>
        public async Task<IReadOnlyList<Bm>> GetListAsync()
        {
            #region SQL-query: SELECT
            const string sqlQuery =
               "WITH aggtab AS( " +
                    "SELECT * " +
                    "FROM flight_dates fd " +
                    "INNER JOIN flights fl ON fd.flight_id = fl.id " +
                    "WHERE current_timestamp<fd.departure_date " +
                    "ORDER BY fd.departure_date-current_timestamp ASC " +
                "), " +
                 "nearest_dates AS( " +
                     "SELECT fd.flight_id, fd.departure_date, fd.arrival_date " +
                     "FROM flight_dates fd " +
                     "INNER JOIN (SELECT aggtab.flight_id, MIN(aggtab.departure_date) AS NearestDepDate " +
                     "FROM aggtab " +
                     "GROUP BY aggtab.flight_id) nd ON nd.flight_id = fd.flight_id " +
                     "WHERE fd.departure_date = nd.NearestDepDate " +
                     ") " +
                "SELECT " +
                       "fl.id AS Id, " +
                       "fl.name AS Name, " +
                       "fl.from AS \"From\", " +
                       "fl.to AS \"To\", " +
                       "nd.departure_date AS NearestDepartureDate, " +
                       "nd.arrival_date AS NearestArrivalDate, " +
                       "fl.airline_id AS AirLineId, " +
                       "fl.flight_type_id AS FlightTypeId, " +
                       "fl.tourist_info_edit_lock AS TouristInfoEditLock, " +
                       "fl.return_flight_same_airline_required AS ReturnFlightSameAirlineRequired, " +
                       "fl.alternative_search AS AlternativeSearch, " +
                       "fl.flight_duration AS FlightDuration, " +
                       "fl.airplane_model_id AS AirplaneModelId, " +
                       "fl.status_id AS StatusId, " +
                       "false AS IsReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id; ";
            #endregion

            var dateDictionary = new Dictionary<long?, Bm>();

            using var conn = _conn.GetConnection();
            return (await conn.QueryAsync<Bm, ReturnFlightIdsHelper, Bm>(sqlQuery, (flight, retFlightId) =>
            {
                if (!dateDictionary.TryGetValue(flight.Id, out Bm flightEntry))
                {
                    flightEntry = flight;
                    flightEntry.ReturnFlightIds = new List<long>();
                    dateDictionary.Add(flightEntry.Id, flightEntry);
                }
                flightEntry.ReturnFlightIds.Add(retFlightId.ReturnId);

                return flightEntry;
            })).Distinct().ToList();
        }

        /// <summary>
        /// Search specific flights with defined filters and pagination. 
        /// </summary>
        /// <param name="pagination">Pagination params.</param>
        /// <param name="filters">Filter params.</param>
        /// <returns>List of requested flights.</returns>
        public async Task<IReadOnlyList<Bm>> GetListAsync(FlightSearchFilters filters, Pagination pagination)
        {
            if (!_sortingColumns.TryGetValue(pagination.OrderBy, out var sortingColumn))
            {
                throw new Exception($"The sorting column is not found. {pagination.OrderBy}");
            }

            #region SQL-query: SELECT
            string sqlQuery =
               "WITH aggtab AS( " +
                    "SELECT * " +
                    "FROM flight_dates fd " +
                    "INNER JOIN flights fl ON fd.flight_id = fl.id " +
                    "WHERE current_timestamp<fd.departure_date " +
                    "ORDER BY fd.departure_date-current_timestamp ASC " +
                "), " +
                 "nearest_dates AS( " +
                     "SELECT fd.flight_id, fd.departure_date, fd.arrival_date " +
                     "FROM flight_dates fd " +
                     "INNER JOIN (SELECT aggtab.flight_id, MIN(aggtab.departure_date) AS NearestDepDate " +
                     "FROM aggtab " +
                     "GROUP BY aggtab.flight_id) nd ON nd.flight_id = fd.flight_id " +
                     "WHERE fd.departure_date = nd.NearestDepDate " +
                     ") " +
                "SELECT " +
                       "fl.id AS Id, " +
                       "fl.name AS Name, " +
                       "fl.from AS \"From\", " +
                       "fl.to AS \"To\", " +
                       "nd.departure_date AS NearestDepartureDate, " +
                       "nd.arrival_date AS NearestArrivalDate, " +
                       "fl.airline_id AS AirLineId, " +
                       "fl.flight_type_id AS FlightTypeId, " +
                       "fl.tourist_info_edit_lock AS TouristInfoEditLock, " +
                       "fl.return_flight_same_airline_required AS ReturnFlightSameAirlineRequired, " +
                       "fl.alternative_search AS AlternativeSearch, " +
                       "fl.flight_duration AS FlightDuration, " +
                       "fl.airplane_model_id AS AirplaneModelId, " +
                       "fl.status_id AS StatusId, " +
                       "false AS IsReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id " +
                "WHERE " +
                    $"((@SearchByName IS NULL) OR (LOWER(fl.name) LIKE @SearchByName)) AND " +
                    "(@Airlines ::int[] IS NULL OR fl.airline_id = ANY (@Airlines)) AND " +
                    "(@Statuses ::int[] IS NULL OR fl.status_id = ANY (@Statuses)) " +
                $"ORDER BY {sortingColumn} {pagination.OrderDirection} " +
                $"OFFSET {pagination.Skip} LIMIT {pagination.Take}; ";
            #endregion

            var dateDictionary = new Dictionary<long?, Bm>();

            using var conn = _conn.GetConnection();
            filters.SearchByName += "%";
            return (await conn.QueryAsync<Bm, ReturnFlightIdsHelper, Bm>(sqlQuery, (flight, retFlightId) =>
            {
                if (!dateDictionary.TryGetValue(flight.Id, out Bm flightEntry))
                {
                    flightEntry = flight;
                    flightEntry.ReturnFlightIds = new List<long>();
                    dateDictionary.Add(flightEntry.Id, flightEntry);
                }
                flightEntry.ReturnFlightIds.Add(retFlightId.ReturnId);

                return flightEntry;
            }, filters)).Distinct().ToList();
        }

        /// <summary>
        /// Updates content of an existed flight
        /// </summary>
        /// <param name="id">The id of an item to be updated.</param>
        /// <param name="source">The information to update content of an existed flight.</param>
        /// <returns>"0" if item was not updated, otherwise >"0".</returns>
        public async Task<Bm> UpdateAsync(long id, Bm source)
        {
            #region SQL-query: UPDATE flights SET
            const string sqlQueryUpdate =
                "UPDATE " +
                    "flights " +
                "SET " +
                    "name=@Name, " +
                    "\"from\"=@From, " +
                    "\"to\"=@To, " +
                    "airline_id=@AirlineId, " +
                    "flight_type_id=@FlightTypeId, " +
                    "tourist_info_edit_lock=@TouristInfoEditLock, " +
                    "return_flight_same_airline_required=@ReturnFlightSameAirlineRequired, " +
                    "alternative_search=@AlternativeSearch, " +
                    "flight_duration=@FlightDuration, " +
                    "airplane_model_id=@AirplaneModelId " +
                "WHERE " +
                    "id=@Id";
            #endregion
            #region SQL-query: SELECT FROM flights
            const string sqlQuerySelect =
               "WITH aggtab AS( " +
                    "SELECT * " +
                    "FROM flight_dates fd " +
                    "INNER JOIN flights fl ON fd.flight_id = @soughtId " +
                    "WHERE current_timestamp<fd.departure_date " +
                    "ORDER BY fd.departure_date-current_timestamp ASC " +
                "), " +
                 "nearest_dates AS( " +
                     "SELECT fd.flight_id, fd.departure_date, fd.arrival_date " +
                     "FROM flight_dates fd " +
                     "INNER JOIN (SELECT aggtab.flight_id, MIN(aggtab.departure_date) AS NearestDepDate " +
                     "FROM aggtab " +
                     "GROUP BY aggtab.flight_id) nd ON nd.flight_id = fd.flight_id " +
                     "WHERE fd.departure_date = nd.NearestDepDate " +
                     ") " +
                "SELECT " +
                       "fl.id AS Id, " +
                       "fl.name AS Name, " +
                       "fl.from AS \"From\", " +
                       "fl.to AS \"To\", " +
                       "nd.departure_date AS NearestDepartureDate, " +
                       "nd.arrival_date AS NearestArrivalDate, " +
                       "fl.airline_id AS AirLineId, " +
                       "fl.flight_type_id AS FlightTypeId, " +
                       "fl.tourist_info_edit_lock AS TouristInfoEditLock, " +
                       "fl.return_flight_same_airline_required AS ReturnFlightSameAirlineRequired, " +
                       "fl.alternative_search AS AlternativeSearch, " +
                       "fl.flight_duration AS FlightDuration, " +
                       "fl.airplane_model_id AS AirplaneModelId, " +
                       "fl.status_id AS StatusId, " +
                       "false AS IsReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id " +
                "WHERE fl.id=@soughtId;";
            #endregion

            var dateDictionary = new Dictionary<long?, Bm>();

            var entity = _mapper.Map<Entity>(source, opt => { opt.AfterMap((source, dest) => dest.Id = id); });

            using var conn = _conn.GetConnection();
            await conn.ExecuteAsync(sqlQueryUpdate, entity);
            return (await conn.QueryAsync<Bm, ReturnFlightIdsHelper, Bm>(sqlQuerySelect, (flight, retFlightId) =>
            {
                if (!dateDictionary.TryGetValue(flight.Id, out Bm flightEntry))
                {
                    flightEntry = flight;
                    flightEntry.ReturnFlightIds = new List<long>();
                    dateDictionary.Add(flightEntry.Id, flightEntry);
                }
                flightEntry.ReturnFlightIds.Add(retFlightId.ReturnId);

                return flightEntry;
            }, new { soughtId = id })).Distinct().SingleOrDefault();
        }

        /// <summary>
        /// Partial update of a record
        /// </summary>
        /// <param name="id">Id of a record.</param>
        /// <param name="property">Column name in database.</param>
        /// <param name="model">New value.</param>
        /// <returns>Updated item.</returns>
        public async Task<Bm> PatchRecordAsync(long id, string property, Bm model)
        {
            var tablePropertyName = AllowedColumns.GetValueOrDefault(property);

            #region SQL-query: UPDATE flights SET
            string sqlQueryUpdate = $"UPDATE flights SET {tablePropertyName}=@{property} WHERE id=@id;";
            #endregion
            #region SQL-query: SELECT FROM flights
            const string sqlQuerySelect =
               "WITH aggtab AS( " +
                    "SELECT * " +
                    "FROM flight_dates fd " +
                    "INNER JOIN flights fl ON fd.flight_id = @soughtId " +
                    "WHERE current_timestamp<fd.departure_date " +
                    "ORDER BY fd.departure_date-current_timestamp ASC " +
                "), " +
                 "nearest_dates AS( " +
                     "SELECT fd.flight_id, fd.departure_date, fd.arrival_date " +
                     "FROM flight_dates fd " +
                     "INNER JOIN (SELECT aggtab.flight_id, MIN(aggtab.departure_date) AS NearestDepDate " +
                     "FROM aggtab " +
                     "GROUP BY aggtab.flight_id) nd ON nd.flight_id = fd.flight_id " +
                     "WHERE fd.departure_date = nd.NearestDepDate " +
                     ") " +
                "SELECT " +
                       "fl.id AS Id, " +
                       "fl.name AS Name, " +
                       "fl.from AS \"From\", " +
                       "fl.to AS \"To\", " +
                       "nd.departure_date AS NearestDepartureDate, " +
                       "nd.arrival_date AS NearestArrivalDate, " +
                       "fl.airline_id AS AirLineId, " +
                       "fl.flight_type_id AS FlightTypeId, " +
                       "fl.tourist_info_edit_lock AS TouristInfoEditLock, " +
                       "fl.return_flight_same_airline_required AS ReturnFlightSameAirlineRequired, " +
                       "fl.alternative_search AS AlternativeSearch, " +
                       "fl.flight_duration AS FlightDuration, " +
                       "fl.airplane_model_id AS AirplaneModelId, " +
                       "fl.status_id AS StatusId, " +
                       "false AS IsReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id " +
                "WHERE fl.id=@soughtId;";
            #endregion

            var dateDictionary = new Dictionary<long?, Bm>();

            var entity = _mapper.Map<Entity>(model, opt => { opt.AfterMap((model, dest) => dest.Id = id); });

            using var conn = _conn.GetConnection();
            await conn.ExecuteAsync(sqlQueryUpdate, entity);
            return (await conn.QueryAsync<Bm, ReturnFlightIdsHelper, Bm>(sqlQuerySelect, (flight, retFlightId) =>
            {
                if (!dateDictionary.TryGetValue(flight.Id, out Bm flightEntry))
                {
                    flightEntry = flight;
                    flightEntry.ReturnFlightIds = new List<long>();
                    dateDictionary.Add(flightEntry.Id, flightEntry);
                }
                flightEntry.ReturnFlightIds.Add(retFlightId.ReturnId);

                return flightEntry;
            }, new { soughtId = id })).Distinct().SingleOrDefault();
        }

        /// <inheritdoc cref="IFlightsRepo.SetStatusAsync(long, short, System.Data.IDbConnection)"/>
        public async Task<int> SetStatusAsync(long id, short statusId, System.Data.IDbConnection conn = null)
        {
            const string sqlQuery =
                "UPDATE flights SET status_id=@statusId WHERE id=@id; ";

            if (!(conn is null)) return await conn.ExecuteAsync(sqlQuery, new { id, statusId });

            using (conn = _conn.GetConnection())
            {
                return await conn.ExecuteAsync(sqlQuery, new { id, statusId });
            }
        }
    }
}
