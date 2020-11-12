using AutoMapper;
using Dapper;
using Flights.DAL.Abstractions;
using Flights.DAL.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace Flights.DAL
{
    /// <summary>
    /// Class for setting up the Data Access Layer
    /// </summary>
    public static class DependencyInjection
    {
        ///<summary>
        /// Add all DAL components
        /// </summary>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            AddPostGisHandlers();

            return services
                .AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString))
                .RegisterRepositories()
                .RegisterMapperProfiles();

        }

        private static IServiceCollection RegisterRepositories(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IAirlinesRepo, AirlinesRepo>()
                .AddSingleton<IFlightsRepo, FlightsRepo>()
                .AddSingleton<IReturnFlightsRepo, ReturnFlightsRepo>();

        }

        private static IServiceCollection RegisterMapperProfiles(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<Profile, FlightProfile>()
                .AddSingleton<Profile, ReturnFlightProfile>();

        }

        private static void AddPostGisHandlers()
        {
            NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
            SqlMapper.AddTypeHandler(new GeometryHandler<NetTopologySuite.Geometries.Point>());
            SqlMapper.AddTypeHandler(new GeometryHandler<NetTopologySuite.Geometries.MultiPolygon>());
        }

    }
}
