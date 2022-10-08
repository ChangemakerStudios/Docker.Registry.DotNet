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

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Authentication
{
    [PublicAPI]
    public class BasicAuthenticationProvider : AuthenticationProvider
    {
        private readonly string _password;

        private readonly string _username;

        public BasicAuthenticationProvider(string username, string password)
        {
            this._username = username;
            this._password = password;
        }

        private static string Schema { get; } = "Basic";

        public override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        public override Task AuthenticateAsync(
            HttpRequestMessage request,
            HttpResponseMessage response)
        {
            this.TryGetSchemaHeader(response, Schema);

            var passBytes = Encoding.UTF8.GetBytes($"{this._username}:{this._password}");
            var base64Pass = Convert.ToBase64String(passBytes);

            //Set the header
            request.Headers.Authorization =
                new AuthenticationHeaderValue(Schema, base64Pass);

            return Task.CompletedTask;
        }
    }
}