using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Business.Models.Extensions
{
    public static class FilterExtensions
    {
        public static FlightSearchFilters DeepClone<FlightSearchFilters>(this FlightSearchFilters obj)
        {
            using var ms = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (FlightSearchFilters)formatter.Deserialize(ms);
        }

    }
}
