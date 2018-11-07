using System;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Endpoints;

namespace Docker.Registry.DotNet
{
    internal sealed class RegistryClient : IRegistryClient
    {
        private readonly NetworkClient _client;

        public RegistryClient(RegistryClientConfiguration configuration, AuthenticationProvider authenticationProvider) 
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (authenticationProvider == null) throw new ArgumentNullException(nameof(authenticationProvider));

            _client = new NetworkClient(configuration, authenticationProvider);

            Manifest = new ManifestOperations(_client);
            Catalog = new CatalogOperations(_client);
            Blobs = new BlobOperations(_client);
            BlobUploads = new BlobUploadOperations(_client);
            System = new SystemOperations(_client);
            Tags = new TagOperations(_client);
        }

        public IManifestOperations Manifest { get; }

        public ICatalogOperations Catalog { get; }

        public IBlobOperations Blobs { get; }

        public IBlobUploadOperations BlobUploads { get; }

        public ITagOperations Tags { get; }

        public ISystemOperations System { get; }

        public void Dispose()
        {
            
        }
    }
}