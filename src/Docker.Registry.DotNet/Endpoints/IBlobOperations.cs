using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface IBlobOperations
    {
        /// <summary>
        /// Retrieve the blob from the registry identified by digest. Performs a monolithic download of the blob.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="digest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GetBlobResponse> GetBlobAsync(string name, string digest, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Delete the blob identified by name and digest.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="digest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteBlobAsync(string name, string digest, CancellationToken cancellationToken = new CancellationToken());
    }
}