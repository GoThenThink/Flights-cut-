using Dapper;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace Flights.DAL
{
    internal sealed class GeometryHandler<TGeometry> : SqlMapper.TypeHandler<TGeometry>
    {
        public override TGeometry Parse(object value)
        {
            if (value is TGeometry geometry)
            {
                return geometry;
            }

            throw new ArgumentException();
        }

        public override void SetValue(IDbDataParameter parameter, TGeometry value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Geometry;
                npgsqlParameter.NpgsqlValue = value;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
