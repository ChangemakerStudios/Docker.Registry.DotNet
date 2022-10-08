﻿//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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
    public class PushManifestResponse
    {
        /// <summary>
        /// The canonical location url of the uploaded manifest.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The Content-Length header must be zero and the body must be empty.
        /// </summary>
        public string ContentLength { get; set; }

        /// <summary>
        /// Digest of the targeted content for the request.
        /// </summary>
        public string DockerContentDigest { get; set; }
    }
}