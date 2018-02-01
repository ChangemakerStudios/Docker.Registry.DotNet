using System.IO;

namespace Docker.Registry.DotNet.Models
{
    public class GetBlobResponse : BlobHeader
    {
        internal GetBlobResponse(string dockerContentDigest, Stream stream) 
            : base(dockerContentDigest)
        {
            Stream = stream;
        }

        public Stream Stream { get; }
    }

    public class BlobHeader
    {

        internal BlobHeader(string dockerContentDigest)
        {
            DockerContentDigest = dockerContentDigest;
        }

        public string DockerContentDigest { get; }
    }
}