using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class TagOperations : ITagOperations
    {
        private readonly NetworkClient _client;

        public TagOperations(NetworkClient client)
        {
            this._client = client;
        }

        public async Task<ListImageTagsResponse> ListImageTagsAsync(
            string name,
            ListImageTagsParameters parameters = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            parameters = parameters ?? new ListImageTagsParameters();

            var queryString = new QueryString();

            queryString.AddIfNotNull("n", parameters.Number);

            var response = await this._client.MakeRequestAsync(
                               cancellationToken,
                               HttpMethod.Get,
                               $"v2/{name}/tags/list",
                               queryString).ConfigureAwait(false);

            return this._client.JsonSerializer.DeserializeObject<ListImageTagsResponse>(
                response.Body);
        }
    }
}