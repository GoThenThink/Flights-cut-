using Flights;
using Flights.Contract.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using Xunit;
using Dto = Flights.Contract.Dto.AirlineDto;

namespace Contract.Client.Tests
{
    public sealed partial class AirlineSectionTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public static T GetPropertyValue<T>(object obj, string propName) { return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); }
        public static string GetPropertyName(object obj, string propName) { return obj.GetType().GetProperty(propName).Name; }

        public static IEnumerable<object[]> ListOfAirlinesForPassingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                     Name = "AtestAirline1",
                     Iata = "ATA1"
                } },
                new object[]{new Dto
                {
                     Name = "BtestAirline2",
                     Iata = "BTA2"
                } },
                new object[]{new Dto
                {
                     Name = "CtestAirline3",
                     Iata = "CTA3"
                } },
                new object[]{new Dto
                {
                     Name = "DtestAirline4",
                     Iata = "DTA4"
                } },
                new object[]{new Dto
                {
                    Name = "EtestAirline5",
                    Iata = "ETA5"
                }}               
            };
        }

        public static IEnumerable<object[]> ListOfAirlinesForFailingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                     Name = "",
                     Iata = "AT11"
                } },
                new object[]{new Dto
                {
                     Name = "BtestAirline22",
                     Iata = ""
                } },
                new object[]{new Dto
                {
                     Name = "",
                     Iata = ""
                } },
                new object[]{new Dto
                {
                     Name = "DtestAirline44"
                } },
                new object[]{new Dto
                {
                    Iata = "ET55"
                }},
                new object[]{new Dto
                {
                    Name = "FtestAirline66",
                    Iata = "FTA6FFF"
                }}
            };
        }

        public static IEnumerable<object[]> PatchAirlinesForPassingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                     Name = "AtestAirline111"
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {
                     Iata = "B222"
                }, nameof(Dto.Iata) },
                new object[]{new Dto
                {
                     Name = "CtestAirline333"
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {
                     Iata = "D444"
                }, nameof(Dto.Iata) },
                new object[]{new Dto
                {
                    Name = "EtestAirline555"
                }, nameof(Dto.Name)}
            };
        }

        public static IEnumerable<object[]> PatchAirlinesForFailingTest()
        {
            return new List<object[]>
            {
                new object[]{ new Dto
                {
                     Name = "",
                     Iata = "1111"
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {

                }, nameof(AirlineDto.Iata) },
                new object[]{new AirlineDto
                {
                     Iata = "3333"
                }, nameof(Dto.Name) },
                new object[]{new Dto
                {
                     Name = "DtestAirline4444",
                     Iata = "DTA48"
                }, nameof(Dto.Iata) },
                new object[]{new Dto
                {
                    Name = "EtestAirline5555",
                    Iata = ""
                }, nameof(Dto.Iata)}
            };
        }
    }
}
