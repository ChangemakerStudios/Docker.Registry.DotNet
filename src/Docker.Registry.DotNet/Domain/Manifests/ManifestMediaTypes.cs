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
// https://docs.docker.com/registry/spec/manifest-v2-1/#signed-manifests

public static class ManifestMediaTypes
{
    /// <summary>
    ///     schema1 (existing manifest format). Note that “application/json” will also be accepted for schema 1.
    /// </summary>
    public const string ManifestSchema1 =
        "application/vnd.docker.distribution.manifest.v1+json";

    /// <summary>
    ///     schema1 (existing manifest format) signed.
    /// </summary>
    public const string ManifestSchema1Signed =
        "application/vnd.docker.distribution.manifest.v1+prettyjws";

    /// <summary>
    ///     New image manifest format (schemaVersion = 2)
    /// </summary>
    public const string ManifestSchema2 =
        "application/vnd.docker.distribution.manifest.v2+json";

    /// <summary>
    ///     Manifest list, aka “fat manifest”
    /// </summary>
    public const string ManifestList =
        "application/vnd.docker.distribution.manifest.list.v2+json";

    /// <summary>
    ///     OCI image manifest (structurally equivalent to schema2). Used by buildkit,
    ///     e.g. when building with provenance/SBOM attestations enabled.
    /// </summary>
    public const string OciManifest = "application/vnd.oci.image.manifest.v1+json";

    /// <summary>
    ///     OCI image index (structurally equivalent to the manifest list).
    /// </summary>
    public const string OciIndex = "application/vnd.oci.image.index.v1+json";

    /// <summary>
    ///     OCI container config JSON
    /// </summary>
    public const string OciContainerConfig = "application/vnd.oci.image.config.v1+json";

    /// <summary>
    ///     Container config JSON
    /// </summary>
    public const string ContainerConfig = "application/vnd.docker.container.image.v1+json";

    /// <summary>
    ///     “Layer”, as a gzipped tar
    /// </summary>
    public const string GzippedTar = "application/vnd.docker.image.rootfs.diff.tar.gzip";

    /// <summary>
    ///     “Layer”, as a gzipped tar that should never be pushed
    /// </summary>
    public const string GzippedLayer =
        "application/vnd.docker.image.rootfs.foreign.diff.tar.gzip";

    /// <summary>
    ///     Plugin config JSON
    /// </summary>
    public const string PluginConfigJson = "application/vnd.docker.plugin.v1+json";
}