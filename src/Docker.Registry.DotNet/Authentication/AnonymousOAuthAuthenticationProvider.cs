using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Docker.Registry.DotNet.OAuth;

namespace Docker.Registry.DotNet.Authentication
{
    public class AnonymousOAuthAuthenticationProvider : AuthenticationProvider
    {
        private readonly OAuthClient _client = new OAuthClient();

        public override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        public override async Task AuthenticateAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            foreach (var header in response.Headers.WwwAuthenticate)
            {
                if (header.Scheme == "Bearer")
                {
                    //Get the bearer bits
                    var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

                    //Get the token
                    var token = await _client.GetTokenAsync(bearerBits.Realm, bearerBits.Service, bearerBits.Scope);

                    //Set the header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                    return;
                }
            }

            throw new InvalidOperationException("No WWW-Authenticate challenge was found.");
        }
    }
}