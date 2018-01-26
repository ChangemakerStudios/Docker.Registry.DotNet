using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class ManifestSignature
    {
        [DataMember(Name = "header")]
        public ManifestSignatureHeader Header { get; set; }

        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        [DataMember(Name = "protected")]
        public string Protected { get; set; }
    }
}