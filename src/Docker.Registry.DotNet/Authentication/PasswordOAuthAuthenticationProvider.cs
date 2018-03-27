using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Docker.Registry.DotNet.OAuth;

namespace Docker.Registry.DotNet.Authentication
{
    public class PasswordOAuthAuthenticationProvider : AuthenticationProvider
    {
        private readonly string _username;
        private readonly string _password;
        private readonly OAuthClient _client = new OAuthClient();

        public PasswordOAuthAuthenticationProvider(string username, string password)
        {
            _username = username;
            _password = password;
        }

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

                    //if (scope == "registry:catalog:*")
                    //{
                    //    scope = "registry:catalog:get";
                    //}

                    //Get the token
                    var token = await _client.GetTokenAsync(bearerBits.Realm, bearerBits.Service, scope, _username, _password);

                    //Console.WriteLine(token.AccessToken);

                    //Set the header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                    return;
                }
            }

            throw new InvalidOperationException("No WWW-Authenticate challenge was found.");
        }
    }
}