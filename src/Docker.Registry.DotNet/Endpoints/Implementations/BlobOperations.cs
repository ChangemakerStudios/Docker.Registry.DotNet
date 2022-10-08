//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Net;
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
        {
            var url = $"v2/{name}/blobs/{digest}";

            return this._client.MakeRequestAsync(cancellationToken, HttpMethod.Delete, url);
        }

        public async Task<bool> IsExistBlobAsync(
            string name,
            string digest,
            CancellationToken cancellationToken = default)
        {
            var path = $"v2/{name}/blobs/{digest}";

            var response = await this._client.MakeRequestNotErrorAsync(
                cancellationToken,
                HttpMethod.Head,
                path);

            if (response.StatusCode != HttpStatusCode.NotFound)
                this._client.HandleIfErrorResponse(response);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}