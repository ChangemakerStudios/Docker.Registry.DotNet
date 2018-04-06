using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Docker.Registry.DotNet.OAuth
{
    using System.Net.Http.Headers;
    using System.Text;

    internal class OAuthClient
    {
        private readonly HttpClient _client = new HttpClient();

        private async Task<OAuthToken> GetTokenInnerAsync(string realm, string service, string scope, string username,
            string password, CancellationToken cancellationToken = new CancellationToken())
        {
            var queryString = new QueryString();

            queryString.AddIfNotEmpty("service", service);

            //if (!string.IsNullOrEmpty(scope))
            //{
                queryString.AddIfNotEmpty("scope", scope);
            //}

            UriBuilder builder = new UriBuilder(new Uri(realm))
            {
                Query = queryString.GetQueryString()
            };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            if (username != null && password != null)
            {
                // https://gist.github.com/jlhawn/8f218e7c0b14c941c41f

                var bytes = Encoding.UTF8.GetBytes($"{username}:{password}");

                string parameter = Convert.ToBase64String(bytes);

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", parameter);
            }

            using (var response = await _client.SendAsync(request, cancellationToken))
            {
                string body = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<OAuthToken>(body);

                return token;
            }
        }

        public Task<OAuthToken> GetTokenAsync(string realm, string service, string scope, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetTokenInnerAsync(realm, service, scope, null, null, cancellationToken);
        }

        public Task<OAuthToken> GetTokenAsync(string realm, string service, string scope, string username, string password, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetTokenInnerAsync(realm, service, scope, username, password, cancellationToken);
        }
    }
}