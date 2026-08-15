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

namespace Docker.Registry.DotNet.Domain.Manifests;

public class GetImageManifestResult
{
    internal GetImageManifestResult(string mediaType, ImageManifest manifest, string content)
    {
        this.Manifest = manifest;
        this.Content = content;
        this.MediaType = mediaType;
    }

    public string? DockerContentDigest { get; internal set; }

    public string? Etag { get; internal set; }

    public string MediaType { get; }

    /// <summary>
    ///     The image manifest
    /// </summary>
    public ImageManifest Manifest { get; }

    /// <summary>
    ///     Gets the original, raw body returned from the server.
    /// </summary>
    public string Content { get; }
}