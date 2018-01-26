using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Endpoints;

namespace Docker.Registry.DotNet
{
    public interface IRegistryClient : IDisposable
    {
        IManifestOperations Manifests { get; }

        ICatalogOperations Catalog { get; }

        IBlobOperations Blobs { get; }

        IBlobUploadOperations BlobUploads { get; }

        ITagOperations Tags { get; }

        Task PingAsync(CancellationToken cancellationToken = new CancellationToken());

    }
}