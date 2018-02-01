using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    public interface IBlobUploadOperations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contentLength"></param>
        /// <param name="stream"></param>
        /// <param name="digest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UploadBlobAsync(string name, int contentLength, Stream stream, string digest, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Initiate a resumable blob upload. If successful, an upload location will be provided to complete the upload. Optionally, if the digest parameter is present, the request body will be used to complete the upload in a single request.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResumableUploadResponse> InitiateBlobUploadAsync(string name, Stream stream = null, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Mount a blob identified by the mount parameter from another repository.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MountResponse> MountBlobAsync(string name, MountParameters parameters, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Retrieve status of upload identified by uuid. The primary purpose of this endpoint is to resolve the current status of a resumable upload.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BlobUploadStatus> GetBlobUploadStatus(string name, string uuid, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Upload a chunk of data for the specified upload.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <param name="chunk"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResumableUploadResponse> UploadBlobChunkAsync(string name, string uuid, Stream chunk, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Complete the upload specified by uuid, optionally appending the body as the final chunk.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <param name="chunk"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResumableUploadResponse> CompleteBlobUploadAsync(string name, string uuid, Stream chunk = null, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Cancel outstanding upload processes, releasing associated resources. If this is not called, the unfinished uploads will eventually timeout.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CancelBlobUploadAsync(string name, string uuid, CancellationToken cancellationToken = new CancellationToken());
    }
}