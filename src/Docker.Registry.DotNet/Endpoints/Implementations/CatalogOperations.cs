using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class CatalogOperations : ICatalogOperations
    {
        private readonly NetworkClient _client;

        public CatalogOperations(NetworkClient client)
        {
            this._client = client;
        }

        public async Task<Catalog> GetCatalogAsync(
            CatalogParameters parameters = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            parameters = parameters ?? new CatalogParameters();

            var queryParameters = new QueryString();

            queryParameters.AddIfNotNull("n", parameters.Number);
            queryParameters.AddIfNotNull("last", parameters.Last);

            var response = await this._client.MakeRequestAsync(
                               cancellationToken,
                               HttpMethod.Get,
                               "v2/_catalog",
                               queryParameters).ConfigureAwait(false);

            return this._client.JsonSerializer.DeserializeObject<Catalog>(response.Body);
        }
    }
}