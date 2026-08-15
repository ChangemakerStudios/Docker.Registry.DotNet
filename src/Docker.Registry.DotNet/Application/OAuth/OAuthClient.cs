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

using Docker.Registry.DotNet.Application.QueryStrings;

namespace Docker.Registry.DotNet.Application.OAuth;

internal class OAuthClient
{
    private static readonly HttpClient _client = new();

    private async Task<OAuthToken?> GetTokenInner(
        string? realm,
        string? service,
        string? scope,
        string? username,
        string? password,
        CancellationToken token = default)
    {
        using var activity = Assembly.Source.StartActivity("OAuthClient.GetTokenInner()");

        HttpRequestMessage request;

        if (username == null || password == null)
        {
            var queryString = new QueryString();

            queryString.AddIfNotEmpty("service", service);
            queryString.AddIfNotEmpty("scope", scope);

            var builder = new UriBuilder(new Uri(realm))
            {
                Query = queryString.GetQueryString()
            };

            request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        }
        else
        {
            request = new HttpRequestMessage(HttpMethod.Post, realm)
            {
                Content = new FormUrlEncodedContent(
                    new Dictionary<string, string?>
                    {
                        { "client_id", "Docker.Registry.DotNet" },
                        { "grant_type", "password" },
                        { "username", username },
                        { "password", password },
                        { "service", service },
                        { "scope", scope }
                    }
                )
            };
        }

        activity?.AddEvent(new ActivityEvent("Getting Token"));

        try
        {
            using var response = await _client.SendAsync(request, token);

            if (!response.IsSuccessStatusCode)
            {
                activity?.AddEvent(new ActivityEvent("Failed to Authenticate"));

                throw new UnauthorizedAccessException(
                    $"Unable to authenticate: {await response.Content.ReadAsStringAsyncWithCancellation(token)}");
            }

            var body = await response.Content.ReadAsStringAsyncWithCancellation(token);

            var authToken = JsonConvert.DeserializeObject<OAuthToken>(body);

            return authToken;
        }
        catch (Exception ex)
        {
            activity?.AddTag("Authentication Exception", ex);

            throw new UnauthorizedAccessException($"Unable to authenticate: {ex}");
        }
    }

    public Task<OAuthToken?> GetToken(
        string? realm,
        string? service,
        string? scope,
        CancellationToken cancellationToken = default)
    {
        return this.GetTokenInner(realm, service, scope, null, null, cancellationToken);
    }

    public Task<OAuthToken?> GetToken(
        string? realm,
        string? service,
        string? scope,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        return this.GetTokenInner(
            realm,
            service,
            scope,
            username,
            password,
            cancellationToken);
    }
}