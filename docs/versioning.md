# LiteTick versioning and release identity

LiteTick is an external product. Every distributed application bundle must have an unambiguous, auditable identity.

## Standards

- Product versions use Semantic Versioning core format: `MAJOR.MINOR.PATCH`.
- The macOS `CFBundleShortVersionString` is exactly the product version and therefore always contains three period-separated integers.
- The macOS `CFBundleVersion` is a positive, monotonically increasing integer. It identifies a specific build and must increase before any build is distributed.
- Released artifacts are immutable. A changed artifact always receives a new build number, and a changed public release receives the appropriate new product version.
- User-facing changes are curated in `CHANGELOG.md` using Keep a Changelog 1.1.0.

## Single source of truth

`apps/macos/Support/Version.plist` is the only editable source for:

- `MarketingVersion` — current `MAJOR.MINOR.PATCH` product version.
- `BuildNumber` — globally increasing macOS build number.
- `ReleaseChannel` — one of `development`, `alpha`, `beta`, `rc`, or `stable`.

Do not add editable version values back to `Support/Info.plist`. The build script injects version metadata into the generated bundle and verifies the result after signing.

`apps/windows/Support/Version.props` is the corresponding sole editable Windows version source. Windows builds use the same SemVer and release-channel meanings with their own monotonically increasing build sequence. `apps/windows/scripts/build.ps1` is the native release build entry point; `build-cross.sh` produces a macOS cross-built PE candidate for structural inspection.

## Version meaning

- `0.y.z` — initial development; behavior and stored-data contracts may still change.
- `1.0.0` — first stable public contract.
- `MAJOR` — incompatible change to documented behavior or persisted-data compatibility.
- `MINOR` — backward-compatible user-facing capability or substantial improvement.
- `PATCH` — backward-compatible correction with no new product capability.

The public contract includes documented interaction behavior, supported local-data compatibility, command-line/build interfaces intended for contributors, and published privacy/storage guarantees.

## Build workflow

Show the current identity:

```bash
apps/macos/scripts/version.sh show
```

Build the macOS application:

```bash
cd apps/macos
./scripts/build-app.sh
```

Every invocation reserves the next build number before compilation. Failed builds may leave gaps; build numbers must never be reused.

Set a new version explicitly:

```bash
apps/macos/scripts/version.sh set 0.2.0 25 beta
```

The supplied build number must be greater than the current one. Never lower or reset it when changing the product version.

## Release gate

Before any artifact is distributed:

1. Select `alpha`, `beta`, `rc`, or `stable`; `development` cannot pass the release gate.
2. Move curated changes from `Unreleased` into a dated `## [MAJOR.MINOR.PATCH] - YYYY-MM-DD` section.
3. Build the final candidate so it receives a unique build number.
4. Run `apps/macos/scripts/release-check.sh`.
5. Confirm signing/notarization, privacy text, migration behavior, supported macOS version, checksums, and release notes.
6. Obtain explicit user approval before publishing, tagging, deploying, or creating external release resources.

For Windows, apply the same gate using `apps/windows/Support/Version.props`, build the accepted x64 EXE, run core and PE verification, record SHA-256, confirm native Windows acceptance for every claimed Windows version, and obtain explicit publication approval.

Git tags will use `vMAJOR.MINOR.PATCH` for stable releases and a SemVer prerelease suffix for prereleases after Git is explicitly initialized. No Git action is implied by this policy.

## Required records

- `CHANGELOG.md` — curated changes for users.
- `progress.md` — detailed internal work history.
- `decision.md` — rationale and trade-offs.
- `release/` — local release procedures and, later, explicitly approved release manifests.
- Hosted release notes — derived from the matching changelog entry, never used as the sole historical record.
