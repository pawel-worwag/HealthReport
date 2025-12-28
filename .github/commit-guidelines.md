# Commit message guidelines

Policy
------
- All commit messages must be written in English.
- Commit messages should use ASCII characters only (no diacritics or non-ASCII letters).

Why
---
Keeping commit messages in English and ASCII improves readability across the team and ensures compatibility with tools that expect ASCII-only metadata.


Assistant guidance
------------------
This repository does not enforce commit-message checks via CI or repository-provided hooks. When you ask the assistant to suggest commit messages, it will propose them in English (ASCII) by default.

Examples
--------
Good:

```
docs: add Copilot JSON guidelines to forbid comments in JSON

Add .github/copilot-json-guidelines.md to instruct assistants to produce valid JSON without comments.
```

Bad (contains Polish diacritics):

```
Dodaj wytyczne Copilot: zabronione komentarze w .json
```
