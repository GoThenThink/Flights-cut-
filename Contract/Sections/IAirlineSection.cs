using Flights.Contract.Base;
using Flights.Contract.Dto;

namespace Flights.Contract.Sections
{
    /// <summary>
    /// Section for work with <see cref="AirlineDto"/>
    /// </summary>
    public interface IAirlineSection : IBaseSection<int, AirlineDto>
    {
    }
}
