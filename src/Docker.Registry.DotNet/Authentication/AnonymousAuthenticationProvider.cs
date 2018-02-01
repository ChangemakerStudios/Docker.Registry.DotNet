using System.Net.Http;
using System.Threading.Tasks;

namespace Docker.Registry.DotNet.Authentication
{
    internal class AnonymousAuthenticationProvider : AuthenticationProvider
    {
        internal override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        internal override Task AuthenticateAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            return Task.CompletedTask;
        }
    }
}