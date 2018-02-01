using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Docker.Registry.DotNet.OAuth;

namespace Docker.Registry.DotNet.Authentication
{
    internal class AnonymousOAuthAuthenticationProvider : AuthenticationProvider
    {
        private readonly OAuthClient _client = new OAuthClient();

        internal override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        internal override async Task AuthenticateAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            foreach (var header in response.Headers.WwwAuthenticate)
            {
                if (header.Scheme == "Bearer")
                {
                    //Get the bearer bits
                    var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

                    string scope = bearerBits.Scope;
                    
                    //Get the token
                    var token = await _client.GetTokenAsync(bearerBits.Realm, bearerBits.Service, scope);

                    var tokenHandler = new JwtSecurityTokenHandler();

                    var parsed = tokenHandler.ReadJwtToken(token.AccessToken);

                    foreach (var payloadItem in parsed.Payload)
                    {
                        Console.WriteLine($" {payloadItem.Key}: {payloadItem.Value}");
                    }

                    //Set the header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                    return;
                }
            }

            throw new InvalidOperationException("No WWW-Authenticate challenge was found.");
        }
    }
}