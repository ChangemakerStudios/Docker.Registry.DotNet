using System;
using System.Threading;
using Docker.Registry.DotNet.Authentication;

namespace Docker.Registry.DotNet
{
    public class RegistryClientConfiguration
    {
        public Uri EndpointBaseUri { get; }

        public string Host { get; }

        public TimeSpan DefaultTimeout { get; internal set; } = TimeSpan.FromSeconds(100);

        /// <summary>
        /// Creates an instance of the RegistryClientConfiguration.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="defaultTimeout"></param>
        public RegistryClientConfiguration(string host, TimeSpan defaultTimeout = default(TimeSpan)) 
            : this(defaultTimeout)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(host));

            Host = host;
        }

        /// <summary>
        /// Obsolete constructor that allows a uri to be used to specify a registry.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="defaultTimeout"></param>
        [Obsolete("Use the constructor that allows you to specify a host.")]
        public RegistryClientConfiguration(Uri endpoint, TimeSpan defaultTimeout = default(TimeSpan)) 
            : this(defaultTimeout)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            EndpointBaseUri = endpoint;
        }

        private RegistryClientConfiguration(TimeSpan defaultTimeout)
        {
            if (defaultTimeout != TimeSpan.Zero)
            {
                if (defaultTimeout < Timeout.InfiniteTimeSpan)
                    // TODO: Should be a resource for localization.
                    // TODO: Is this a good message?
                    throw new ArgumentException("Timeout must be greater than Timeout.Infinite", nameof(defaultTimeout));
                DefaultTimeout = defaultTimeout;
            }
        }

        public IRegistryClient CreateClient()
        {
            return new RegistryClient(this, new AnonymousOAuthAuthenticationProvider());
        }

        public IRegistryClient CreateClient(AuthenticationProvider authenticationProvider)
        {
            return new RegistryClient(this, authenticationProvider);
        }
    }
}