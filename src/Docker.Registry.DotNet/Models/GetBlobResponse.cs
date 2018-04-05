namespace Docker.Registry.DotNet.Models
{
    using System.IO;

    public class GetBlobResponse : BlobHeader
    {
        internal GetBlobResponse(string dockerContentDigest, Stream stream) 
            : base(dockerContentDigest)
        {
            Stream = stream;
        }

        public Stream Stream { get; }
    }
}