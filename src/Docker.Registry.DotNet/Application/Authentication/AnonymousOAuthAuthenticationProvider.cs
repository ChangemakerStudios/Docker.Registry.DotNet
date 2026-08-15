//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Docker.Registry.DotNet.Application.OAuth;

namespace Docker.Registry.DotNet.Application.Authentication;


public class AnonymousOAuthAuthenticationProvider : AuthenticationProvider
{
    private readonly OAuthClient _client = new();

    private static string Schema { get; } = "Bearer";

    public override Task Authenticate(HttpRequestMessage request, IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("AnonymousOAuthAuthenticationProvider.Authenticate(request)");

        return Task.CompletedTask;
    }

    public override async Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("AnonymousOAuthAuthenticationProvider.Authenticate(request, response)");

        var header = this.TryGetSchemaHeader(response, Schema);

        //Get the bearer bits
        var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

        //Get the token
        var token = await this._client.GetToken(
            bearerBits.Realm,
            bearerBits.Service,
            bearerBits.Scope);

        if (token?.Token == null)
        {
            throw new ArgumentNullException(nameof(token.Token), "Authorization token cannot be null");
        }

        //Set the header
        request.Headers.Authorization = new AuthenticationHeaderValue(Schema, token.Token);
    }
}