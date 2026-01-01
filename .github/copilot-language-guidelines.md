# Copilot Comments Guidelines

Purpose
-------
This file instructs Copilot (and other generative assistants) how to produce comments and XML documentation in C# source files within this repository.

Rules (strict)
----------------
- All comments and XML documentation (`/// <summary/>`, `<param/>`, `<returns/>`, `<remarks/>`, etc.) in classes, records, and interfaces must be written in English.
- Prefer plain ASCII for comments. Avoid non-ASCII characters and diacritics (for example: ąćęłńóśżź). If Unicode is required (proper names, library identifiers), keep it minimal and justified.
- Use XML documentation comments for public API (public/internal types and members). Use concise inline comments (`//` or `/* */`) only for implementation notes that are not part of the public contract.
- Do not place long design explanations inside code comments; prefer repository documentation files (e.g. `docs/` or sidecar `*.md`).

Guidance for Copilot prompts
----------------------------
- When asked to generate or modify C# files, output comments and XML docs in English only.
- Use `///` XML comments for public types and members, and include `summary`, `param`, and `returns` where appropriate.
- Keep comments concise and focused; avoid writing paragraphs longer than a few sentences inside code files.
- If asked to include example usage, prefer a short `/// <example>` block or a separate `*.md` example file.

UI strings and Blazor pages
---------------------------
- All user-facing text in Blazor pages and components (files with extension `.razor`, UI fragments, and static content) should be written in English. This includes headings, labels, button text, validation messages embedded in UI, and other visible strings.
- Exception: localization/i18n resource files (for example `*.resx`, translation JSON files under `wwwroot/i18n` or other dedicated translation folders) may contain localized text and remain in other languages.
- When generating or updating Blazor UI components, produce English text by default. If a translation resource exists, map visible strings to keys and place localized values in i18n files instead of hardcoding non-English text.

Good examples
-------------
XML doc for a class:

```csharp
/// <summary>
/// Represents a blood pressure measurement.
/// </summary>
public record BloodPressureMeasurement(int Systolic, int Diastolic, int? Pulse);
```

Inline comment:

```csharp
// Use UTC to avoid timezone ambiguities in stored timestamps.
var timestamp = DateTimeOffset.UtcNow;
```

Bad examples (do not generate)
-----------------------------
Contains non-English or non-ASCII documentation — forbidden. Use English XML docs and comments instead.

```csharp
/// <summary>
/// Represents a blood pressure measurement.
/// </summary>
```

Inline comment example (English):

```csharp
// Use local time for readability
```

