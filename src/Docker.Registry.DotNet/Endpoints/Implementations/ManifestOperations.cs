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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Helpers;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Docker.Registry.DotNet.Endpoints.Implementations
{
    internal class ManifestOperations : IManifestOperations
    {
        private readonly NetworkClient _client;

        public ManifestOperations(NetworkClient client)
        {
            this._client = client;
        }

        public async Task<GetImageManifestResult> GetManifestAsync(
            string name,
            string reference,
            CancellationToken cancellationToken = default)
        {
            var headers = new Dictionary<string, string>
            {
                {
                    "Accept",
                    $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"
                }
            };

            var response = await this._client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Head,
                $"v2/{name}/manifests/{reference}",
                null,
                headers).ConfigureAwait(false);

            var digestReference = response.GetHeader("Docker-Content-Digest");

            response = await this._client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Get,
                $"v2/{name}/manifests/{digestReference}",
                null,
                headers).ConfigureAwait(false);

            var contentType = this.GetContentType(response.GetHeader("ContentType"), response.Body);

            switch (contentType)
            {
                case ManifestMediaTypes.ManifestSchema1:
                case ManifestMediaTypes.ManifestSchema1Signed:
                    return new GetImageManifestResult(
                        contentType,
                        this._client.JsonSerializer.DeserializeObject<ImageManifest2_1>(
                            response.Body),
                        response.Body)
                    {
                        DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
                        Etag = response.GetHeader("Etag")
                    };

                case ManifestMediaTypes.ManifestSchema2:
                    return new GetImageManifestResult(
                        contentType,
                        this._client.JsonSerializer.DeserializeObject<ImageManifest2_2>(
                            response.Body),
                        response.Body)
                    {
                        DockerContentDigest = response.GetHeader("Docker-Content-Digest")
                    };

                case ManifestMediaTypes.ManifestList:
                    return new GetImageManifestResult(
                        contentType,
                        this._client.JsonSerializer.DeserializeObject<ManifestList>(response.Body),
                        response.Body);

                default:
                    throw new Exception($"Unexpected ContentType '{contentType}'.");
            }
        }

        public async Task<PushManifestResponse> PutManifestAsync(
            string name,
            string reference,
            ImageManifest manifest,
            CancellationToken cancellationToken)
        {
            string manifestMediaType = null;
            if (manifest is ImageManifest2_1)
                manifestMediaType = ManifestMediaTypes.ManifestSchema1;
            if (manifest is ImageManifest2_2)
                manifestMediaType = ManifestMediaTypes.ManifestSchema2;
            if (manifest is ManifestList)
                manifestMediaType = ManifestMediaTypes.ManifestList;

            var response = await this._client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Put,
                $"v2/{name}/manifests/{reference}",
                content: () =>
                {
                    var content = new StringContent(
                        this._client.JsonSerializer.SerializeObject(manifest));
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue(manifestMediaType);
                    return content;
                }).ConfigureAwait(false);

            return new PushManifestResponse
            {
                DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
                ContentLength = response.GetHeader("Content-Length"),
                Location = response.GetHeader("location"),
            };
        }

        //public Task<bool> DoesManifestExistAsync(string name, string reference, CancellationToken cancellation = default)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task DeleteManifestAsync(
            string name,
            string reference,
            CancellationToken cancellationToken = default)
        {
            var path = $"v2/{name}/manifests/{reference}";

            await this._client.MakeRequestAsync(cancellationToken, HttpMethod.Delete, path);
        }

        private string GetContentType(string contentTypeHeader, string manifest)
        {
            if (!string.IsNullOrWhiteSpace(contentTypeHeader))
                return contentTypeHeader;

            var check = JsonConvert.DeserializeObject<SchemaCheck>(manifest);

            if (!string.IsNullOrWhiteSpace(check.MediaType))
                return check.MediaType;

            if (check.SchemaVersion == null)
                return ManifestMediaTypes.ManifestSchema1;

            if (check.SchemaVersion.Value == 2)
                return ManifestMediaTypes.ManifestSchema2;

            throw new Exception(
                $"Unable to determine schema type from version {check.SchemaVersion}");
        }

        [PublicAPI]
        public async Task<string> GetManifestRawAsync(
            string name,
            string reference,
            CancellationToken cancellationToken)
        {
            var headers = new Dictionary<string, string>
            {
                {
                    "Accept",
                    $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"
                }
            };

            var response = await this._client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Get,
                $"v2/{name}/manifests/{reference}",
                null,
                headers).ConfigureAwait(false);

            return response.Body;
        }

        private class SchemaCheck
        {
            /// <summary>
            ///     This field specifies the image manifest schema version as an integer.
            /// </summary>
            [DataMember(Name = "schemaVersion")]
            public int? SchemaVersion { get; set; }

            [DataMember(Name = "mediaType")]
            public string MediaType { get; set; }
        }
    }
}