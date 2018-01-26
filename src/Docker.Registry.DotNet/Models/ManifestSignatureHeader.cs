using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class ManifestSignatureHeader
    {
        [DataMember(Name = "alg")]
        public string Alg { get; set; }
    }
}