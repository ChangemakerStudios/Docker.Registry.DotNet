using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class MountParameters
    {
        /// <summary>
        /// Digest of blob to mount from the source repository.
        /// </summary>
        //[QueryStringParameter("mount", true)]
        public string Digest { get; set; }

        /// <summary>
        /// Name of the source repository.
        /// </summary>
        //[QueryStringParameter("from", true)]
        public string From { get; set; }
    }
}