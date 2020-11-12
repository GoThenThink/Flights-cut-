using AutoMapper;
using Dapper;
using Flights.DAL.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bm = Business.Models.Flight;
using Entity = Flights.DAL.Entities.ReturnFlight;

namespace Flights.DAL
{
    internal sealed class ReturnFlightsRepo : IReturnFlightsRepo
    {
        private readonly IDbConnectionFactory _conn;
        private readonly IMapper _mapper;

        public ReturnFlightsRepo(IDbConnectionFactory conn, IMapper mapper)
        {
            _conn = conn;
            _mapper = mapper;
        }

        /// <inheritdoc cref="IReturnFlightsRepo.AddAsync(long, Bm, System.Data.IDbConnection)"/>
        public async Task<int> AddAsync(long directFlightId, Bm source, System.Data.IDbConnection conn = null)
        {
            #region SQL-query: INSERT INTO return_flights
            const string sqlQuery =
                "INSERT INTO " +
                    "return_flights (flight_to_id, flight_from_id, priority, is_available, is_native_return_flight) " +
                "SELECT @FlightToId, @FlightFromId, @Priority, " +
                    "CASE WHEN (SELECT fl.status_id FROM flights fl WHERE fl.id=@FlightToId)<>1 OR " +
                              "(SELECT fl.status_id FROM flights fl WHERE fl.id=@FlightFromId)<>1 " +
                         "THEN false " +
                         "ELSE @IsAvailable " +
                    "END AS is_available, " +
                    "@IsNativeReturnFlight ";
            #endregion

            var entity = _mapper.Map<Entity>(source, opt => { opt.AfterMap((source, dest) => dest.FlightToId = directFlightId); });

            if (!(conn is null)) return await conn.ExecuteAsync(sqlQuery, entity);

            using (conn = _conn.GetConnection())
            {
                return await conn.ExecuteAsync(sqlQuery, entity);
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.DeleteAsync(long, long, System.Data.IDbConnection)"/>
        public async Task<bool> DeleteAsync(long directFlightId, long returnFlightId, System.Data.IDbConnection conn = null)
        {
            const string sqlQuery = "DELETE FROM return_flights WHERE flight_to_id=@directFlightId AND flight_from_id=@returnFlightId RETURNING is_native_return_flight;";

            if (!(conn is null)) return await conn.QuerySingleAsync<bool>(sqlQuery, new { directFlightId, returnFlightId });

            using (conn = _conn.GetConnection())
            {
                return await conn.QuerySingleAsync<bool>(sqlQuery, new { directFlightId, returnFlightId });
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.GetAsync(long, long)"/>
        public async Task<Bm> GetAsync(long directFlightId, long returnFlightId)
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
                "), " +
                "return_flights_short AS( " +
                    "SELECT * " +
                    "FROM return_flights rf " +
                    "WHERE flight_to_id=@DirectFlightId " +
                    "AND flight_from_id=@ReturnFlightId " +
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
                       "true AS IsReturnFlight, " +
                       "rfs.flight_to_id AS FlightToId, " +
                       "rfs.priority AS Priority, " +
                       "rfs.is_available AS IsAvailable, " +
                       "rfs.is_native_return_flight AS IsNativeReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "INNER JOIN return_flights_short rfs ON fl.id=rfs.flight_from_id " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id ";
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
            }, new { DirectFlightId = directFlightId, ReturnFlightId = returnFlightId})).Distinct().SingleOrDefault();
        }

