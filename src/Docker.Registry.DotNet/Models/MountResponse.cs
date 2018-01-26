namespace Docker.Registry.DotNet.Models
{
    public class MountResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Identifies the docker upload uuid for the current request.
        /// </summary>
        public string DockerUploadUuid { get; set; }
    }
}