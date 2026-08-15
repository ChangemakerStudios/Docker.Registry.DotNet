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
using System.Net.Http;
using System.Text;

using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Registry;

using FluentAssertions;

using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests;

[TestFixture]
public class ManifestOperationsTests
{
    private const string Digest =
        "sha256:8c5d9e9a493cf7a1b6320761b4c916d037e0543ac9d85a7172055a1f691f1c32";

    private const string ManifestBody =
        """{"schemaVersion":2,"mediaType":"application/vnd.docker.distribution.manifest.v2+json","config":{},"layers":[]}""";

    private static IRegistryClient CreateClient(FakeManifestHandler handler)
    {
        return new RegistryClientConfiguration("http://localhost:5000")
            .SetHttpMessageHandler(handler)
            .CreateClient();
    }

    [Test]
    public async Task GetDigestUsesHeadRequest()
    {
        // e.g. AWS ECR only returns Docker-Content-Digest on HEAD (#34)
        var handler = new FakeManifestHandler(digestOnHead: true, digestOnGet: false);

        using var client = CreateClient(handler);

        var digest = await client.Manifest.GetDigest("test", new ImageTag("latest"));

        digest.Should().NotBeNull();
        digest!.Value.Should().Be(Digest);
        handler.Requests.Should().ContainSingle(m => m == HttpMethod.Head);
    }

    [Test]
    public async Task GetDigestFallsBackToGetWhenHeadHasNoDigestHeader()
    {
        var handler = new FakeManifestHandler(digestOnHead: false, digestOnGet: true);

        using var client = CreateClient(handler);

        var digest = await client.Manifest.GetDigest("test", new ImageTag("latest"));

        digest.Should().NotBeNull();
        digest!.Value.Should().Be(Digest);
        handler.Requests.Should().Equal(HttpMethod.Head, HttpMethod.Get);
    }

    [Test]
    public async Task GetDigestReturnsNullWhenNoDigestHeaderIsReturned()
    {
        var handler = new FakeManifestHandler(digestOnHead: false, digestOnGet: false);

        using var client = CreateClient(handler);

        var digest = await client.Manifest.GetDigest("test", new ImageTag("latest"));

        digest.Should().BeNull();
    }

    private class FakeManifestHandler(bool digestOnHead, bool digestOnGet) : HttpMessageHandler
    {
        public List<HttpMethod> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            this.Requests.Add(request.Method);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    request.Method == HttpMethod.Head ? string.Empty : ManifestBody,
                    Encoding.UTF8,
                    "application/vnd.docker.distribution.manifest.v2+json")
            };

            var includeDigest = request.Method == HttpMethod.Head ? digestOnHead : digestOnGet;

            if (includeDigest) response.Headers.Add("Docker-Content-Digest", Digest);

            return Task.FromResult(response);
        }
    }
}
