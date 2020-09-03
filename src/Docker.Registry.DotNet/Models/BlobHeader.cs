namespace Docker.Registry.DotNet.Models
{
    public class BlobHeader
    {
        internal BlobHeader(string dockerContentDigest)
        {
            this.DockerContentDigest = dockerContentDigest;
        }

        public string DockerContentDigest { get; }
    }
}