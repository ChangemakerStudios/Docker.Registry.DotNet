using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.DotNet.Endpoints
{
    internal class BlobUploadOperations : IBlobUploadOperations
    {
        private readonly NetworkClient _client;

        internal BlobUploadOperations(NetworkClient client)
        {
            _client = client;
        }

        public Task<InitiateMonolithicUploadResponse> UploadBlobAsync(string name, int contentLength, Stream stream, string digest,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<ResumableUploadResponse> InitiateBlobUploadAsync(string name, Stream stream = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<MountResponse> MountBlobAsync(string name, MountParameters parameters,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<BlobUploadStatus> GetBlobUploadStatus(string name, string uuid, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<ResumableUploadResponse> UploadBlobChunkAsync(string name, string uuid, Stream chunk,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task<ResumableUploadResponse> CompleteBlobUploadAsync(string name, string uuid, Stream chunk = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public Task CancelBlobUploadAsync(string name, string uuid, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}