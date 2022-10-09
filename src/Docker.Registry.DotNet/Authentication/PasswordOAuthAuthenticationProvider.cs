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
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Docker.Registry.DotNet.OAuth;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Authentication
{
    [PublicAPI]
    public class PasswordOAuthAuthenticationProvider : AuthenticationProvider
    {
        private readonly OAuthClient _client = new OAuthClient();

        private readonly string _password;

        private readonly string _username;

        public PasswordOAuthAuthenticationProvider(string username, string password)
        {
            this._username = username;
            this._password = password;
        }

        private static string Schema { get; } = "Bearer";

        public override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        public override async Task AuthenticateAsync(
            HttpRequestMessage request,
            HttpResponseMessage response)
        {
            var header = this.TryGetSchemaHeader(response, Schema);

            //Get the bearer bits
            var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

            string scope = null;

            if (!string.IsNullOrWhiteSpace(bearerBits.Scope))
            {
                //Also include the repository(plugin) resource type to be able to access plugin repositories.
                //See https://docs.docker.com/registry/spec/auth/scope/
                scope = $"{bearerBits.Scope} {bearerBits.Scope?.Replace("repository:", "repository(plugin):")}";
            }

            //Get the token
            var token = await this._client.GetTokenAsync(
                bearerBits.Realm,
                bearerBits.Service,
                scope,
                this._username,
                this._password);

            //Set the header
            request.Headers.Authorization =
                new AuthenticationHeaderValue(Schema, token.AccessToken);
        }
    }
}