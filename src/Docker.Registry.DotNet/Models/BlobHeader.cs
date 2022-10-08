using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Models
{
    [PublicAPI]
    public class BlobHeader
    {
        internal BlobHeader(string dockerContentDigest)
        {
            this.DockerContentDigest = dockerContentDigest;
        }

        public string DockerContentDigest { get; }
    }
}