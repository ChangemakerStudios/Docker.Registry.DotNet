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
using Docker.Registry.DotNet.Domain.Manifests;
using Docker.Registry.DotNet.Domain.Registry;

using FluentAssertions;

using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests;

[TestFixture]
public class OciManifestTests
{
    private const string IndexDigest =
        "sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    private const string ChildDigest =
        "sha256:bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    private const string OciIndexBody =
        $$"""
          {"schemaVersion":2,"mediaType":"{{ManifestMediaTypes.OciIndex}}","manifests":[{"mediaType":"{{ManifestMediaTypes.OciManifest}}","digest":"{{ChildDigest}}","size":1234}]}
          """;

    private const string OciManifestBody =
        $$"""
          {"schemaVersion":2,"mediaType":"{{ManifestMediaTypes.OciManifest}}","config":{"mediaType":"{{ManifestMediaTypes.OciContainerConfig}}","digest":"sha256:cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc","size":100},"layers":[]}
          """;

    private static IRegistryClient CreateClient(FakeOciRegistryHandler handler)
    {
        return new RegistryClientConfiguration("http://localhost:5000")
            .SetHttpMessageHandler(handler)
            .CreateClient();
    }

    [Test]
    public async Task AcceptHeaderIncludesOciMediaTypes()
    {
        var handler = new FakeOciRegistryHandler();

        using var client = CreateClient(handler);

        await client.Manifest.GetManifest("test", ImageReference.Create(IndexDigest));

        handler.LastAcceptHeader.Should().Contain(ManifestMediaTypes.OciIndex);
        handler.LastAcceptHeader.Should().Contain(ManifestMediaTypes.OciManifest);
    }

    [Test]
    public async Task GetManifestParsesOciIndexAsManifestList()
    {
        using var client = CreateClient(new FakeOciRegistryHandler());

        var result = await client.Manifest.GetManifest("test", ImageReference.Create(IndexDigest));

        result.MediaType.Should().Be(ManifestMediaTypes.OciIndex);
        result.Manifest.Should().BeOfType<ManifestList>();

        var list = (ManifestList)result.Manifest;
        list.Manifests.Should().ContainSingle(m => m.Digest == ChildDigest);
    }

    [Test]
    public async Task GetManifestParsesOciImageManifest()
    {
        using var client = CreateClient(new FakeOciRegistryHandler());

        var result = await client.Manifest.GetManifest("test", ImageReference.Create(ChildDigest));

        result.MediaType.Should().Be(ManifestMediaTypes.OciManifest);
        result.Manifest.Should().BeOfType<ImageManifest2_2>();

        var manifest = (ImageManifest2_2)result.Manifest;
        manifest.Config?.MediaType.Should().Be(ManifestMediaTypes.OciContainerConfig);
    }

    [Test]
    public async Task GetManifestByTagResolvesOciIndex()
    {
        using var client = CreateClient(new FakeOciRegistryHandler());

        var result = await client.Manifest.GetManifest("test", ImageReference.Create("latest"));

        result.Manifest.Should().BeOfType<ManifestList>();
    }

    /// <summary>
    ///     Serves an OCI index at tag "latest"/its digest and an OCI image manifest at the
    ///     child digest, mimicking an image pushed by buildkit with provenance enabled (#28).
    /// </summary>
    private class FakeOciRegistryHandler : HttpMessageHandler
    {
        public string? LastAcceptHeader { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            this.LastAcceptHeader = request.Headers.Accept.ToString();

            var isChild = request.RequestUri!.AbsolutePath.EndsWith($"/manifests/{ChildDigest}");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    isChild ? OciManifestBody : OciIndexBody,
                    Encoding.UTF8,
                    isChild ? ManifestMediaTypes.OciManifest : ManifestMediaTypes.OciIndex)
            };

            response.Headers.Add(
                "Docker-Content-Digest",
                isChild ? ChildDigest : IndexDigest);

            return Task.FromResult(response);
        }
    }
}
