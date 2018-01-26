using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Authentication;

namespace Docker.Registry.DotNet
{
    internal class NetworkClient : IDisposable
    {
        private readonly RegistryClientConfiguration _configuration;
        private readonly AuthenticationProvider _authenticationProvider;
        private readonly HttpClient _client;

        private static readonly TimeSpan s_InfiniteTimeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        private const string UserAgent = "Docker.Registry.DotNet";

        private readonly IEnumerable<Action<RegistryApiResponse>> _errorHandlers = new Action<RegistryApiResponse>[]
        {
            r =>
            {
                if (r.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedApiException(r);
                }
            }
        };

        public NetworkClient(RegistryClientConfiguration configuration, AuthenticationProvider authenticationProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));

            _client = new HttpClient();

            DefaultTimeout = configuration.DefaultTimeout;

            JsonSerializer = new JsonSerializer();
        }

        internal async Task<RegistryApiResponse> MakeRequestAsync(
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            IQueryString queryString = null,
            IDictionary<string, string> headers = null,
            HttpContent content = null)
        {
            using (var response = await InternalMakeRequestAsync(DefaultTimeout,
                HttpCompletionOption.ResponseContentRead, method, path, queryString, headers, content, cancellationToken))
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var responseHeaders =
                    response.Headers.Select(h => new KeyValuePair<string, string[]>(h.Key, h.Value.ToArray()))
                        .ToArray();

                var apiResponse = new RegistryApiResponse(response.StatusCode, responseBody, responseHeaders);

                HandleIfErrorResponse(apiResponse);

                return apiResponse;
            }
        }

        private async Task<HttpResponseMessage> InternalMakeRequestAsync(
            TimeSpan timeout,
            HttpCompletionOption completionOption,
            HttpMethod method,
            string path,
            IQueryString queryString,
            IDictionary<string, string> headers,
            HttpContent content,
            CancellationToken cancellationToken)
        {
            var request = PrepareRequest(method, path, queryString, headers, content);

            if (timeout != s_InfiniteTimeout)
            {
                var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutTokenSource.CancelAfter(timeout);
                cancellationToken = timeoutTokenSource.Token;
            }

            await _authenticationProvider.AuthenticateAsync(request);

            var response = await _client.SendAsync(request, completionOption, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //Prepare another request (we can't reuse the same request)
                var request2 = PrepareRequest(method, path, queryString, headers, content);

                //Authenticate given the challenge
                await _authenticationProvider.AuthenticateAsync(request2, response);

                //Send it again
                response = await _client.SendAsync(request2, completionOption, cancellationToken);
            }

            return response;
        }

        private void HandleIfErrorResponse(RegistryApiResponse response)
        {
            // If no customer handlers just default the response.
            foreach (var handler in _errorHandlers)
            {
                handler(response);
            }

            // No custom handler was fired. Default the response for generic success/failures.
            if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.BadRequest)
            {
                throw new RegistryApiException(response);
            }
        }

        internal HttpRequestMessage PrepareRequest(HttpMethod method, string path, IQueryString queryString, IDictionary<string, string> headers, HttpContent content)
        {
            if (string.IsNullOrEmpty("path"))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var request = new HttpRequestMessage(method, HttpUtility.BuildUri(_configuration.EndpointBaseUri, path, queryString));

            request.Headers.Add("User-Agent", UserAgent);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.Content = content;
           
            return request;
        }

        public TimeSpan DefaultTimeout { get; set; }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public JsonSerializer JsonSerializer { get; }
    }
}