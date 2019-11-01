using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Docker.Registry.DotNet.Authentication
{
    public class BasicAuthenticationProvider : AuthenticationProvider
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthenticationProvider(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public override Task AuthenticateAsync(HttpRequestMessage request)
        {
            return Task.CompletedTask;
        }

        public override Task AuthenticateAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            foreach (var header in response.Headers.WwwAuthenticate)
            {
                if (header.Scheme == "Basic")
                {
                    // Convert password to base64 encoded string
                    var passBytes = Encoding.UTF8.GetBytes($"{_username}:{_password}");
                    var base64Pass = Convert.ToBase64String(passBytes);

                    //Set the header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Pass);

                    return Task.CompletedTask;
                }
            }

            throw new InvalidOperationException("No WWW-Authenticate challenge was found.");
        }
    }
}