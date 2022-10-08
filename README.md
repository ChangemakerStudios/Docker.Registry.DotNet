# ![docker icon](https://raw.githubusercontent.com/ChangemakerStudios/Docker.Registry.DotNet/develop/assets/docker_registry_dotnet-24p.png) Docker.Registry.DotNet

[![NuGet version](https://badge.fury.io/nu/Docker.Registry.DotNet.svg)](https://badge.fury.io/nu/Docker.Registry.DotNet)
[![Build and Push to Nuget](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml/badge.svg)](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml)
[![Downloads](https://img.shields.io/nuget/dt/Docker.Registry.DotNet.svg?logo=nuget&color=purple)](https://www.nuget.org/packages/Docker.Registry.DotNet)

.NET (C#) Client Library for interacting with a Docker Registry API (v2 only).

# Setup
*Install nuget package into your project via powershell:*
```powershell
PM> Install-Package Docker.Registry.DotNet
```

*Add nuget package via dotnet CLI:*
```
dotnet add package Docker.Registry.DotNet
```

# Usage
```csharp
var configuration = new RegistryClientConfiguration("localhost:5000");

using (var client = configuration.CreateClient())
{
    await client.System.PingAsync();
}
```

# Changelog

### v1.1.33
* Added Basic Authentication (thanks [Zguy](https://github.com/Zguy)).
* Fixed issue with operational parameters (thanks [lostllama](https://github.com/lostllama)).
* Fixed issue with large manifest layers (thanks [msvprogs](https://github.com/msvprogs)).
