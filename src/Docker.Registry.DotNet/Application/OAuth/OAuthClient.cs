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

internal class OAuthClient(HttpClient? client = null)
{
    private static readonly HttpClient _defaultClient = new();

    private readonly HttpClient _client = client ?? _defaultClient;

    private async Task<OAuthToken?> GetTokenInner(
        string? realm,
        string? service,
        string? scope,
        string? username,
        string? password,
        CancellationToken token = default)
    {
        using var activity = Assembly.Source.StartActivity("OAuthClient.GetTokenInner()");

        activity?.AddEvent(new ActivityEvent("Getting Token"));

        try
        {
            if (username != null && password != null)
            {
                // Try the OAuth2 password grant first -- it supports multiple scopes
                // (e.g. the repository(plugin) scope on Docker Hub).
                using var oauthResponse = await _client.SendAsync(
                    BuildOAuth2Request(realm, service, scope, username, password),
                    token);

                if (oauthResponse.IsSuccessStatusCode)
                    return await ReadToken(oauthResponse, token);

                // Token servers that only implement the classic token endpoint
                // (e.g. docker/distribution) reject the POST -- fall back to
                // a GET with basic auth per the Docker Registry token spec.
                if (oauthResponse.StatusCode is not (HttpStatusCode.MethodNotAllowed
                    or HttpStatusCode.NotFound or HttpStatusCode.BadRequest))
                {
                    activity?.AddEvent(new ActivityEvent("Failed to Authenticate"));

                    throw new UnauthorizedAccessException(
                        $"Unable to authenticate: {await oauthResponse.Content.ReadAsStringAsyncWithCancellation(token)}");
                }

                activity?.AddEvent(
                    new ActivityEvent("OAuth2 Grant Unsupported - Falling Back to Basic Auth"));
            }

            using var response = await _client.SendAsync(
                BuildTokenRequest(realm, service, scope, username, password),
                token);

            if (!response.IsSuccessStatusCode)
            {
                activity?.AddEvent(new ActivityEvent("Failed to Authenticate"));

                throw new UnauthorizedAccessException(
                    $"Unable to authenticate: {await response.Content.ReadAsStringAsyncWithCancellation(token)}");
            }

            return await ReadToken(response, token);
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            activity?.AddTag("Authentication Exception", ex);

            throw new UnauthorizedAccessException($"Unable to authenticate: {ex}");
        }
    }

    private static HttpRequestMessage BuildOAuth2Request(
        string? realm,
        string? service,
        string? scope,
        string username,
        string password)
    {
        return new HttpRequestMessage(HttpMethod.Post, realm)
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

    private static HttpRequestMessage BuildTokenRequest(
        string? realm,
        string? service,
        string? scope,
        string? username,
        string? password)
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
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{username}:{password}"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        return request;
    }

    private static async Task<OAuthToken?> ReadToken(
        HttpResponseMessage response,
        CancellationToken token)
    {
        var body = await response.Content.ReadAsStringAsyncWithCancellation(token);

        return JsonConvert.DeserializeObject<OAuthToken>(body);
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