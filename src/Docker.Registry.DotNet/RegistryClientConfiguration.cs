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
using System.Threading;

using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Registry;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet
{
    public class RegistryClientConfiguration
    {
        /// <summary>
        ///     Creates an instance of the RegistryClientConfiguration.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="defaultTimeout"></param>
        public RegistryClientConfiguration(string host, TimeSpan defaultTimeout = default)
            : this(defaultTimeout)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));

            this.Host = host;
        }

        /// <summary>
        ///     Creates an instance of the RegistryClientConfiguration.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="httpMessageHandler"></param>
        /// <param name="defaultTimeout"></param>
        public RegistryClientConfiguration(
            string host,
            HttpMessageHandler httpMessageHandler,
            TimeSpan defaultTimeout = default)
            : this(defaultTimeout)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));

            this.Host = host;
            this.HttpMessageHandler = httpMessageHandler;
        }

        /// <summary>
        ///     Obsolete constructor that allows a uri to be used to specify a registry.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="defaultTimeout"></param>
        [Obsolete("Use the constructor that allows you to specify a host.")]
        public RegistryClientConfiguration(Uri endpoint, TimeSpan defaultTimeout = default)
            : this(defaultTimeout)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            this.EndpointBaseUri = endpoint;
        }

        private RegistryClientConfiguration(TimeSpan defaultTimeout)
        {
            if (defaultTimeout != TimeSpan.Zero)
            {
                if (defaultTimeout < Timeout.InfiniteTimeSpan)
                    // TODO: Should be a resource for localization.
                    // TODO: Is this a good message?
                    throw new ArgumentException(
                        "Timeout must be greater than Timeout.Infinite",
                        nameof(defaultTimeout));
                this.DefaultTimeout = defaultTimeout;
            }
        }

        public Uri EndpointBaseUri { get; }

        public string Host { get; }

        public HttpMessageHandler HttpMessageHandler { get; }

        public TimeSpan DefaultTimeout { get; internal set; } = TimeSpan.FromSeconds(100);

        [PublicAPI]
        public IRegistryClient CreateClient()
        {
            return new RegistryClient(this, new AnonymousOAuthAuthenticationProvider());
        }

        [PublicAPI]
        public IRegistryClient CreateClient(AuthenticationProvider authenticationProvider)
        {
            return new RegistryClient(this, authenticationProvider);
        }
    }
}