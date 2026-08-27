# Release workspace

This directory is reserved for LiteTick release material. It must not contain ordinary source or design work.

No artifact in this directory is approved for public distribution merely because it exists here.

For each approved release, use a directory named `vMAJOR.MINOR.PATCH/` containing, as applicable:

- signed and notarized application/archive;
- SHA-256 checksum manifest;
- release notes derived from `CHANGELOG.md`;
- signing/notarization verification output;
- compatibility, migration, privacy, and uninstall checks;
- a release manifest recording version, build number, channel, date, and artifact names.

Follow `docs/versioning.md` and pass `apps/macos/scripts/release-check.sh` before placing a candidate here. Publishing or creating external resources always requires explicit user approval.

