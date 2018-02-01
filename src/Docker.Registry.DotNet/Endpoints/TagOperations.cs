using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    internal class TagOperations : ITagOperations
    {
        private readonly NetworkClient _client;

        public TagOperations(NetworkClient client)
        {
            _client = client;
        }

        public async Task<ListImageTagsResponse> ListImageTagsAsync(string name, ListImageTagsParameters parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            QueryString queryString = new QueryString();

            queryString.AddIfNotNull("n", parameters.Number);

            var response = await _client.MakeRequestAsync(cancellationToken, HttpMethod.Get, $"v2/{name}/tags/list", queryString).ConfigureAwait(false);
            return _client.JsonSerializer.DeserializeObject<ListImageTagsResponse>(response.Body);
        }
    }
}