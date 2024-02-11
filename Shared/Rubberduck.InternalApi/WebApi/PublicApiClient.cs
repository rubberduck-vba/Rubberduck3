using Rubberduck.InternalApi.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.WebApi;

public class PublicApiClient : ApiClientBase, IPublicApiClient
{
    private static readonly string FeatureEndpoint = "Feature";
    private static readonly string FeatureItemEndpoint = "FeatureItem";
    private static readonly string TagsEndpoint = "Tags";
    private static readonly string TelemetryEndpoint = "Telemetry";

    public PublicApiClient(HttpClient client, Version version, Uri baseUri) 
        : base(client, version, baseUri)
    {
    }

    public async Task<IEnumerable<Tag>> GetLatestTagsAsync() => await GetResponseAsync<Tag[]>(TagsEndpoint)
        ?? Enumerable.Empty<Tag>();

    public async Task<Feature> GetFeatureAsync(string name) => await GetResponseAsync<Feature>($"{FeatureEndpoint}/{name}")
        ?? throw new ArgumentOutOfRangeException(nameof(name));

    public async Task<FeatureItem> GetFeatureItemAsync(string name) => await GetResponseAsync<FeatureItem>($"{FeatureItemEndpoint}/{name}")
        ?? throw new ArgumentOutOfRangeException(nameof(name));

    public async Task TransmitTelemetryAsync(object payload) => await PostAsync($"{TelemetryEndpoint}", payload);
}
