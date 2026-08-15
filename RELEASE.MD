# Release Process

This project follows [Gitflow](https://nvie.com/posts/a-successful-git-branching-model/) and uses [GitVersion](https://gitversion.net/) (ContinuousDelivery mode) for automatic semantic versioning. NuGet publishing is handled by GitHub Actions on push to `master`.

## Branch Overview

| Branch | Purpose |
|---|---|
| `master` | Production releases. Pushes here trigger NuGet publish. |
| `develop` | Integration branch for the next release. |
| `feature/*` | Feature branches off `develop`. |
| `fix/*` | Bug fix branches off `develop`. |
| `release/*` | Release stabilization branches off `develop`. |

> **Important:** Pull requests for features and fixes should target `develop`, not `master`. Any push to `master` (including a PR merge) triggers a NuGet publish attempt.

## Release Steps

### 1. Ensure `develop` is ready

- All feature/fix branches for the release are merged into `develop`.
- CI is green on `develop`.

### 2. Create a release branch

```bash
git checkout develop
git pull origin develop
git checkout -b release/X.Y.Z
```

### 3. Finalize the release

On the release branch, make release-only changes in `src/Docker.Registry.DotNet/Docker.Registry.DotNet.csproj`:

- **Update `<TargetFrameworks>`** if adding/dropping TFMs.
- **Update `<Version>`** to match the release. (CI overrides the package version with GitVersion's `semVer`, but keeping this in sync helps local builds.)
- **Update `<PackageReleaseNotes>`** with a summary of the release.
- **Update `<Description>`** if the library's capabilities have changed.
- **Update `README.md`** with any new/changed usage examples and troubleshooting entries.
- Build and run tests locally:

```bash
dotnet build
dotnet test test/Docker.Registry.DotNet.Tests/Docker.Registry.DotNet.Tests.csproj
```

### 4. Merge release branch into `master`

```bash
git checkout master
git pull origin master
git merge --no-ff release/X.Y.Z
git tag X.Y.Z
git push origin master --tags
```

This push triggers the GitHub Actions workflow which will:
1. Calculate the package version via GitVersion.
2. Build and run the test suite with code coverage.
3. Pack the NuGet package (with symbols, `snupkg`).
4. Publish to nuget.org.

### 5. Merge release branch back into `develop`

```bash
git checkout develop
git pull origin develop
git merge --no-ff release/X.Y.Z
git push origin develop
```

### 6. Clean up

```bash
git branch -d release/X.Y.Z
git push origin --delete release/X.Y.Z
```

### 7. Create a GitHub Release

- Go to **Releases** > **Draft a new release**.
- Choose the `X.Y.Z` tag.
- Title: `X.Y.Z`
- Body: List breaking changes, features, bug fixes, and documentation changes (reference the merged PRs/issues).
- Publish.

## Hotfix Process

For critical fixes to a released version:

```bash
git checkout master
git checkout -b hotfix/X.Y.Z+1
# make fix, commit
git checkout master
git merge --no-ff hotfix/X.Y.Z+1
git tag X.Y.Z+1
git push origin master --tags

git checkout develop
git merge --no-ff hotfix/X.Y.Z+1
git push origin develop

git branch -d hotfix/X.Y.Z+1
```

## CI/CD Details

- **Workflow**: `.github/workflows/publish.yml`
- **Triggers**: All pushes and pull requests (build + test + coverage report). NuGet publish only on push to `master`.
- **Versioning**: GitVersion with `ContinuousDelivery` mode (`GitVersion.yml`). Versions are derived from tags on `master`, so always tag releases.
- **NuGet API key**: Stored as the `NUGETKEY` repository secret. If the Publish step fails with `403 (The specified API key is invalid, has expired, ...)`, generate a new key at nuget.org, update the secret, and re-run the failed workflow run (`gh run rerun <run-id>`).
