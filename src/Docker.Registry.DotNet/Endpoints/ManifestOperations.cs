using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    internal class ManifestOperations : IManifestOperations
    {

        private readonly NetworkClient _client;

        public ManifestOperations(NetworkClient client)
        {
            _client = client;
        }

        public async Task<GetImageManifestResult> GetManifestAsync(string name, string reference, CancellationToken cancellationToken = new CancellationToken())
        {
            var headers = new Dictionary<string, string>
            {
                { "Accept", $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"   }
            };

            var response = await _client.MakeRequestAsync(cancellationToken, HttpMethod.Get, $"v2/{name}/manifests/{reference}", null, headers).ConfigureAwait(false);

            string contentType = response.GetHeader("ContentType");

            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = ManifestMediaTypes.ManifestSchema1;
            }

            switch (contentType)
            {
                case ManifestMediaTypes.ManifestSchema1:
                case ManifestMediaTypes.ManifestSchema1Signed:
                    return new GetImageManifestResult(contentType,
                        _client.JsonSerializer.DeserializeObject<ImageManifest2_1>(response.Body))
                    {
                        DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
                        Etag = response.GetHeader("Etag")
                    };

                case ManifestMediaTypes.ManifestSchema2:
                    return new GetImageManifestResult(contentType, _client.JsonSerializer.DeserializeObject<ImageManifest2_2>(response.Body));

                case ManifestMediaTypes.ManifestList:
                    return new GetImageManifestResult(contentType, _client.JsonSerializer.DeserializeObject<ManifestList>(response.Body));

                default:
                    throw new Exception($"Unexpectd ContentType '{contentType}'.");
            }
        }

        public Task PutManifestAsync(string name, string reference, ImageManifest manifest,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteManifestAsync(string name, string reference,
            CancellationToken cannCancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}