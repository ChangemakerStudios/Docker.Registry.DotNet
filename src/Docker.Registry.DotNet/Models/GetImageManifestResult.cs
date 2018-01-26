namespace Docker.Registry.DotNet.Models
{
    public class GetImageManifestResult
    {
        internal GetImageManifestResult(string mediaType, ImageManifest manifest) 
        {
            Manifest = manifest;
            MediaType = mediaType;
        }

        public string DockerContentDigest { get; internal set; }

        public string Etag { get; internal set; }

        public string MediaType { get; }

        /// <summary>
        /// The image manifest
        /// </summary>
        public ImageManifest Manifest { get; }
    }
}