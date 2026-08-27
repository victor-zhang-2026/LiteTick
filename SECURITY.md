# Security Policy

## Reporting a vulnerability

Please do not publish security vulnerabilities in a public issue.

Before a private reporting channel is listed, use GitHub's private vulnerability reporting feature for this repository. Include the affected version, macOS version, reproduction steps, impact, and any suggested mitigation. Do not include unrelated personal data or real task contents.

## Scope

LiteTick is local-only and has no account, server, analytics, update service, or synchronization system. Task files are intentionally plaintext and are not a secrets vault. Plaintext local storage is a documented limitation, not by itself a vulnerability.

Security-sensitive areas include local file handling, migration and backup behavior, code-signing and release integrity, unsafe URL or process handling, and unintended network access.
