using Contract.Client.Tests.Configuration;
using ExpectedObjects;
using Flights;
using Flights.Contract.Client;
using Flights.Contract.Sections;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Dto = Flights.Contract.Dto.FlightDto;

namespace Contract.Client.Tests
{
    public sealed partial class FlightSectionTest : Configs
    {
        private readonly IFlightSection section;

        public FlightSectionTest(WebApplicationFactory<Startup> factory)
            :base(factory)
        {
            var flightsClient = new FlightServiceClient(_flightsHttpClient);
            section = flightsClient.FlightSection;
        }

        [Fact]
        public async Task GetListAsync_Success_ListReturned()
        {
            //Act
            var result = await section.GetListAsync();

            //Assert
            Assert.IsType<List<Dto>>(result);
        }

        [Theory]
        [MemberData(nameof(ListOfFlightsForPassingTest))]
        public async Task CreateObjectAsync_Success_CreatedObjectReturned(Dto dto)
        {
            //Act
            var actualResult = await section.InsertAsync(dto);
            var resultId = actualResult.Id ?? default;
            await section.DeleteAsync(resultId); //Clear database
            dto.Id = actualResult.Id;

            var expectedResult = dto.ToExpectedObject();

            //Assert
            expectedResult.ShouldEqual(actualResult);
        }

