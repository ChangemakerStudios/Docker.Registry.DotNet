using System;

using Docker.Registry.DotNet.Endpoints;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Registry
{
    /// <summary>
    ///     The registry client.
    /// </summary>
    public interface IRegistryClient : IDisposable
    {
        /// <summary>
        ///     Manifest operations
        /// </summary>
        [PublicAPI]
        IManifestOperations Manifest { get; }

        /// <summary>
        ///     Catalog operations
        /// </summary>
        [PublicAPI]
        ICatalogOperations Catalog { get; }

        /// <summary>
        ///     Blog operations
        /// </summary>
        [PublicAPI]
        IBlobOperations Blobs { get; }

        /// <summary>
        ///     Tag operations
        /// </summary>
        [PublicAPI]
        ITagOperations Tags { get; }

        /// <summary>
        ///     System operations
        /// </summary>
        [PublicAPI]
        ISystemOperations System { get; }
    }
}