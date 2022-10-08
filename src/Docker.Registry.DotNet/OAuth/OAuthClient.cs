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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;

using Newtonsoft.Json;

namespace Docker.Registry.DotNet.OAuth
{
    internal class OAuthClient
    {
        private readonly HttpClient _client = new HttpClient();

        private async Task<OAuthToken> GetTokenInnerAsync(
            string realm,
            string service,
            string scope,
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request;

            if (username == null || password == null)
            {
                var queryString = new QueryString();

                queryString.AddIfNotEmpty("service", service);
                queryString.AddIfNotEmpty("scope", scope);

                var builder = new UriBuilder(new Uri(realm))
                {
                    Query = queryString.GetQueryString()
                };

                request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, realm)
                {
                    Content = new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            { "client_id", "Docker.Registry.DotNet" },
                            { "grant_type", "password" },
                            { "username", username },
                            { "password", password },
                            { "service", service },
                            { "scope", scope },
                        }
                    ),
                };
            }

            using (var response = await this._client.SendAsync(request, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                    throw new UnauthorizedAccessException("Unable to authenticate.");

                var body = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<OAuthToken>(body);

                return token;
            }
        }

        public Task<OAuthToken> GetTokenAsync(
            string realm,
            string service,
            string scope,
            CancellationToken cancellationToken = default)
        {
            return this.GetTokenInnerAsync(realm, service, scope, null, null, cancellationToken);
        }

        public Task<OAuthToken> GetTokenAsync(
            string realm,
            string service,
            string scope,
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            return this.GetTokenInnerAsync(
                realm,
                service,
                scope,
                username,
                password,
                cancellationToken);
        }
    }
}