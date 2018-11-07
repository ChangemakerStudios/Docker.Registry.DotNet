using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    /// <summary>
    /// Manifest operations.
    /// </summary>
    public interface IManifestOperations
    {
        ///// <summary>
        ///// Fetch the manifest identified by name and reference where reference can be a tag or digest. A HEAD request can also be issued to this endpoint to obtain resource information without receiving all data.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="reference"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        Task<GetImageManifestResult> GetManifestAsync(string name, string reference, CancellationToken cancellationToken = new CancellationToken());

        ///// <summary>
        ///// Returns true if the image exists, false otherwise.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="reference"></param>
        ///// <param name="cancellation"></param>
        ///// <returns></returns>
        //Task<bool> DoesManifestExistAsync(string name, string reference, CancellationToken cancellation = new CancellationToken());

        ///// <summary>
        ///// Put the manifest identified by name and reference where reference can be a tag or digest.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="reference"></param>
        ///// <param name="manifest"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //Task PutManifestAsync(string name, string reference, ImageManifest manifest, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Delete the manifest identified by name and reference. Note that a manifest can only be deleted by digest.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reference"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteManifestAsync(string name, string reference, CancellationToken cancellationToken = new CancellationToken());
    }
}