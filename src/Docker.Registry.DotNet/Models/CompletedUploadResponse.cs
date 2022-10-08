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

namespace Docker.Registry.DotNet.Models
{
    /// <summary>
    /// A completed upload response.
    /// </summary>
    public class CompletedUploadResponse
    {
        /// <summary>
        /// The Location will contain the registry URL to access the accepted layer file
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The DockerContentDigest returns the canonical digest of the uploaded blob which may differ from the provided digest. Most clients may ignore the value but if it is used, the client should verify the value against the uploaded blob data.
        /// </summary>
        public string DockerContentDigest { get; set; }
    }
}