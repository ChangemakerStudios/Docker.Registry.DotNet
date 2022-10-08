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
    public class MountResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Identifies the docker upload uuid for the current request.
        /// </summary>
        public string DockerUploadUuid { get; set; }

        /// <summary>
        /// If the blob is successfully mounted Created is true,Otherwise, it is flse
        /// If a mount fails due to invalid repository or digest arguments, the registry will fall back to the standard upload behavior And with the upload URL in the <see cref="Location"/>
        /// </summary>
        public bool Created { get; set; }
    }
}