        [Theory]
        [MemberData(nameof(ListOfFlightsForFailingTest))]
        public async Task CreateObjectAsync_Fail_BadRequestReturned(Dto dto)
        {
            //Act
            Func<Task<Dto>> act = () => section.InsertAsync(dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => { 
                var result = act.Invoke();
                if (result.Result.GetType().Equals(typeof(Dto)))
                {
                    var resultId = result.Result.Id ?? default;
                    section.DeleteAsync(resultId); //Clear database
                    return result;
                }
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.InnerException.Message);
        }

        [Theory]
        [MemberData(nameof(ListOfFlightsForPassingTest))]
        public async Task DeleteObjectAsync_Success_NotFoundReturned(Dto dto)
        {
            //Arrange
            var temporaryCreatedObject = await section.InsertAsync(dto);
            var resultId = temporaryCreatedObject.Id ?? default;

            //Act
            await section.DeleteAsync(resultId);
            Func<Task<Dto>> act = () => section.GetAsync(resultId);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            Assert.Equal("Response status code does not indicate success: 404 (Not Found).", ex.Message);
        }

        [Theory]
        [MemberData(nameof(ListOfFlightsForPassingTest))]
        public async Task UpdateObjectAsync_Success_UpdatedObjectReturned(Dto dto)
        {
            //Arrange
            var result = await section.InsertAsync(new Dto { Name = "TestFlightEx1", From = 1, To = 7, AirlineId=1, FlightTypeId=1, ReturnFlightSameAirlineRequired =true, AlternativeSearch=false, FlightDuration=1, AirplaneModelId=1, StatusId=1});
            var resultId = result.Id ?? default;
            Dto actualResult = null;

            //Act
            try
            {
                actualResult = await section.UpdateAsync(resultId, dto);
                await section.DeleteAsync(resultId); //Clear database
                dto.Id = actualResult.Id;
                dto.IsReturnFlight = false;
            }
            catch
            {
                await section.DeleteAsync(resultId); //Clear database
            }
            var expectedResult = dto.ToExpectedObject();

            //Assert
            expectedResult.ShouldEqual(actualResult);
        }

        [Theory]
        [MemberData(nameof(ListOfFlightsForFailingTest))]
        public async Task UpdateObjectAsync_Fail_BadRequestReturned(Dto dto)
        {
            //Arrange
            var newObjectId = (await section.InsertAsync(new Dto { Name = "TestFlightEx2", From = 1, To = 7, AirlineId = 1, FlightTypeId = 1, ReturnFlightSameAirlineRequired = true, AlternativeSearch = false, FlightDuration = 1, AirplaneModelId = 1, StatusId = 1 })).Id;
            var resultId = newObjectId ?? default;

            //Act
            Func<Task<Dto>> act = () => section.UpdateAsync(resultId, dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => {
                var result = act.Invoke();
                section.DeleteAsync(resultId); //Clear database
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.Message);
        }

        [Theory]
        [MemberData(nameof(PatchFlightsForPassingTest))]
        public async Task PatchObjectAsync_Success_PatchedObjectReturned(Dto dto, string property)
        {
            //Arrange
            var result = await section.InsertAsync(new Dto { Name = "TestFlightEx3", From = 1, To = 7, AirlineId = 1, FlightTypeId = 1, ReturnFlightSameAirlineRequired = true, AlternativeSearch = false, FlightDuration = 1, AirplaneModelId = 1, StatusId = 1 });
            var resultId = result.Id ?? default;
            Dto patchedObject = null;

            //Act
            try
            {
                patchedObject = await section.PatchRecordAsync(resultId, property, dto);
                await section.DeleteAsync(resultId); //Clear database
            }
            catch
            {
                await section.DeleteAsync(resultId); //Clear database
            }

            //Assert
            switch (property)
            {
                case nameof(Dto.AirplaneModelId):
                case nameof(Dto.FlightDuration):
                case nameof(Dto.TouristInfoEditLock):
                case nameof(Dto.AirlineId):
                    {
                        var actualResult = GetPropertyValue<int?>(patchedObject, property);
                        var expectedResult = GetPropertyValue<int?>(dto, property);
                        Assert.Equal(expectedResult, actualResult);
                        break;
                    }
                case nameof(Dto.From):
                case nameof(Dto.To):
                    {
                        var actualResult = GetPropertyValue<long?>(patchedObject, property);
                        var expectedResult = GetPropertyValue<long?>(dto, property);
                        Assert.Equal(expectedResult, actualResult);
                        break;
                    }
                case nameof(Dto.AlternativeSearch):
                case nameof(Dto.ReturnFlightSameAirlineRequired):
                    {
                        var actualResult = GetPropertyValue<bool?>(patchedObject, property);
                        var expectedResult = GetPropertyValue<bool?>(dto, property);
                        Assert.Equal(expectedResult, actualResult);
                        break;
                    }
                case nameof(Dto.FlightTypeId):
                case nameof(Dto.StatusId):
                    {
                        var actualResult = GetPropertyValue<short?>(patchedObject, property);
                        var expectedResult = GetPropertyValue<short?>(dto, property);
                        Assert.Equal(expectedResult, actualResult);
                        break;
                    }
                default:
                    {
                        var actualResult = GetPropertyValue<string>(patchedObject, property);
                        var expectedResult = GetPropertyValue<string>(dto, property);
                        Assert.Equal(expectedResult, actualResult);
                        break;
                    }
            }
        }

        [Theory]
        [MemberData(nameof(PatchFlightsForFailingTest))]
        public async Task PatchObjectAsync_Fail_BadRequestReturned(Dto dto, string property)
        {
            //Arrange
            var newObjectId = (await section.InsertAsync(new Dto { Name = "TestFlightEx4", From = 1, To = 7, AirlineId = 1, FlightTypeId = 1, ReturnFlightSameAirlineRequired = true, AlternativeSearch = false, FlightDuration = 1, AirplaneModelId = 1, StatusId = 1 })).Id;
            var resultId = newObjectId ?? default;

            //Act
            Func<Task<Dto>> act = () => section.PatchRecordAsync(resultId, property, dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => {
                var result = act.Invoke();
                section.DeleteAsync(resultId); //Clear database
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task SetStatusAsync_Success_UpdatedObjectReturned(short status)
        {
            //Arrange
            var result = await section.InsertAsync(new Dto { Name = "TestFlightEx5", From = 1, To = 7, AirlineId = 1, FlightTypeId = 1, ReturnFlightSameAirlineRequired = true, AlternativeSearch = false, FlightDuration = 1, AirplaneModelId = 1, StatusId = 3 });
            var resultId = result.Id ?? default;
            short? resultObject = 0;

            //Act
            try
            {
                await section.SetStatusAsync(resultId, status);
                resultObject = (await section.GetAsync(resultId)).StatusId;
                await section.DeleteAsync(resultId); //Clear database
            }
            catch
            {
                await section.DeleteAsync(resultId); //Clear database
            }

            //Assert
            Assert.Equal(resultObject, status);
        }

        [Fact]
        public async Task SearchFlightsAsync_Success_ListReturned()
        {
            //Act
            var result = await section.SearchFlightsAsync(
                new Common.Types.Pagination { Skip=0, Take=20, OrderDirection = Common.Types.OrderDirection.Asc, OrderBy="Id"}, 
                new Flights.Contract.FlightSearchFiltersDto { Statuses = { 1}, SearchByName="" });

            //Assert
            Assert.IsType<List<Dto>>(result);
        }

    }
}
