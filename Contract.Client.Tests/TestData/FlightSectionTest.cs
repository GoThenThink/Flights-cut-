using Flights;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using Xunit;
using Dto = Flights.Contract.Dto.FlightDto;

namespace Contract.Client.Tests
{
    public sealed partial class FlightSectionTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public static T GetPropertyValue<T>(object obj, string propName) { return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); }
        public static string GetPropertyName(object obj, string propName) { return obj.GetType().GetProperty(propName).Name; }

        public static IEnumerable<object[]> ListOfFlightsForPassingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                    Name = "ATestFlight1",
                    From = 1, 
                    To = 7, 
                    AirlineId = 1, 
                    FlightTypeId = 1, 
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false, 
                    AlternativeSearch = false, 
                    FlightDuration = 1, 
                    AirplaneModelId = 1, 
                    StatusId = 1
                } },
                new object[]{new Dto
                {
                    Name = "BTestFlight2",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                } },
                new object[]{new Dto
                {
                    Name = "CTestFlight3",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                } },
                new object[]{new Dto
                {
                    Name = "DTestFlight4",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                } },
                new object[]{new Dto
                {
                    Name = "ETestFlight5",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                }}               
            };
        }

        public static IEnumerable<object[]> ListOfFlightsForFailingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                    Name = "ATestFlight11",
                    From = 1,
                    To = 7,
                    AirlineId = 0,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1,
                    IsReturnFlight = false
                } },
                new object[]{new Dto
                {
                    Name = "BTestFlight22",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 5,
                    IsReturnFlight = false
                } },
                new object[]{new Dto
                {
                    Name = "",
                    From = -1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1,
                    IsReturnFlight = false
                } },
                new object[]{new Dto
                {
                    Name = "DTestFlight44",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1,
                    IsReturnFlight = false
                } },
                new object[]{new Dto
                {

                }}
            };
        }

        public static IEnumerable<object[]> PatchFlightsForPassingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                    Name = "ATestFlight111"
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {
                    Name = "BTestFlight222",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 100,
                    AirplaneModelId = 1,
                    StatusId = 3
                }, nameof(Dto.FlightDuration) },
                new object[]{new Dto
                {
                    Name = "CTestFlight333",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 2,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                }, nameof(Dto.FlightTypeId) },
                new object[]{new Dto
                {
                    From = 5,
                    StatusId = 1
                }, nameof(Dto.From) },
                new object[]{new Dto
                {
                    TouristInfoEditLock = 5
                }, nameof(Dto.TouristInfoEditLock)}
            };
        }

        public static IEnumerable<object[]> PatchFlightsForFailingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                    Name = "",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 3
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {
                    Name = "BTestFlight2222",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 1,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 6
                }, nameof(Dto.StatusId) },
                new object[]{new Dto
                {
                    Name = "CTestFlight3333",
                    From = 1,
                    To = 7,
                    AirlineId = 1,
                    FlightTypeId = 2,
                    TouristInfoEditLock = 5,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                }, nameof(Dto.ReturnFlightSameAirlineRequired) },
                new object[]{new Dto
                {
                    Name = "DTestFlight4444",
                    From = 1,
                    To = 7,
                    AirlineId = 0,
                    FlightTypeId = 2,
                    TouristInfoEditLock = 5,
                    ReturnFlightSameAirlineRequired = false,
                    AlternativeSearch = false,
                    FlightDuration = 1,
                    AirplaneModelId = 1,
                    StatusId = 1
                }, nameof(Dto.AirlineId) },
                new object[]{new Dto
                {
                    FlightDuration = -1
                }, nameof(Dto.FlightDuration)}
            };
        }
    }
}
