namespace Docker.Registry.DotNet.Models
{
    using System;
    using System.IO;

    public class GetBlobResponse : BlobHeader, IDisposable
    {
        internal GetBlobResponse(string dockerContentDigest, Stream stream) 
            : base(dockerContentDigest)
        {
            Stream = stream;
        }

        public Stream Stream { get; }

        public void Dispose()
        {
            Stream?.Dispose();    
        }
    }
}