        /// <inheritdoc cref="IReturnFlightsRepo.GetListAsync(long)"/>
        public async Task<IReadOnlyList<Bm>> GetListAsync(long directFlightId)
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
                "), " +
                "return_flights_short AS( " +
                    "SELECT * " +
                    "FROM return_flights rf " +
                    "WHERE flight_to_id=@directFlightId " +
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
                       "true AS IsReturnFlight, " +
                       "rfs.flight_to_id AS FlightToId, " +
                       "rfs.priority AS Priority, " +
                       "rfs.is_available AS IsAvailable, " +
                       "rfs.is_native_return_flight AS IsNativeReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "INNER JOIN return_flights_short rfs ON fl.id=rfs.flight_from_id " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id ";
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
            }, new { directFlightId })).Distinct().ToList();
        }

        /// <inheritdoc cref="IReturnFlightsRepo.GetListAsync(IReadOnlyList{long})"/>
        public async Task<IReadOnlyList<Bm>> GetListAsync(IReadOnlyList<long> directFlightIds)
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
                "), " +
                "return_flights_short AS( " +
                    "SELECT * " +
                    "FROM return_flights rf " +
                    "WHERE " +
                        "(flight_to_id = ANY (@directFlightIds)) " +
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
                       "true AS IsReturnFlight, " +
                       "rfs.flight_to_id AS FlightToId, " +
                       "rfs.priority AS Priority, " +
                       "rfs.is_available AS IsAvailable, " +
                       "rfs.is_native_return_flight AS IsNativeReturnFlight, " +
                       "fl.id, " +
                       "rf.flight_from_id AS ReturnId " +
                "FROM flights fl " +
                "INNER JOIN return_flights_short rfs ON fl.id=rfs.flight_from_id " +
                "LEFT JOIN return_flights rf ON fl.id = rf.flight_to_id " +
                "LEFT JOIN nearest_dates nd ON nd.flight_id = fl.id ";
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
            }, new { directFlightIds })).Distinct().ToList();
        }

        /// <inheritdoc cref="IReturnFlightsRepo.RecalculatePriorityAsync(long, int?, System.Data.IDbConnection)"/>
        public async Task<int> RecalculatePriorityAsync(long directFlightId, int? priority, System.Data.IDbConnection conn = null)
        {
            #region SQL-query: UPDATE without priority
            const string sqlQueryWithoutPriority =
                "WITH sorted_return_flight AS( "+
                    "SELECT rf.flight_from_id, row_number() over(ORDER BY rf.priority) AS row_number " +
                        "FROM return_flights rf " +
                        "WHERE rf.flight_to_id = @directFlightId " +
                        "ORDER BY rf.priority ASC " +
                    ") " +
                "UPDATE return_flights " +
                "SET priority = srf.row_number " +
                "FROM sorted_return_flight srf " +
                "WHERE return_flights.flight_to_id = @directFlightId AND " +
                      "return_flights.flight_from_id = srf.flight_from_id";
            #endregion
            #region SQL-query: UPDATE with priority
            const string sqlQueryWithPriority =
                "WITH sorted_return_flight AS( " +
                    "SELECT rf.flight_from_id " +
                        "FROM return_flights rf " +
                        "WHERE rf.flight_to_id = @directFlightId AND " +
                              "rf.priority >= @priority " +
                    ") " +
                "UPDATE return_flights " +
                "SET priority = priority + 1 " +
                "FROM sorted_return_flight srf " +
                "WHERE return_flights.flight_to_id = @directFlightId AND " +
                      "return_flights.flight_from_id = srf.flight_from_id";
            #endregion

            var sqlQuery = priority is null ? sqlQueryWithoutPriority : sqlQueryWithPriority;

            if (!(conn is null))
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, priority });
                if (result.Equals(0))
                    return 1;
                return result;
            }

            using (conn = _conn.GetConnection())
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, priority });
                if (result.Equals(0))
                    return 1;
                return result;
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.SetPriorityAsync(long, long, int, System.Data.IDbConnection)"/>
        public async Task<int> SetPriorityAsync(long directFlightId, long returnFlightId, int priority, System.Data.IDbConnection conn = null)
        {
            const string sqlQueryGeneral =
                "UPDATE return_flights SET priority=@priority WHERE flight_to_id=@directFlightId AND flight_from_id=@returnFlightId; ";

            const string sqlQueryForPriorityOne =
                "UPDATE return_flights SET priority=@priority, is_native_return_flight=true WHERE flight_to_id=@directFlightId AND flight_from_id=@returnFlightId; ";

            var sqlQuery = priority.Equals(1) ? sqlQueryForPriorityOne : sqlQueryGeneral;

            if (!(conn is null))
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, returnFlightId, priority });
                if (result.Equals(0))
                    return 1;
                return result;
            }

            using (conn = _conn.GetConnection())
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, returnFlightId, priority });
                if (result.Equals(0))
                    return 1;
                return result;
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.UnsetNativeAsync(long, System.Data.IDbConnection)"/>
        public async Task<int> UnsetNativeAsync(long directFlightId, System.Data.IDbConnection conn = null)
        {
            const string sqlQuery =
                "UPDATE return_flights SET is_native_return_flight=false WHERE flight_to_id=@directFlightId; ";

            if (!(conn is null))
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId });
                if (result.Equals(0))
                    return 1;
                return result;
            }

            using (conn = _conn.GetConnection())
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId });
                if (result.Equals(0))
                    return 1;
                return result;
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.SetNativeAsync(long, System.Data.IDbConnection)"/>
        public async Task<int> SetNativeAsync(long directFlightId, System.Data.IDbConnection conn = null)
        {
            const string sqlQuery =
                "UPDATE return_flights SET is_native_return_flight=true WHERE flight_to_id=@directFlightId AND priority=1; ";

            if (!(conn is null))
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId });
                if (result.Equals(0))
                    return 1;
                return result;
            }

            using (conn = _conn.GetConnection())
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId });
                if (result.Equals(0))
                    return 1;
                return result;
            }
        }

        /// <inheritdoc cref="IReturnFlightsRepo.SetStatusAsync(long?, long, bool, System.Data.IDbConnection)"/>
        public async Task<int> SetStatusAsync(long? directFlightId, long returnFlightId, bool newStatus, System.Data.IDbConnection conn = null)
        {
            #region SQL-query: UPDATE return_flights with direct flight id
            const string sqlQueryWithDirectFlight =
                "UPDATE " +
                    "return_flights " +
                "SET is_available = (SELECT CASE " +
                               "WHEN(SELECT fl.status_id FROM flights fl WHERE fl.id = @directFlightId) <> 1 OR " +
                                   "(SELECT fl.status_id FROM flights fl WHERE fl.id = @returnFlightId) <> 1 " +
                                   "THEN false " +
                               "ELSE @newStatus " +
                               "END AS is_available) " +
                "WHERE " +
                    "flight_to_id=@directFlightId AND " +
                    "flight_from_id=@returnFlightId; ";
            #endregion
            #region SQL-query: UPDATE return_flights without direct flight id
            const string sqlQueryWithoutDirectFlight =
                "UPDATE " +
                    "return_flights " +
                "SET is_available = (SELECT CASE " +
                               "WHEN(SELECT fl.status_id FROM flights fl WHERE fl.id = @returnFlightId) <> 1 " +
                                   "THEN false " +
                               "ELSE @newStatus " +
                               "END AS is_available) " +
                "WHERE " +
                    "flight_from_id=@returnFlightId; ";
            #endregion

            var sqlQuery = directFlightId.Equals(null) ? sqlQueryWithoutDirectFlight : sqlQueryWithDirectFlight;

            if (!(conn is null))
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, returnFlightId, newStatus });
                if (result.Equals(0))
                    return 1;
                return result;
            }

            using (conn = _conn.GetConnection()) 
            {
                var result = await conn.ExecuteAsync(sqlQuery, new { directFlightId, returnFlightId, newStatus });
                if (result.Equals(0))
                    return 1;
                return result;
            }
        }
    }
}
