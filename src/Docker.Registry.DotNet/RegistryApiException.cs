using System;
using System.Collections.Generic;
using System.Net;

namespace Docker.Registry.DotNet
{
    public class RegistryApiException : Exception
    {
        internal RegistryApiException(RegistryApiResponse response)
            : base($"Docker API responded with status code={response.StatusCode}, response={response.Body}")
        {
            StatusCode = response.StatusCode;
            ResponseBody = response.Body;
            Headers = response.Headers;
        }

        public HttpStatusCode StatusCode { get; }

        public string ResponseBody { get; }

        public KeyValuePair<string, string[]>[] Headers { get; }
    }
}