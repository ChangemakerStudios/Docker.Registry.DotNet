using System.IO;

namespace Docker.Registry.DotNet.Models
{
    public class GetBlobResponse : BlobHeader
    {
        internal GetBlobResponse(int contentLength, string dockerContentDigest, Stream stream) 
            : base(contentLength, dockerContentDigest)
        {
            Stream = stream;
        }

        public Stream Stream { get; }
    }

    public class BlobHeader
    {

        internal BlobHeader(int contentLength, string dockerContentDigest)
        {
            ContentLength = contentLength;
            DockerContentDigest = dockerContentDigest;
        }

        public int ContentLength { get; }

        public string DockerContentDigest { get; }
    }
}