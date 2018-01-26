using System;
using System.Threading;
using Docker.Registry.DotNet.Authentication;

namespace Docker.Registry.DotNet
{
    public class RegistryClientConfiguration
    {
        public Uri EndpointBaseUri { get; internal set; }

        public TimeSpan DefaultTimeout { get; internal set; } = TimeSpan.FromSeconds(100);

        public RegistryClientConfiguration(Uri endpoint, TimeSpan defaultTimeout = default(TimeSpan))
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            EndpointBaseUri = endpoint;

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
            return new RegistryClient(this, new AnonymousAuthenticationProvider());
        }

        public IRegistryClient CreateClient(AuthenticationProvider authenticationProvider)
        {
            return new RegistryClient(this, authenticationProvider);
        }
    }
}