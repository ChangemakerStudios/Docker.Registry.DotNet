using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class ManifestHistory
    {
        [DataMember(Name = "v1Compatibility")]
        public string V1Compatibility { get; set; }
    }
}