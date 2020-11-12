using Flights.Contract.Base;
using Flurl;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Flights.Contract.Client.Base
{
    /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}"/>
    internal abstract class BaseSection<TIdentifier, TSource> : IBaseSection<TIdentifier, TSource>
    {
        protected readonly HttpClient HttpClient;
        protected readonly string Endpoint;
        protected readonly JsonSerializerOptions Options;

        /// <summary/>
        protected BaseSection(HttpClient pHttpClient, string pEndpoint)
        {
            HttpClient = pHttpClient;
            Endpoint = pEndpoint;
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.GetAsync"/>
        public virtual async Task<TSource> GetAsync(TIdentifier id, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegment(id);
            using var response = await HttpClient.GetAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);         
            return JsonSerializer.Deserialize<TSource>(content, Options);
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.GetListAsync"/>
        public virtual async Task<IReadOnlyList<TSource>> GetListAsync(CancellationToken ct = default)
        {
            using var response = await HttpClient.GetAsync(Endpoint, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<IReadOnlyList<TSource>>(content, Options);        
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.InsertAsync"/>
        public virtual async Task<TSource> InsertAsync(TSource source, CancellationToken ct = default)
        {
            var body = new StringContent(JsonSerializer.Serialize(source, Options), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PostAsync(Endpoint, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TSource>(content, Options);
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.PatchRecordAsync(TIdentifier, string, TSource, CancellationToken)"/>
        public virtual async Task<TSource> PatchRecordAsync(TIdentifier id, string property, TSource model, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegments(id, property);
            var body = new StringContent(JsonSerializer.Serialize(model, Options), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PatchAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TSource>(content, Options);
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.UpdateAsync"/>
        public virtual async Task<TSource> UpdateAsync(TIdentifier id, TSource source, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegment(id);
            var body = new StringContent(JsonSerializer.Serialize(source, Options), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await HttpClient.PutAsync(requestUri, body, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TSource>(content, Options);
        }

        /// <inheritdoc cref="IBaseSection{TIdentifier,TSource}.DeleteAsync"/>
        public virtual async Task DeleteAsync(TIdentifier id, CancellationToken ct = default)
        {
            var requestUri = Endpoint.AppendPathSegment(id);
            using var response = await HttpClient.DeleteAsync(requestUri, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

    }
}
