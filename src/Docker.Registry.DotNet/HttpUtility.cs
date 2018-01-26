using System;
using System.Collections.Generic;
using System.Linq;

namespace Docker.Registry.DotNet
{
    internal static class HttpUtility
    {
        public static Uri BuildUri(Uri baseUri, string path, IQueryString queryString)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            var builder = new UriBuilder(baseUri);

            if (!string.IsNullOrEmpty(path))
            {
                builder.Path += path;
            }

            if (queryString != null)
            {
                builder.Query = queryString.GetQueryString();
            }

            return builder.Uri;
        }

        /// <summary>
        /// Attempts to retrieve the value of a response header.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="name"></param>
        /// <returns>The first value if one is found, null otherwise.</returns>
        public static string GetHeader(this RegistryApiResponse response, string name)
        {
            return response.Headers
                .FirstOrDefault(h => h.Key == name).Value?.FirstOrDefault();
        }


        /// <summary>
        /// Attempts to retrieve the value of a response header.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="name"></param>
        /// <returns>The first value if one is found, null otherwise.</returns>
        public static string[] GetHeaders(this KeyValuePair<string, string[]>[] headers, string name)
        {
            return headers
                .Where(h => h.Key == name)
                .Select(h => h.Value?.FirstOrDefault())
                .ToArray();
        }
    }
}