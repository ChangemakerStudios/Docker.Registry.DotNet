# ![docker icon](https://raw.githubusercontent.com/ChangemakerStudios/Docker.Registry.DotNet/develop/assets/docker_registry_dotnet-24p.png) Docker.Registry.DotNet

[![NuGet version](https://badge.fury.io/nu/Docker.Registry.DotNet.svg)](https://badge.fury.io/nu/Docker.Registry.DotNet)
[![Build and Push to Nuget](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml/badge.svg)](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml)
[![Downloads](https://img.shields.io/nuget/dt/Docker.Registry.DotNet.svg?logo=nuget&color=purple)](https://www.nuget.org/packages/Docker.Registry.DotNet)

.NET (C#) Client Library for interacting with the Docker Registry API (v2).

## Table of Contents
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Authentication](#authentication)
- [Core Operations](#core-operations)
  - [Catalog Operations](#catalog-operations)
  - [Tag Operations](#tag-operations)
  - [Manifest Operations](#manifest-operations)
  - [Blob Operations](#blob-operations)
  - [Blob Upload Operations](#blob-upload-operations)
- [Real-World Scenarios](#real-world-scenarios)
- [Registry-Specific Examples](#registry-specific-examples)
- [Configuration & Advanced Topics](#configuration--advanced-topics)
- [Error Handling](#error-handling)
- [Troubleshooting](#troubleshooting)
- [API Reference](#api-reference)
- [Migration Guide](#migration-guide)
- [Contributing](#contributing)
- [License](#license)

# Installation

## Package Manager (Visual Studio)
```powershell
PM> Install-Package Docker.Registry.DotNet
```

## .NET CLI
```bash
dotnet add package Docker.Registry.DotNet
```

## Package Reference (.csproj)
```xml
<PackageReference Include="Docker.Registry.DotNet" Version="2.0.0" />
```

## Supported Frameworks
- .NET Standard 2.0
- .NET 5.0, 6.0, 7.0, 8.0+

# Quick Start

### Local Registry (No Authentication)

```csharp
using Docker.Registry.DotNet;

var configuration = new RegistryClientConfiguration("http://localhost:5000");

using var client = configuration.CreateClient();

// Get catalog of repositories
var catalog = await client.Catalog.GetCatalog();

foreach (var repo in catalog.Repositories)
{
    Console.WriteLine($"Repository: {repo}");

    // List tags for each repository
    var tags = await client.Tags.ListTags(repo);
    foreach (var tag in tags.Tags)
    {
        Console.WriteLine($"  Tag: {tag}");
    }
}
```

### Remote Registry with Authentication

```csharp
var configuration = new RegistryClientConfiguration("https://registry.mycompany.com");
configuration.UsePasswordOAuthAuthentication("username", "password");

using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();
```

### Docker Hub

```csharp
var configuration = new RegistryClientConfiguration("https://hub.docker.com");

using var client = configuration.CreateClient();

// List tags for a specific repository
var tags = await client.Repository.ListRepositoryTags("grafana", "loki-docker-driver");

foreach (var tag in tags.Tags)
{
    Console.WriteLine($"{tag.Name} - Last Updated: {tag.LastUpdated}");
}
```

# Authentication

Docker.Registry.DotNet supports multiple authentication methods depending on your registry configuration.

## Anonymous OAuth Authentication (Default)

Use this for registries that allow anonymous access or require OAuth tokens without credentials.

```csharp
var configuration = new RegistryClientConfiguration("https://registry-1.docker.io");
configuration.UseAnonymousOAuthAuthentication();

using var client = configuration.CreateClient();
```

## Basic Authentication

Use this for registries that support HTTP Basic Authentication.

```csharp
var configuration = new RegistryClientConfiguration("https://registry.mycompany.com");
configuration.UseBasicAuthentication("username", "password");

using var client = configuration.CreateClient();
```

## Password OAuth Authentication

Use this for registries that require OAuth token-based authentication (most common for private registries).

```csharp
var configuration = new RegistryClientConfiguration("https://registry.mycompany.com");
configuration.UsePasswordOAuthAuthentication("username", "password");

using var client = configuration.CreateClient();
```

## Authentication for Different Registries

| Registry | Authentication Method |
|----------|----------------------|
| Docker Hub | `UseAnonymousOAuthAuthentication()` (public) or `UsePasswordOAuthAuthentication()` (private) |
| Azure Container Registry (ACR) | `UseBasicAuthentication()` with username and access token |
| Amazon ECR | Use AWS credentials to obtain a token, then `UseBasicAuthentication("AWS", token)` |
| Harbor | `UseBasicAuthentication()` or `UsePasswordOAuthAuthentication()` |
| GitLab Container Registry | `UseBasicAuthentication()` with personal access token |
| Local Registry | Often no authentication or `UseBasicAuthentication()` |

# Core Operations

## Catalog Operations

### Get All Repositories

```csharp
using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();

foreach (var repository in catalog.Repositories)
{
    Console.WriteLine(repository);
}
```

### Get Repositories with Pagination

```csharp
var parameters = new CatalogParameters
{
    Number = 100  // Number of repositories per page
};

var catalog = await client.Catalog.GetCatalog(parameters);

// Check if there are more results
if (!string.IsNullOrEmpty(catalog.Link))
{
    Console.WriteLine($"More results available at: {catalog.Link}");
}
```

### Iterate Through All Repositories (Paginated)

```csharp
var allRepositories = new List<string>();
CatalogParameters? parameters = new CatalogParameters { Number = 100 };

do
{
    var catalog = await client.Catalog.GetCatalog(parameters);
    allRepositories.AddRange(catalog.Repositories);

    // Check if there's a next page
    if (string.IsNullOrEmpty(catalog.Last))
        break;

    parameters = new CatalogParameters
    {
        Number = 100,
        Last = catalog.Last  // Use last repository as marker
    };

} while (true);

Console.WriteLine($"Total repositories: {allRepositories.Count}");
```

## Tag Operations

### List Tags for a Repository

```csharp
var tags = await client.Tags.ListTags("myapp");

foreach (var tag in tags.Tags)
{
    Console.WriteLine(tag);
}
```

### List Tags with Pagination

```csharp
var parameters = new ListTagsParameters
{
    Number = 50  // Tags per page
};

var tags = await client.Tags.ListTags("myapp", parameters);
```

### List Tags by Digest (Get All Tags for a Specific Image)

This is useful to find all tags pointing to the same image digest.

```csharp
var tagsByDigest = await client.Tags.ListTagsByDigests("myapp");

foreach (var digest in tagsByDigest.Manifests)
{
    Console.WriteLine($"Digest: {digest.Digest}");
    Console.WriteLine($"Tags: {string.Join(", ", digest.Tags)}");
    Console.WriteLine($"MediaType: {digest.MediaType}");
    Console.WriteLine($"Size: {digest.Size} bytes");
    Console.WriteLine();
}
```

## Manifest Operations

### Get Image Manifest

```csharp
var manifestResult = await client.Manifest.GetManifest("myapp", "latest");

Console.WriteLine($"MediaType: {manifestResult.MediaType}");
Console.WriteLine($"Digest: {manifestResult.DockerContentDigest}");

// Access manifest details
if (manifestResult.Manifest is ImageManifest2_2 manifest)
{
    Console.WriteLine($"Architecture: {manifest.Config.Platform?.Architecture}");
    Console.WriteLine($"OS: {manifest.Config.Platform?.OS}");

    // Access layers
    foreach (var layer in manifest.Layers)
    {
        Console.WriteLine($"Layer: {layer.Digest}, Size: {layer.Size} bytes");
    }
}
```

### Get Full Manifest with History

To access complete manifest information including history:

```csharp
var manifestResult = await client.Manifest.GetManifest("myapp", "latest");

if (manifestResult.Manifest is ImageManifest2_2 manifest)
{
    // Get the config blob which contains history
    var configBlob = await client.Blobs.GetBlob("myapp", manifest.Config.Digest);

    using var reader = new StreamReader(configBlob.Stream);
    var configJson = await reader.ReadToEndAsync();

    // Parse config JSON to access history
    var config = JsonConvert.DeserializeObject<ImageConfig>(configJson);

    foreach (var historyEntry in config.History)
    {
        Console.WriteLine($"Created: {historyEntry.Created}");
        Console.WriteLine($"Command: {historyEntry.CreatedBy}");
        Console.WriteLine();
    }
}
```

### Get Raw Manifest (as JSON string)

```csharp
var rawManifest = await client.Manifest.GetManifestRaw("myapp", "latest");
Console.WriteLine(rawManifest);
```

### Get Manifest Digest

```csharp
var digest = await client.Manifest.GetDigest("myapp", "latest");
Console.WriteLine($"Digest: {digest}");
```

### Push/Upload a Manifest

```csharp
var manifest = new ImageManifest2_2
{
    SchemaVersion = 2,
    MediaType = "application/vnd.docker.distribution.manifest.v2+json",
    Config = new Descriptor
    {
        MediaType = "application/vnd.docker.container.image.v1+json",
        Digest = configDigest,
        Size = configSize
    },
    Layers = new List<Descriptor>
    {
        new Descriptor
        {
            MediaType = "application/vnd.docker.image.rootfs.diff.tar.gzip",
            Digest = layerDigest,
            Size = layerSize
        }
    }
};

var response = await client.Manifest.PutManifest("myapp", "v1.0.0", manifest);
Console.WriteLine($"Manifest uploaded with digest: {response.DockerContentDigest}");
```

### Delete a Manifest

```csharp
// Delete by digest (recommended to avoid deleting wrong manifest)
var digest = await client.Manifest.GetDigest("myapp", "latest");
await client.Manifest.DeleteManifest("myapp", digest);

Console.WriteLine($"Deleted manifest with digest: {digest}");
```

## Blob Operations

### Download a Blob (Layer)

```csharp
var manifestResult = await client.Manifest.GetManifest("myapp", "latest");

if (manifestResult.Manifest is ImageManifest2_2 manifest)
{
    var firstLayer = manifest.Layers.First();
    var blob = await client.Blobs.GetBlob("myapp", firstLayer.Digest);

    Console.WriteLine($"Content Type: {blob.ContentType}");
    Console.WriteLine($"Digest: {blob.DockerContentDigest}");

    // Save blob to file
    using var fileStream = File.Create($"layer-{firstLayer.Digest}.tar.gz");
    await blob.Stream.CopyToAsync(fileStream);
}
```

### Check if Blob Exists

```csharp
var exists = await client.Blobs.BlobExists("myapp", "sha256:abc123...");

if (exists)
{
    Console.WriteLine("Blob exists in registry");
}
```

### Delete a Blob

```csharp
await client.Blobs.DeleteBlob("myapp", "sha256:abc123...");
Console.WriteLine("Blob deleted");
```

## Blob Upload Operations

### Monolithic Upload (Simple Upload)

For smaller blobs, use a monolithic upload:

```csharp
using var fileStream = File.OpenRead("my-layer.tar.gz");

// Calculate digest
using var sha256 = SHA256.Create();
var hash = await sha256.ComputeHashAsync(fileStream);
var digest = $"sha256:{BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()}";
fileStream.Position = 0;

// Start upload
var upload = await client.BlobUploads.StartUploadBlob("myapp");

// Complete upload in one request
var imageDigest = new ImageDigest(digest);
var result = await client.BlobUploads.MonolithicUploadBlob(
    upload,
    imageDigest,
    fileStream
);

Console.WriteLine($"Uploaded blob: {result.DockerContentDigest}");
```

### Chunked Upload (For Large Files)

For larger blobs, use chunked uploads with resumable capability:

```csharp
using var fileStream = File.OpenRead("large-layer.tar.gz");

// Calculate final digest
var digest = CalculateSHA256(fileStream);
fileStream.Position = 0;

// Start upload session
var upload = await client.BlobUploads.StartUploadBlob("myapp");

const int chunkSize = 5 * 1024 * 1024; // 5 MB chunks
var buffer = new byte[chunkSize];
long position = 0;

while (position < fileStream.Length)
{
    var bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize);
    using var chunkStream = new MemoryStream(buffer, 0, bytesRead);

    var from = position;
    var to = position + bytesRead - 1;

    // Upload chunk
    upload = await client.BlobUploads.UploadBlobChunk(
        upload,
        chunkStream,
        from,
        to
    );

    position += bytesRead;
    Console.WriteLine($"Uploaded {position}/{fileStream.Length} bytes");
}

// Complete the upload
var imageDigest = new ImageDigest(digest);
var result = await client.BlobUploads.CompleteBlobUpload(upload, imageDigest);

Console.WriteLine($"Upload complete: {result.DockerContentDigest}");
```

### Mount a Blob from Another Repository

Copy a blob from one repository to another without re-uploading:

```csharp
var parameters = new MountParameters
{
    From = "source-repo",
    Mount = "sha256:abc123..."  // Blob digest to mount
};

var result = await client.BlobUploads.MountBlob("destination-repo", parameters);

if (result.DockerUploadUuid == null)
{
    Console.WriteLine("Blob mounted successfully!");
}
else
{
    Console.WriteLine("Mount not supported, need to upload blob");
}
```

### Cancel an Upload

```csharp
var upload = await client.BlobUploads.StartUploadBlob("myapp");

// ... something goes wrong ...

await client.BlobUploads.CancelBlobUpload("myapp", upload.UploadUuid);
Console.WriteLine("Upload cancelled");
```

# Real-World Scenarios

## Copy an Image Between Registries

```csharp
var sourceConfig = new RegistryClientConfiguration("https://source-registry.com");
sourceConfig.UsePasswordOAuthAuthentication("user1", "pass1");

var destConfig = new RegistryClientConfiguration("https://dest-registry.com");
destConfig.UsePasswordOAuthAuthentication("user2", "pass2");

using var sourceClient = sourceConfig.CreateClient();
using var destClient = destConfig.CreateClient();

// Get manifest from source
var manifest = await sourceClient.Manifest.GetManifest("myapp", "v1.0");

if (manifest.Manifest is ImageManifest2_2 imageManifest)
{
    // Copy config blob
    var configBlob = await sourceClient.Blobs.GetBlob("myapp", imageManifest.Config.Digest);
    var configUpload = await destClient.BlobUploads.StartUploadBlob("myapp");
    await destClient.BlobUploads.MonolithicUploadBlob(
        configUpload,
        new ImageDigest(imageManifest.Config.Digest),
        configBlob.Stream
    );

    // Copy each layer
    foreach (var layer in imageManifest.Layers)
    {
        var layerBlob = await sourceClient.Blobs.GetBlob("myapp", layer.Digest);
        var layerUpload = await destClient.BlobUploads.StartUploadBlob("myapp");
        await destClient.BlobUploads.MonolithicUploadBlob(
            layerUpload,
            new ImageDigest(layer.Digest),
            layerBlob.Stream
        );
    }

    // Push manifest to destination
    await destClient.Manifest.PutManifest("myapp", "v1.0", imageManifest);

    Console.WriteLine("Image copied successfully!");
}
```

## List All Tags Across All Repositories

```csharp
using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();

foreach (var repo in catalog.Repositories)
{
    var tags = await client.Tags.ListTags(repo);

    foreach (var tag in tags.Tags)
    {
        Console.WriteLine($"{repo}:{tag}");
    }
}
```

## Download All Layers of an Image

```csharp
using var client = configuration.CreateClient();

var manifest = await client.Manifest.GetManifest("myapp", "latest");

if (manifest.Manifest is ImageManifest2_2 imageManifest)
{
    Directory.CreateDirectory("layers");

    int layerIndex = 0;
    foreach (var layer in imageManifest.Layers)
    {
        Console.WriteLine($"Downloading layer {layerIndex}: {layer.Digest}");

        var blob = await client.Blobs.GetBlob("myapp", layer.Digest);
        var fileName = $"layers/layer-{layerIndex:D3}.tar.gz";

        using var fileStream = File.Create(fileName);
        await blob.Stream.CopyToAsync(fileStream);

        Console.WriteLine($"Saved to {fileName} ({layer.Size} bytes)");
        layerIndex++;
    }
}
```

## Clean Up Old Image Tags

```csharp
using var client = configuration.CreateClient();

var cutoffDate = DateTime.UtcNow.AddMonths(-6);
var tagsToDelete = new List<string>();

var tagsByDigest = await client.Tags.ListTagsByDigests("myapp");

foreach (var manifest in tagsByDigest.Manifests)
{
    // Note: You'd need to get the actual creation date from the manifest
    // This is a simplified example
    foreach (var tag in manifest.Tags)
    {
        if (tag.StartsWith("temp-") || tag.Contains("-old"))
        {
            tagsToDelete.Add(tag);
        }
    }
}

foreach (var tag in tagsToDelete)
{
    var digest = await client.Manifest.GetDigest("myapp", tag);
    if (digest != null)
    {
        await client.Manifest.DeleteManifest("myapp", digest);
        Console.WriteLine($"Deleted tag: {tag}");
    }
}
```

# Registry-Specific Examples

## Docker Hub

### Public Repository
```csharp
var configuration = new RegistryClientConfiguration("https://hub.docker.com");

using var client = configuration.CreateClient();

var tags = await client.Repository.ListRepositoryTags("library", "nginx");
```

### Private Repository
```csharp
var configuration = new RegistryClientConfiguration("https://proget.mycompany.com");
configuration.UsePasswordOAuthAuthentication("username", "password");

using var client = configuration.CreateClient();

var tags = await client.Tags.ListTags("myapp");
```

## Azure Container Registry (ACR)

```csharp
var configuration = new RegistryClientConfiguration("https://myregistry.azurecr.io");

// Use the admin username and password, or a service principal
configuration.UseBasicAuthentication("username", "password");

using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();
```

## Amazon ECR

```csharp
// First, get an authorization token using AWS SDK
// var ecrClient = new AmazonECRClient();
// var authResponse = await ecrClient.GetAuthorizationTokenAsync(...);
// var token = DecodeBase64(authResponse.AuthorizationData[0].AuthorizationToken);

var configuration = new RegistryClientConfiguration("https://123456789012.dkr.ecr.us-east-1.amazonaws.com");
configuration.UseBasicAuthentication("AWS", token);

using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();
```

## Harbor

```csharp
var configuration = new RegistryClientConfiguration("https://harbor.mycompany.com");
configuration.UseBasicAuthentication("admin", "Harbor12345");

using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();
```

## GitLab Container Registry

```csharp
var configuration = new RegistryClientConfiguration("https://registry.gitlab.com");

// Use personal access token or deploy token
configuration.UseBasicAuthentication("gitlab-ci-token", "your-token");

using var client = configuration.CreateClient();

var tags = await client.Tags.ListTags("mygroup/myproject");
```

## Local Development Registry

```csharp
// No authentication typically required
var configuration = new RegistryClientConfiguration("http://localhost:5000");

using var client = configuration.CreateClient();

var catalog = await client.Catalog.GetCatalog();
```

# Configuration & Advanced Topics

## Custom HttpClient

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(10)
};

var configuration = new RegistryClientConfiguration("https://registry.mycompany.com");
// Note: The library creates its own HttpClient internally
// For advanced scenarios, consider creating a custom HttpMessageHandler
```

## Working with Self-Signed Certificates

```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        // WARNING: Only use this in development/testing environments
        return true; // Accept all certificates
    }
};

// For production, properly validate the certificate
// or add it to your system's trusted certificate store
```

## ActivitySource Support (Telemetry/Tracing)

The library includes built-in ActivitySource support for distributed tracing:

```csharp
using System.Diagnostics;

// Create an ActivityListener to capture traces
var listener = new ActivityListener
{
    ShouldListenTo = source => source.Name == "Docker.Registry.DotNet",
    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
    ActivityStarted = activity => Console.WriteLine($"Started: {activity.DisplayName}"),
    ActivityStopped = activity => Console.WriteLine($"Stopped: {activity.DisplayName} ({activity.Duration})")
};

ActivitySource.AddActivityListener(listener);

// Now all registry operations will be traced
using var client = configuration.CreateClient();
var catalog = await client.Catalog.GetCatalog();
```

## Proxy Configuration

```csharp
var handler = new HttpClientHandler
{
    Proxy = new WebProxy("http://proxy.company.com:8080"),
    UseProxy = true
};

// Use the handler with your configuration
```

# Error Handling

## Handling Unauthorized Exceptions

```csharp
using Docker.Registry.DotNet.Domain.Registry;

try
{
    var catalog = await client.Catalog.GetCatalog();
}
catch (UnauthorizedApiException ex)
{
    Console.WriteLine("Authentication failed!");
    Console.WriteLine($"Status Code: {ex.StatusCode}");

    // Inspect WWW-Authenticate headers for more details
    foreach (var header in ex.Headers)
    {
        Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
    }
}
```

## Handling General API Exceptions

```csharp
try
{
    var manifest = await client.Manifest.GetManifest("myapp", "nonexistent-tag");
}
catch (DockerApiException ex)
{
    Console.WriteLine($"API Error: {ex.StatusCode}");
    Console.WriteLine($"Message: {ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
```

## Retry Logic with Polly

```csharp
using Polly;

var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TaskCanceledException>()
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

await retryPolicy.ExecuteAsync(async () =>
{
    var catalog = await client.Catalog.GetCatalog();
});
```

## Proper Disposal

```csharp
// Use 'using' statement to ensure proper disposal
using (var client = configuration.CreateClient())
{
    var catalog = await client.Catalog.GetCatalog();
    // Client is disposed here
}

// Or with 'using' declaration (C# 8.0+)
using var client = configuration.CreateClient();
var catalog = await client.Catalog.GetCatalog();
// Client is disposed at end of scope
```

# Troubleshooting

## Issue: "Unable to read manifest with provenance enabled image"

**Problem:** Images built with Docker BuildKit's provenance feature (`--provenance true`) may fail to retrieve manifests.

**Cause:** BuildKit creates manifests with OCI media types that may not be fully supported in older versions.

**Solution:**
1. Build images with `--provenance false` for compatibility:
   ```bash
   docker build --provenance false -t myimage:latest .
   ```

2. Or ensure you're using the latest version of Docker.Registry.DotNet which includes improved OCI support.

## Issue: "405 Method Not Allowed" During Authentication

**Problem:** Authentication fails with a 405 error.

**Cause:** The registry may not support the authentication method being used.

**Solution:**
1. Try different authentication methods:
   ```csharp
   // Try Basic Authentication instead of OAuth
   configuration.UseBasicAuthentication("username", "password");

   // Or try Anonymous OAuth
   configuration.UseAnonymousOAuthAuthentication();
   ```

2. Verify the registry supports the Docker Registry API v2.

## Issue: "401 Unauthorized" Errors

**Problem:** Getting 401 errors even with correct credentials.

**Cause:** Various authentication issues.

**Solutions:**
- Verify credentials are correct
- Check if user has permissions for the repository
- For Docker Hub, use your username (not email) and an access token (not password)
- For cloud registries (ACR, ECR, GCR), ensure you're using the correct authentication method

## Issue: SSL/TLS Certificate Errors

**Problem:** `The SSL connection could not be established`

**Solution:**
```csharp
// For development only - accept self-signed certificates
// DO NOT use in production!
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
```

For production, add the certificate to your system's trusted certificate store.

## Issue: Timeout on Large Image Operations

**Problem:** Operations timeout when working with large images.

**Solution:**
Increase timeout and use chunked uploads:
```csharp
// The library handles timeouts internally
// For very large operations, use chunked uploads instead of monolithic uploads
```

## Issue: "Manifest Not Found" for Valid Tags

**Problem:** Getting 404 errors for tags that exist.

**Cause:** Registry may be using a different API endpoint or authentication scope.

**Solution:**
- Verify the repository name format (some registries use `namespace/repository`)
- Check authentication scopes include the repository
- Try using `Repository.ListRepositoryTags()` for Docker Hub instead of `Tags.ListTags()`

# API Reference

The `IRegistryClient` interface provides access to all registry operations:

## Available Operations

### `client.Manifest`
Manifest operations for working with image manifests.
- `GetManifest()` - Get an image manifest
- `GetManifestRaw()` - Get raw manifest JSON
- `GetDigest()` - Get manifest digest
- `PutManifest()` - Upload/push a manifest
- `DeleteManifest()` - Delete a manifest

### `client.Catalog`
Catalog operations for listing repositories.
- `GetCatalog()` - Get list of repositories

### `client.Blobs`
Blob operations for downloading image layers.
- `GetBlob()` - Download a blob/layer
- `BlobExists()` - Check if a blob exists
- `DeleteBlob()` - Delete a blob

### `client.BlobUploads`
Blob upload operations for pushing image layers.
- `StartUploadBlob()` - Start an upload session
- `MonolithicUploadBlob()` - Upload blob in one request
- `UploadBlobChunk()` - Upload a chunk (for resumable uploads)
- `CompleteBlobUpload()` - Complete a chunked upload
- `CancelBlobUpload()` - Cancel an upload
- `MountBlob()` - Mount blob from another repository
- `GetBlobUploadStatus()` - Get status of resumable upload

### `client.Tags`
Tag operations for working with image tags.
- `ListTags()` - List tags for a repository
- `ListTagsByDigests()` - List tags grouped by digest (shows all tags for each image)

### `client.Repository`
Docker Hub specific repository operations.
- `ListRepositoryTags()` - List tags for a Docker Hub repository

### `client.System`
System operations.
- Operations for registry health checks and version info

# Migration Guide

## Migrating from v1.x to v2.x

Version 2.0 introduced several breaking changes for a more modern .NET API:

### Method Names Changed (Removed "Async" Suffix)

**v1.x:**
```csharp
var catalog = await client.Catalog.GetCatalogAsync();
var tags = await client.Tags.ListImageTagsAsync("myapp", new ListImageTagsParameters());
```

**v2.x:**
```csharp
var catalog = await client.Catalog.GetCatalog();
var tags = await client.Tags.ListTags("myapp", new ListTagsParameters());
```

**Note:** Legacy methods with `Async` suffix are still available but marked as `[Obsolete]` to help with migration.

### Configuration Builder Pattern

**v1.x:**
```csharp
var configuration = new RegistryClientConfiguration("https://registry.com");
var client = new RegistryClient(configuration);
```

**v2.x (Recommended):**
```csharp
var configuration = new RegistryClientConfiguration("https://registry.com");
configuration.UsePasswordOAuthAuthentication("user", "pass");

using var client = configuration.CreateClient();
```

### Authentication Setup Simplified

Authentication configuration is now done through extension methods:

```csharp
// Basic Authentication
configuration.UseBasicAuthentication("username", "password");

// Password OAuth
configuration.UsePasswordOAuthAuthentication("username", "password");

// Anonymous OAuth
configuration.UseAnonymousOAuthAuthentication();
```

### Framework Support

v2.x adds direct support for:
- .NET 5.0
- .NET 6.0
- .NET 7.0
- .NET 8.0

While maintaining .NET Standard 2.0 support for maximum compatibility.

### New Features in v2.x

- ActivitySource support for distributed tracing
- Improved error handling
- Docker Hub registry support via `Repository` operations
- `ListTagsByDigests()` to get all tags for specific images
- Better nullability annotations
- Improved async/await patterns

# Contributing

We welcome contributions!

## How to Contribute

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Reporting Issues

If you find a bug or have a feature request, please [open an issue](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/issues) on GitHub.

# License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

Copyright © Rich Quackenbush, Jaben Cargman and the Docker.Registry.DotNet Contributors 2017-2024

---

**Questions?** Check out the [sample projects](samples/) or [open an issue](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/issues)!
