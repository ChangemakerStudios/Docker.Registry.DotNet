using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Docker.Registry.DotNet.OAuth
{
    internal class OAuthClient
    {
        private readonly HttpClient _client = new HttpClient();

        

        public async Task<OAuthToken> GetTokenAsync(string realm, string service, string scope, CancellationToken cancellationToken = new CancellationToken())
        {
            var queryString = new QueryString();

            queryString.AddIfNotEmpty("service", service);
            queryString.AddIfNotEmpty("scope", scope);

            UriBuilder builder = new UriBuilder(new Uri(realm))
            {
                Query = queryString.GetQueryString()
            };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            using (var response = await _client.SendAsync(request, cancellationToken))
            {
                string body = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<OAuthToken>(body);

               

                return token;
            }
        }

        public async Task<OAuthToken> GetTokenAsync(string realm, string service, string scope, string username, string password, CancellationToken cancellationToken = new CancellationToken())
        {
            var queryString = new QueryString();

            queryString.Add("grant_type", "password");
            queryString.AddIfNotEmpty("service", service);
            queryString.AddIfNotEmpty("scope", scope);
            queryString.Add("username", username);
            queryString.Add("password", password);

            UriBuilder builder = new UriBuilder(new Uri(realm))
            {
                Query = queryString.GetQueryString()
            };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            using (var response = await _client.SendAsync(request, cancellationToken))
            {
                string body = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<OAuthToken>(body);

                //var parsed = _tokenHandler.ReadJwtToken(token.AccessToken);

                return token;


            }

        }
    }
}