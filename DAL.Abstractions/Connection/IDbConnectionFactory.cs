using System.Data;

namespace Flights.DAL.Abstractions
{
    /// <summary>
    /// Interface for DB connection factory
    /// </summary>
    public interface IDbConnectionFactory
    {
        ///<summary>
        ///Return connection
        ///</summary>
        IDbConnection GetConnection();
    }
}
