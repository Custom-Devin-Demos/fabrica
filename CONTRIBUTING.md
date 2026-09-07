# Contributing to Fabrica

## Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`global.json` pins the build to the 8.0 SDK band, so newer installed SDKs are not used)
* `git` (the build reads version information from the repository history)

## Build and test

One command restores, builds and runs the test suite:

```sh
dotnet test Source/Fabrica.sln -c Release
```

Tests live in `Source/Fabrica.Test` and use NUnit; `dotnet test` discovers and runs them without any additional tooling. This is the same command that CI (`.github/workflows/ci.yml`) runs on every push and pull request.

## Produce a package

```sh
dotnet build Source/Fabrica.sln -c Release
dotnet pack Source/Fabrica/Fabrica.csproj --no-build -c Release -o artifacts
```

The `GEAviation.Fabrica.<version>.nupkg` file is written to `artifacts/`.

## Versioning

Versions are derived from git by `Source/Versioning.targets` at build time; there is no version number to edit in the project file.

* If the current commit is tagged `v1.2.3` or `version/1.2.3`, the assembly and package version is `1.2.3` (any tag suffix such as `-beta` is carried through).
* Otherwise the version is `0.0.0-<short commit hash>`.
* If the working tree has uncommitted changes, `-dirty` is appended.

CI verifies that the built assembly and package carry a version in this format. To release, tag the commit and build from a clean checkout with full history (`git clone`, not a shallow clone).

## Branches and pull requests

* Work on a feature branch; open a pull request against `main`.
* CI must be green before review.
* Every pull request needs human approval; do not merge your own pull request.
* Structure the pull request description as: a short risk summary, then **Root cause**, **Fix**, **Verification** (commands run and results), **Risk and rollback**.
* Keep changes scoped; do not mix unrelated refactoring or formatting changes into a pull request.
