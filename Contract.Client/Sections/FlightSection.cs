using Common.Types;
using Flights.Contract.Client.Base;
using Flights.Contract.Dto;
using Flights.Contract.Sections;
using Flurl;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Flights.Contract.Client.Sections
{
    internal sealed class FlightSection : BaseSection<long, FlightDto>, IFlightSection
    {
        public FlightSection(HttpClient httpClient)
            : base(httpClient, "api/flights")
        {
        }

        /// <inheritdoc cref="IFlightSection.DeleteReturnAsync(long, long, CancellationToken)"/>
        public async Task DeleteReturnAsync(long id, long returnFlightId, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns", returnFlightId);
            using var response = await HttpClient.DeleteAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IFlightSection.GetListReturnAsync(long, CancellationToken)"/>
        public async Task<IReadOnlyList<FlightDto>> GetListReturnAsync(long id, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns");
            using var response = await HttpClient.GetAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<FlightDto>>(content, Options);
        }

        /// <inheritdoc cref="IFlightSection.GetListReturnAsync(IReadOnlyList{long}, CancellationToken)"/>
        public async Task<IReadOnlyList<FlightDto>> GetListReturnAsync(IReadOnlyList<long> flightIds, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegment("returns").SetQueryParam(nameof(flightIds), flightIds);
            using var response = await HttpClient.GetAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<FlightDto>>(content, Options);
        }

        /// <inheritdoc cref="IFlightSection.GetReturnAsync(long, long, CancellationToken)"/>
        public async Task<FlightDto> GetReturnAsync(long id, long returnFlightId, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns", returnFlightId);
            using var response = await HttpClient.GetAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<FlightDto>(content, Options);
        }

        /// <inheritdoc cref="IFlightSection.PostReturnAsync(long, FlightDto, CancellationToken)"/>
        public async Task PostReturnAsync(long id, FlightDto dto, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns");
            var body = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PostAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IFlightSection.SearchFlightsAsync(Pagination, FlightSearchFiltersDto, CancellationToken)"/>
        public async Task<IReadOnlyList<FlightDto>> SearchFlightsAsync(Pagination pagination, FlightSearchFiltersDto filters, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegment("search").SetQueryParams(pagination);
            var body = new StringContent(JsonSerializer.Serialize(filters), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PostAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<FlightDto>>(content, Options);
        }

        /// <inheritdoc cref="IFlightSection.SetStatusAsync(long, short, CancellationToken)"/>
        public async Task SetStatusAsync(long id, short statusId, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "status");
            var body = new StringContent(JsonSerializer.Serialize(statusId), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PatchAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IFlightSection.SetNativeAsync(long, long, CancellationToken)"/>
        public async Task SetNativeAsync(long id, long returnFlightId, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns", "set-native");
            var body = new StringContent(JsonSerializer.Serialize(returnFlightId), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PatchAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IFlightSection.SetPriorityAsync(long, long, int, CancellationToken)"/>
        public async Task SetPriorityAsync(long id, long returnFlightId, int priority, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns", returnFlightId, "priority");
            var body = new StringContent(JsonSerializer.Serialize(priority), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PatchAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IFlightSection.SetStatusReturnAsync(long, long, bool, CancellationToken)"/>
        public async Task SetStatusReturnAsync(long id, long returnFlightId, bool newStatus, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, "returns", returnFlightId, "status");
            var body = new StringContent(JsonSerializer.Serialize(newStatus), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PatchAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
