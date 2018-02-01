using System.Net.Http;
using System.Threading.Tasks;

namespace Docker.Registry.DotNet.Authentication
{
    /// <summary>
    /// Authentication provider.
    /// </summary>
    internal abstract class AuthenticationProvider
    {
        /// <summary>
        /// Called on the initial send
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal abstract Task AuthenticateAsync(HttpRequestMessage request);

        /// <summary>
        /// Called when the send is challenged.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        internal abstract Task AuthenticateAsync(HttpRequestMessage request, HttpResponseMessage response);
    }
}