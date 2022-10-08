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
    public class ImageManifest2_2 : ImageManifest
    {
        /// <summary>
        /// The MIME type of the referenced object
        /// </summary>
        [DataMember(Name = "mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The config field references a configuration object for a container, by digest. This configuration 
        /// item is a JSON blob that the runtime uses to set up the container. This new schema uses a tweaked 
        /// version of this configuration to allow image content-addressability on the daemon side.
        /// </summary>

        [DataMember(Name = "config")]
        public Config Config { get; set; }

        /// <summary>
        /// The layer list is ordered starting from the base image (opposite order of schema1).
        /// </summary>
        [DataMember(Name = "layers")]
        public ManifestLayer[] Layers { get; set; }
    }
}