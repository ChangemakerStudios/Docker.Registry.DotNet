using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class BlobOperations : IBlobOperations
    {
        private readonly NetworkClient _client;

        public BlobOperations(NetworkClient client)
        {
            this._client = client;
        }

        public async Task<GetBlobResponse> GetBlobAsync(
            string name,
            string digest,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var url = $"v2/{name}/blobs/{digest}";

            var response = await this._client.MakeRequestForStreamedResponseAsync(
                               cancellationToken,
                               HttpMethod.Get,
                               url);

            return new GetBlobResponse(
                response.Headers.GetString("Docker-Content-Digest"),
                response.Body);
        }

        public Task DeleteBlobAsync(
            string name,
            string digest,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var url = $"v2/{name}/blobs/{digest}";

            return this._client.MakeRequestAsync(cancellationToken, HttpMethod.Delete, url);
        }
    }
}