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

using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Endpoints;
using Docker.Registry.DotNet.Endpoints.Implementations;

namespace Docker.Registry.DotNet.Registry
{
    internal sealed class RegistryClient : IRegistryClient
    {
        private readonly NetworkClient _client;

        public RegistryClient(
            RegistryClientConfiguration configuration,
            AuthenticationProvider authenticationProvider)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (authenticationProvider == null)
                throw new ArgumentNullException(nameof(authenticationProvider));

            _client = new NetworkClient(configuration, authenticationProvider);

            this.Manifest = new ManifestOperations(_client);
            this.Catalog = new CatalogOperations(_client);
            this.Blobs = new BlobOperations(_client);
            this.BlobUploads = new BlobUploadOperations(_client);
            this.System = new SystemOperations(_client);
            this.Tags = new TagOperations(_client);
        }

        public IBlobUploadOperations BlobUploads { get; }

        public IManifestOperations Manifest { get; }

        public ICatalogOperations Catalog { get; }

        public IBlobOperations Blobs { get; }

        public ITagOperations Tags { get; }

        public ISystemOperations System { get; }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}