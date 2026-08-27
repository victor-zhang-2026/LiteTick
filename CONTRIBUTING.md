# Contributing to 妥了 LiteTick

感谢参与。LiteTick 有意保持小而明确：一张本地清单，一个悬浮入口，不需要账号或网络。

Thank you for contributing. LiteTick intentionally stays small: one local list, one floating entry point, no account, and no network requirement.

## Before proposing a change

- Check existing issues before opening a new one.
- Describe the user problem before proposing implementation details.
- Keep the MVP boundary: no categories, tags, folders, deadlines, reminders, priorities, calendars, AI, analytics, accounts, servers, or synchronization.
- Discuss substantial product, storage-format, dependency, or platform changes before implementing them.
- Do not include task data, credentials, signing material, or personal filesystem paths.

## Development

Requirements: Apple silicon Mac, macOS 14 SDK, and Swift 6.

```bash
cd apps/macos
./scripts/build-app.sh
open build/LiteTick.app
```

Never change migration or persistence behavior without testing existing LiteTick data and preserving backward compatibility.

## Pull requests

- Keep each pull request focused.
- Explain the behavior change and how it was tested.
- Update user-facing documentation for visible changes.
- Add or update project records when changing product, architecture, storage, or release behavior.
- Do not initialize additional repositories, publish artifacts, or add third-party dependencies without prior discussion.

By submitting a contribution, you agree that it may be distributed under the repository's MIT License.
