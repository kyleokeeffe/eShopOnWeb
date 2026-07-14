# Prompt Template: Reproduce a GitHub Issue with Playwright

You are helping reproduce a UI bug reported in a GitHub issue using Playwright in this repository.

## Inputs

Issue URL/number:

<replace>

Issue summary:

<replace>

Expected behavior:

<replace>

Observed behavior:

<replace>

Preconditions:

<replace>

## Requirements

1. Convert the issue narrative into deterministic repro steps.
2. Produce or update a Playwright C# test aligned with repo conventions.
3. Use semantic locators when possible.
4. Assert user-observable behavior only.
5. Run a targeted test filter if provided.
6. Collect evidence paths for screenshot/trace/video artifacts.

## Output format

1. Repro status: reproduced / not reproduced / inconclusive.
2. Exact repro steps used.
3. Test code (or file reference and diff summary).
4. Artifact paths captured.
5. Draft GitHub issue comment using `templates/issue-comment-template.md`.
