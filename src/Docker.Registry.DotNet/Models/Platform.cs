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
    public class Platform
    {
        /// <summary>
        ///     The architecture field specifies the CPU architecture, for example amd64 or ppc64le
        /// </summary>
        [DataMember(Name = "architecture")]
        public string Architecture { get; set; }

        /// <summary>
        ///     The os field specifies the operating system, for example linux or windows.
        /// </summary>
        [DataMember(Name = "os")]
        public string Os { get; set; }

        /// <summary>
        ///     The optional os.version field specifies the operating system version, for example 10.0.10586.
        /// </summary>
        [DataMember(Name = "os.version")]
        public string OsVersion { get; set; }

        /// <summary>
        ///     The optional os.features field specifies an array of strings, each listing a required OS feature (for example on
        ///     Windows win32k)
        /// </summary>
        [DataMember(Name = "os.features")]
        public string OsFeatures { get; set; }

        /// <summary>
        ///     The optional variant field specifies a variant of the CPU, for example armv6l to specify a particular CPU variant
        ///     of the ARM CPU.
        /// </summary>
        [DataMember(Name = "variant")]
        public string Variant { get; set; }

        /// <summary>
        ///     The optional features field specifies an array of strings, each listing a required CPU feature (for example sse4 or
        ///     aes).
        /// </summary>
        [DataMember(Name = "features")]
        public string[] Features { get; set; }
    }
}