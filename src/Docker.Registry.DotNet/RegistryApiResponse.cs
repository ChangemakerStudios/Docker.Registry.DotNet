using System.Net;
using System.Net.Http.Headers;

namespace Docker.Registry.DotNet
{
    internal abstract class RegistryApiResponse
    {
        protected RegistryApiResponse(HttpStatusCode statusCode, HttpResponseHeaders headers)
        {
            StatusCode = statusCode;
            Headers = headers;
        }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders Headers { get; }
    }

    internal class RegistryApiResponse<TBody> : RegistryApiResponse
    {
        internal RegistryApiResponse(HttpStatusCode statusCode, TBody body, HttpResponseHeaders headers) : base(statusCode, headers)
        {
            Body = body;
        }

        public TBody Body { get; }
    }
}