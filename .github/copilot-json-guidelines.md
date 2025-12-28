# Copilot JSON Guidelines

Purpose
-------
This file tells Copilot (and other generative assistants reading the repo) how to produce .json files for this project.

Rules (strict)
----------------
- Always produce valid JSON (RFC 8259).
- Never include comments in .json output. Do not use `//` or `/* ... */`.
- If an explanation or human note is needed, create a separate markdown file next to the JSON file (example: `config.json` → `config.json.md`) and put explanations there.

Allowed alternatives
---------------------
- If metadata is required inside the JSON, use explicit properties (for example `_meta` or `__comment`) so the file remains valid JSON. Only do this when the consumer expects such properties.

Forbidden examples
------------------
Bad (contains comment, not valid JSON):

```jsonc
{
  // default timeout in seconds
  "timeout": 30
}
```

Good (valid JSON):

```json
{
  "timeout": 30
}
```
