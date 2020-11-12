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
using Dto = Flights.Contract.Dto.AirlineDto;

namespace Contract.Client.Tests
{
    public sealed partial class AirlineSectionTest : Configs
    {
        private readonly IAirlineSection section;

        public AirlineSectionTest(WebApplicationFactory<Startup> factory)
            :base(factory)
        {
            var flightsClient = new FlightServiceClient(_flightsHttpClient);
            section = flightsClient.AirlineSection;
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
        [MemberData(nameof(ListOfAirlinesForPassingTest))]
        public async Task CreateObjectAsync_Success_CreatedObjectReturned(Dto dto)
        {
            //Act
            var actualResult = await section.InsertAsync(dto);
            await section.DeleteAsync(actualResult.Id); //Clear database
            dto.Id = actualResult.Id;

            var expectedResult = dto.ToExpectedObject();

            //Assert
            expectedResult.ShouldEqual(actualResult);
        }

        [Theory]
        [MemberData(nameof(ListOfAirlinesForFailingTest))]
        public async Task CreateObjectAsync_Fail_BadRequestReturned(Dto dto)
        {
            //Act
            Func<Task<Dto>> act = () => section.InsertAsync(dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => { 
                var result = act.Invoke();
                if (result.Result.GetType().Equals(typeof(Dto)))
                {
                    section.DeleteAsync(result.Result.Id); //Clear database
                    return result;
                }
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.InnerException.Message);
        }

        [Theory]
        [MemberData(nameof(ListOfAirlinesForPassingTest))]
        public async Task DeleteObjectAsync_Success_NotFoundReturned(Dto dto)
        {
            //Arrange
            var temporaryCreatedObject = await section.InsertAsync(dto);

            //Act
            await section.DeleteAsync(temporaryCreatedObject.Id);
            Func<Task<Dto>> act = () => section.GetAsync(temporaryCreatedObject.Id);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            Assert.Equal("Response status code does not indicate success: 404 (Not Found).", ex.Message);
        }

        [Theory]
        [MemberData(nameof(ListOfAirlinesForPassingTest))]
        public async Task UpdateObjectAsync_Success_UpdatedObjectReturned(Dto dto)
        {
            //Arrange
            var result = await section.InsertAsync(new Dto { Name = "TestAirlineEx1", Iata = "TSA1"});

            //Act
            var actualResult = await section.UpdateAsync(result.Id, dto);
            await section.DeleteAsync(actualResult.Id); //Clear database
            dto.Id = actualResult.Id;

            var expectedResult = dto.ToExpectedObject();

            //Assert
            expectedResult.ShouldEqual(actualResult);
        }

        [Theory]
        [MemberData(nameof(ListOfAirlinesForFailingTest))]
        public async Task UpdateObjectAsync_Fail_BadRequestReturned(Dto dto)
        {
            //Arrange
            var newObjectId = (await section.InsertAsync(new Dto { Name = "TestAirlineEx2", Iata = "TSA2" })).Id;

            //Act
            Func<Task<Dto>> act = () => section.UpdateAsync(newObjectId, dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => {
                var result = act.Invoke();
                section.DeleteAsync(newObjectId); //Clear database
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.Message);
        }

        [Theory]
        [MemberData(nameof(PatchAirlinesForPassingTest))]
        public async Task PatchObjectAsync_Success_PatchedObjectReturned(Dto dto, string property)
        {
            //Arrange
            var result = await section.InsertAsync(new Dto { Name = "TestAirlineEx3", Iata = "TSA3" });

            //Act
            var patchedObject = await section.PatchRecordAsync(result.Id, property, dto);
            await section.DeleteAsync(result.Id); //Clear database
            var actualResult = GetPropertyValue<string>(patchedObject, property);
            var expectedResult = GetPropertyValue<string>(dto, property);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [MemberData(nameof(PatchAirlinesForFailingTest))]
        public async Task PatchObjectAsync_Fail_BadRequestReturned(Dto dto, string property)
        {
            //Arrange
            var newObjectId = (await section.InsertAsync(new Dto { Name = "TestAirlineEx4", Iata = "TSA4" })).Id;

            //Act
            Func<Task<Dto>> act = () => section.PatchRecordAsync(newObjectId, property, dto);

            //Assert
            var ex = await Assert.ThrowsAnyAsync<Exception>(() => {
                var result = act.Invoke();
                section.DeleteAsync(newObjectId); //Clear database
                return result;
            });
            Assert.Equal("Response status code does not indicate success: 400 (Bad Request).", ex.Message);
        }

    }
}
