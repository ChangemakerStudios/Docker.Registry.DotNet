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
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;

namespace Docker.Registry.DotNet.Authentication
{
    /// <summary>
    /// Authentication provider.
    /// </summary>
    public abstract class AuthenticationProvider
    {
        /// <summary>
        /// Called on the initial send
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task AuthenticateAsync(HttpRequestMessage request);

        /// <summary>
        /// Called when the send is challenged.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public abstract Task AuthenticateAsync(
            HttpRequestMessage request,
            HttpResponseMessage response);

        /// <summary>
        /// Gets the schema header from the http response.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        protected AuthenticationHeaderValue TryGetSchemaHeader(
            HttpResponseMessage response,
            string schema)
        {
            var header = response.GetHeaderBySchema(schema);

            if (header == null)
            {
                throw new InvalidOperationException(
                    $"No WWW-Authenticate challenge was found for schema {schema}");
            }

            return header;
        }
    }
}