using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class Catalog
    {
        [DataMember(Name = "repositories", EmitDefaultValue = false)]
        public string[] Repositories { get; set; }
    }

}