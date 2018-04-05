namespace Docker.Registry.DotNet.Models
{
    public class BlobHeader
    {

        internal BlobHeader(string dockerContentDigest)
        {
            DockerContentDigest = dockerContentDigest;
        }

        public string DockerContentDigest { get; }
    }
}