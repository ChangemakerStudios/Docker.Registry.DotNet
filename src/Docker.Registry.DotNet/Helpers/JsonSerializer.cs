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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Helpers
{
    /// <summary>
    ///     Facade for <see cref="JsonConvert" />.
    /// </summary>
    internal class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                //new JsonIso8601AndUnixEpochDateConverter(),
                //new JsonVersionConverter(),
                new StringEnumConverter()
                //new TimeSpanSecondsConverter(),
                //new TimeSpanNanosecondsConverter()
            }
        };

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
    }
}