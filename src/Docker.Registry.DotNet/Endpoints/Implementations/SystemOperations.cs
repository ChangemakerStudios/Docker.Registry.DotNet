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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class SystemOperations : ISystemOperations
    {
        private readonly NetworkClient _client;

        public SystemOperations(NetworkClient client)
        {
            this._client = client;
        }

        public Task PingAsync(CancellationToken cancellationToken = default)
        {
            return this._client.MakeRequestAsync(cancellationToken, HttpMethod.Get, "v2/");
        }
    }
}