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

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class CatalogOperations : ICatalogOperations
    {
        private readonly NetworkClient _client;

        public CatalogOperations([NotNull] NetworkClient client)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Catalog> GetCatalogAsync(
            CatalogParameters parameters = null,
            CancellationToken cancellationToken = default)
        {
            parameters = parameters ?? new CatalogParameters();

            var queryParameters = new QueryString();

            queryParameters.AddFromObjectWithQueryParameters(parameters);

            var response = await this._client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Get,
                "v2/_catalog",
                queryParameters).ConfigureAwait(false);

            return this._client.JsonSerializer.DeserializeObject<Catalog>(response.Body);
        }
    }
}