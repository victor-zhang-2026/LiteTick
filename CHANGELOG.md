# Changelog

All notable user-facing changes to LiteTick are documented in this file.

The format follows [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and version numbers follow [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.1] - 2026-09-04

### Fixed

- Allowed the floating trigger to move to any connected display, remain fully visible when released, and restore its saved secondary-display position on the next launch.

## [1.0.0] - 2026-08-27

### Added

- Native macOS floating checklist with local-only persistence, bilingual Simplified Chinese and English UI, Light and Dark appearances, task completion, searchable Completed view, inline editing, item pinning, and handle-based reordering.
- Automatic preservation of the last valid local task file and fallback recovery when the primary JSON cannot be decoded.
- Strict application version metadata, monotonically increasing build identities, bundle verification, and a release-readiness gate.
- Validation-first legacy data compatibility that preserves original task data.
- Production application icon and a minimal bilingual About window with live version, build, channel, privacy, license, and repository information.

### Changed

- Refined the panel into a compact, borderless desktop utility with stable Hover geometry, consistent linear header icons, fast bilingual help tags, and platform-neutral right-side Close placement.
- Standardized the list around square completion controls, straight contiguous rows, subtle separators, and restrained green state surfaces.
- Finalized the public product brand as “妥了 LiteTick”.

### Security

- Kept the application account-free, network-free, analytics-free, and fully offline; task data remains plaintext on the local Mac as documented in the privacy statement.
