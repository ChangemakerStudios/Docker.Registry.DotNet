using System;
using Docker.Registry.DotNet.Endpoints;

namespace Docker.Registry.DotNet
{
    public interface IRegistryClient : IDisposable
    {
        IManifestOperations Manifest { get; }

        ICatalogOperations Catalog { get; }

        //IBlobOperations Blobs { get; }

        //IBlobUploadOperations BlobUploads { get; }

        ITagOperations Tags { get; }

        ISystemOperations System { get; }
    }
}