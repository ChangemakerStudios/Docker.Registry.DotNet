using System;
using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.OAuth
{
    [DataContract]
    internal class OAuthToken
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "issued_at")]
        public DateTime IssuedAt { get; set; }
    }
}