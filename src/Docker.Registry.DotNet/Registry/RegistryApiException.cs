using System;
using System.Net;
using System.Net.Http.Headers;

namespace Docker.Registry.DotNet.Registry
{
    public class RegistryApiException : Exception
    {
        internal RegistryApiException(RegistryApiResponse response)
            : base($"Docker API responded with status code={response.StatusCode}")
        {
            this.StatusCode = response.StatusCode;
            this.Headers = response.Headers;
        }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders Headers { get; }
    }

    public class RegistryApiException<TBody> : RegistryApiException
    {
        internal RegistryApiException(RegistryApiResponse<TBody> response)
            : base(response)
        {
            this.Body = response.Body;
        }

        public TBody Body { get; }
    }
}