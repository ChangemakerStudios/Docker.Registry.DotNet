// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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

using System.Net;
using System.Text;

using Docker.Registry.DotNet.Application.OAuth;

using FluentAssertions;

using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests.OAuth;

[TestFixture]
public class OAuthClientTests
{
    private const string Realm = "https://auth.example.com/token";

    private static readonly string ClassicTokenJson = """{"token":"classic-token"}""";

    private static readonly string OAuth2TokenJson = """{"access_token":"oauth2-token"}""";

    private static OAuthClient CreateClient(FakeHttpHandler handler) => new(new HttpClient(handler));

    [Test]
    public async Task GivenNoCredentials_WhenGettingAToken_ThenItShouldUseGetWithoutAuthorization()
    {
        var handler = new FakeHttpHandler(
            _ => Respond(HttpStatusCode.OK, ClassicTokenJson));

        var token = await CreateClient(handler).GetToken(Realm, "service", "scope");

        token?.Token.Should().Be("classic-token");

        handler.Requests.Should().HaveCount(1);
        handler.Requests[0].Method.Should().Be(HttpMethod.Get);
        handler.Requests[0].Headers.Authorization.Should().BeNull();
        handler.Requests[0].RequestUri!.Query.Should().Contain("service=service").And.Contain("scope=scope");
    }

    [Test]
    public async Task GivenCredentials_WhenTheOAuth2GrantSucceeds_ThenItShouldNotFallBack()
    {
        var handler = new FakeHttpHandler(
            _ => Respond(HttpStatusCode.OK, OAuth2TokenJson));

        var token = await CreateClient(handler).GetToken(Realm, "service", "scope", "user", "pass");

        token?.AccessToken.Should().Be("oauth2-token");

        handler.Requests.Should().HaveCount(1);
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
    }

    [TestCase(HttpStatusCode.MethodNotAllowed)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public async Task GivenCredentials_WhenTheOAuth2GrantIsUnsupported_ThenItShouldFallBackToBasicAuthGet(
        HttpStatusCode oauth2Status)
    {
        var handler = new FakeHttpHandler(
            request => request.Method == HttpMethod.Post
                ? Respond(oauth2Status, "method not allowed")
                : Respond(HttpStatusCode.OK, ClassicTokenJson));

        var token = await CreateClient(handler).GetToken(Realm, "service", "scope", "user", "pass");

        token?.Token.Should().Be("classic-token");

        handler.Requests.Should().HaveCount(2);
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[1].Method.Should().Be(HttpMethod.Get);

        var authorization = handler.Requests[1].Headers.Authorization;

        authorization.Should().NotBeNull();
        authorization!.Scheme.Should().Be("Basic");
        authorization.Parameter.Should()
            .Be(Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")));
    }

    [Test]
    public async Task GivenCredentials_WhenTheOAuth2GrantIsRejected_ThenItShouldThrowWithoutFallingBack()
    {
        var handler = new FakeHttpHandler(
            _ => Respond(HttpStatusCode.Unauthorized, "invalid credentials"));

        var getToken = () => CreateClient(handler).GetToken(Realm, "service", "scope", "user", "pass");

        await getToken.Should().ThrowAsync<UnauthorizedAccessException>();

        handler.Requests.Should().HaveCount(1);
    }

    [Test]
    public async Task GivenCredentials_WhenTheFallbackAlsoFails_ThenItShouldThrow()
    {
        var handler = new FakeHttpHandler(
            request => request.Method == HttpMethod.Post
                ? Respond(HttpStatusCode.MethodNotAllowed, "method not allowed")
                : Respond(HttpStatusCode.Unauthorized, "invalid credentials"));

        var getToken = () => CreateClient(handler).GetToken(Realm, "service", "scope", "user", "pass");

        await getToken.Should().ThrowAsync<UnauthorizedAccessException>();

        handler.Requests.Should().HaveCount(2);
    }

    private static HttpResponseMessage Respond(HttpStatusCode statusCode, string body) =>
        new(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

    private class FakeHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            this.Requests.Add(request);

            return Task.FromResult(responder(request));
        }
    }
}
