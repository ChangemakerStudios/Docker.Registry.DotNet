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

using System.Threading;
using System.Threading.Tasks;

using Docker.Registry.DotNet.Models;

using JetBrains.Annotations;

namespace Docker.Registry.DotNet.Endpoints;

/// <summary>
///     Manifest operations.
/// </summary>
public interface IManifestOperations
{
    /// <summary>
    ///     Delete the manifest.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteManifestAsync(
        string name,
        string reference,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Fetch the manifest identified by name and reference raw.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetManifestRawAsync(
        string name,
        string reference,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Fetch the manifest identified by name and reference where reference can be a tag or digest. A HEAD request can also
    ///     be issued to this endpoint to obtain resource information without receiving all data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task<GetImageManifestResult> GetManifestAsync(
        string name,
        string reference,
        CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Returns true if the image exists, false otherwise.
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="reference"></param>
    ///// <param name="cancellation"></param>
    ///// <returns></returns>
    //Task<bool> DoesManifestExistAsync(string name, string reference, CancellationToken cancellation = default);

    /// <summary>
    ///     Put the manifest identified by name and reference where reference can be a tag or digest.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="manifest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PushManifestResponse> PutManifestAsync(
        string name,
        string reference,
        ImageManifest manifest,
        CancellationToken cancellationToken = default);
}