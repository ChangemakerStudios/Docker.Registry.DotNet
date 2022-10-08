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
    /// <summary>
    /// Image Manifest Version 2, Schema 1
    /// </summary>
    [DataContract]
    public class ImageManifest2_1 : ImageManifest
    {
        /// <summary>
        /// name is the name of the image’s repository
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// tag is the tag of the image
        /// </summary>
        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        /// <summary>
        /// architecture is the host architecture on which this image is intended to run. This is for information purposes and not currently used by the engine
        /// </summary>
        [DataMember(Name = "architecture")]
        public string Architecture { get; set; }

        /// <summary>
        /// fsLayers is a list of filesystem layer blob sums contained in this image.
        /// </summary>
        [DataMember(Name = "fsLayers")]
        public ManifestFsLayer[] FsLayers { get; set; }

        /// <summary>
        /// history is a list of unstructured historical data for v1 compatibility. It contains ID of the image layer and ID of the layer’s parent layers.
        /// </summary>
        [DataMember(Name = "history")]
        public ManifestHistory[] History { get; set; }

        /// <summary>
        /// Signed manifests provides an envelope for a signed image manifest. A signed manifest consists of an image manifest along with an additional field containing the signature of the manifest.
        /// The docker client can verify signed manifests and displays a message to the user.
        /// </summary>
        [DataMember(Name = "signatures")]
        public ManifestSignature[] Signatures { get; set; }
    }
}