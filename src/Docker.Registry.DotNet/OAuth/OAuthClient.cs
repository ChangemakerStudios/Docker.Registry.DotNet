using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;

using Newtonsoft.Json;

namespace Docker.Registry.DotNet.OAuth
{
    internal class OAuthClient
    {
        private readonly HttpClient _client = new HttpClient();

        private async Task<OAuthToken> GetTokenInnerAsync(
            string realm,
            string service,
            string scope,
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            var queryString = new QueryString();

            queryString.AddIfNotEmpty("service", service);
            queryString.AddIfNotEmpty("scope", scope);

            var builder = new UriBuilder(new Uri(realm))
                          {
                              Query = queryString.GetQueryString()
                          };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            if (username != null && password != null)
            {
                // https://gist.github.com/jlhawn/8f218e7c0b14c941c41f

                var bytes = Encoding.UTF8.GetBytes($"{username}:{password}");

                var parameter = Convert.ToBase64String(bytes);

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", parameter);
            }

            using (var response = await this._client.SendAsync(request, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                    throw new UnauthorizedAccessException("Unable to authenticate.");

                var body = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<OAuthToken>(body);

                return token;
            }
        }

        public Task<OAuthToken> GetTokenAsync(
            string realm,
            string service,
            string scope,
            CancellationToken cancellationToken = default)
        {
            return this.GetTokenInnerAsync(realm, service, scope, null, null, cancellationToken);
        }

        public Task<OAuthToken> GetTokenAsync(
            string realm,
            string service,
            string scope,
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            return this.GetTokenInnerAsync(
                realm,
                service,
                scope,
                username,
                password,
                cancellationToken);
        }
    }
}