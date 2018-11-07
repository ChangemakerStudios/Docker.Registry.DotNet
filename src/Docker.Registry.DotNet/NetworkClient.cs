using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        private const string UserAgent = "Docker.Registry.DotNet";

        private Uri _effectiveEndpointBaseUri;

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

            if (_configuration.EndpointBaseUri != null)
            {
                _effectiveEndpointBaseUri = _configuration.EndpointBaseUri;
            }
        }

        /// <summary>
        /// Ensures that we have configured (and potentially probed) the end point.
        /// </summary>
        /// <returns></returns>
        private async Task EnsureConnection()
        {
            if (_effectiveEndpointBaseUri == null)
            {
                string httpUrl = $"http://{_configuration.Host}";
                string httpsUrl = $"https://{_configuration.Host}";

                if (await ProbeSingleAsync(httpsUrl))
                {
                    _effectiveEndpointBaseUri = new Uri(httpsUrl);
                }
                else if (await ProbeSingleAsync(httpUrl))
                {
                    _effectiveEndpointBaseUri = new Uri(httpUrl);
                }
                else
                {
                    throw new RegistryConnectionException($"Unable to connect to '{_configuration.Host}'");
                }
            }
        }

        async Task<bool> ProbeSingleAsync(string uri)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                using (await _client.SendAsync(request))
                {

                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("  " + ex.GetType().Name);

                return false;
            }
        }

        internal async Task<RegistryApiResponse<string>> MakeRequestAsync(
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            IQueryString queryString = null,
            IDictionary<string, string> headers = null,
            Func<HttpContent> content = null)
        {
            using (var response = await InternalMakeRequestAsync(DefaultTimeout,
                HttpCompletionOption.ResponseContentRead, method, path, queryString, headers, content, cancellationToken))
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var apiResponse = new RegistryApiResponse<string>(response.StatusCode, responseBody, response.Headers);

                HandleIfErrorResponse(apiResponse);

                return apiResponse;
            }
        }

        internal async Task<RegistryApiResponse<Stream>> MakeRequestForStreamedResponseAsync(
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            IQueryString queryString = null)
        {
            var response = await InternalMakeRequestAsync(InfiniteTimeout, HttpCompletionOption.ResponseHeadersRead, method, path, queryString, null, null, cancellationToken);

            var body = await response.Content.ReadAsStreamAsync();

            var apiResponse = new RegistryApiResponse<Stream>(response.StatusCode, body, response.Headers);

            HandleIfErrorResponse(apiResponse);

            return apiResponse;
        }

        private async Task<HttpResponseMessage> InternalMakeRequestAsync(
            TimeSpan timeout,
            HttpCompletionOption completionOption,
            HttpMethod method,
            string path,
            IQueryString queryString,
            IDictionary<string, string> headers,
            Func<HttpContent> content,
            CancellationToken cancellationToken)
        {
            await EnsureConnection();

            var request = PrepareRequest(method, path, queryString, headers, content);

            if (timeout != InfiniteTimeout)
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

        internal HttpRequestMessage PrepareRequest(HttpMethod method, string path, IQueryString queryString, IDictionary<string, string> headers, Func<HttpContent> content)
        {
            if (string.IsNullOrEmpty("path"))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var request = new HttpRequestMessage(method, HttpUtility.BuildUri(_effectiveEndpointBaseUri, path, queryString));

            request.Headers.Add("User-Agent", UserAgent);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            //Create the content
            request.Content = content?.Invoke();
           
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