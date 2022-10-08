//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Runtime.Serialization;

namespace Docker.Registry.DotNet.Models
{
    [DataContract]
    public class Config
    {
        /// <summary>
        ///     The MIME type of the referenced object. This should generally be application/vnd.docker.image.rootfs.diff.tar.gzip.
        ///     Layers of type application/vnd.docker.image.rootfs.foreign.diff.tar.gzip may be pulled from a remote location but
        ///     they should never be pushed.
        /// </summary>
        [DataMember(Name = "mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        ///     The size in bytes of the object. This field exists so that a client will have an expected size for the content
        ///     before validating. If the length of the retrieved content does not match the specified length, the content should
        ///     not be trusted.
        /// </summary>
        [DataMember(Name = "size")]
        public long Size { get; set; }

        /// <summary>
        ///     The digest of the content, as defined by the Registry V2 HTTP API Specificiation.
        ///     https://docs.docker.com/registry/spec/api/#digest-parameter
        /// </summary>
        [DataMember(Name = "digest")]
        public string Digest { get; set; }

        /// <summary>
        ///     Provides a list of URLs from which the content may be fetched. Content should be verified against the digest and
        ///     size. This field is optional and uncommon.
        /// </summary>
        [DataMember(Name = "urls")]
        public string[] Urls { get; set; }
    }
}