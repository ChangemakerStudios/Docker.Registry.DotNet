using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    internal class CatalogOperations : ICatalogOperations
    {
        private readonly NetworkClient _client;

        public CatalogOperations(NetworkClient client)
        {
            _client = client;
        }

        public async Task<Catalog> GetCatalogAsync(CatalogParameters parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var queryParameters = new QueryString();

            queryParameters.AddIfNotNull("n", parameters.Number);
            queryParameters.AddIfNotNull("last", parameters.Last);

            var response = await _client.MakeRequestAsync(cancellationToken, HttpMethod.Get, "v2/_catalog", queryParameters).ConfigureAwait(false);

            return _client.JsonSerializer.DeserializeObject<Catalog>(response.Body);
        }
    }
}
