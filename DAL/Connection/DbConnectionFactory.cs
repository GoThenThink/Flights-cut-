using Flights.DAL.Abstractions;
using Npgsql;
using System.Data;

namespace Flights.DAL
{
    internal sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
