using System;
using Docker.Registry.DotNet.Endpoints;

namespace Docker.Registry.DotNet
{
    /// <summary>
    /// The registry client.
    /// </summary>
    public interface IRegistryClient : IDisposable
    {
        /// <summary>
        /// Manifest operations
        /// </summary>
        IManifestOperations Manifest { get; }

        /// <summary>
        /// Catalog operations
        /// </summary>
        ICatalogOperations Catalog { get; }

        /// <summary>
        /// Blog operations
        /// </summary>
        IBlobOperations Blobs { get; }

        //IBlobUploadOperations BlobUploads { get; }

        /// <summary>
        /// Tag operations
        /// </summary>
        ITagOperations Tags { get; }

        /// <summary>
        /// System operations
        /// </summary>
        ISystemOperations System { get; }
    }
}