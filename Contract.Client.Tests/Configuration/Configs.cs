using Flights;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Xunit;

namespace Contract.Client.Tests.Configuration
{
    public class Configs : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly HttpClient _flightsHttpClient;
        protected readonly ClientCredentialsTokenRequest _tokenRequest;

        public Configs(WebApplicationFactory<Startup> factory)
        {
            var configuration = factory.Services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("SecurityConfig");

            _flightsHttpClient = factory.CreateClient();

            _tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = _flightsHttpClient.BaseAddress.ToString() + "connect/token",
                ClientId = configuration["ClientId"],
                ClientSecret = configuration["ClientSecret"],
                Scope = "flight_api"
            };

            var response = _flightsHttpClient.RequestClientCredentialsTokenAsync(_tokenRequest).Result;
            _flightsHttpClient.SetBearerToken(response.AccessToken);
        }

    }
}
