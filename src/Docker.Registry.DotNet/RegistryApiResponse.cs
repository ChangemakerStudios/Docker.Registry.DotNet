using System.Collections.Generic;
using System.Net;

namespace Docker.Registry.DotNet
{
    internal class RegistryApiResponse
    {
        public RegistryApiResponse(HttpStatusCode statusCode, string body, KeyValuePair<string, string[]>[] headers)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = headers;
        }

        public HttpStatusCode StatusCode { get; }

        public string Body { get; }

        public KeyValuePair<string, string[]>[] Headers { get; }
    }
}