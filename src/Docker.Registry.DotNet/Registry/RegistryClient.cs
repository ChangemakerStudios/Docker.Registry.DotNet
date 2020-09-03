using System;

using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Endpoints;

namespace Docker.Registry.DotNet.Registry
{
    internal sealed class RegistryClient : IRegistryClient
    {
        public RegistryClient(
            RegistryClientConfiguration configuration,
            AuthenticationProvider authenticationProvider)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (authenticationProvider == null)
                throw new ArgumentNullException(nameof(authenticationProvider));

            var client = new NetworkClient(configuration, authenticationProvider);

            this.Manifest = new ManifestOperations(client);
            this.Catalog = new CatalogOperations(client);
            this.Blobs = new BlobOperations(client);
            this.BlobUploads = new BlobUploadOperations(client);
            this.System = new SystemOperations(client);
            this.Tags = new TagOperations(client);
        }

        public IBlobUploadOperations BlobUploads { get; }

        public IManifestOperations Manifest { get; }

        public ICatalogOperations Catalog { get; }

        public IBlobOperations Blobs { get; }

        public ITagOperations Tags { get; }

        public ISystemOperations System { get; }

        public void Dispose()
        {
        }
    }
}