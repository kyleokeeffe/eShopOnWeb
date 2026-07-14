---
name: playwright-issue-repro-report
description: Reproduce GitHub issues with Playwright, collect screenshot/video/trace artifacts, and publish evidence-based issue comments. Use when a bug report needs reproducible UI steps and supporting artifacts for triage.
metadata:
  author: nimblepros
  version: "1.0"
  spec: agentskills.io
---

# Playwright Issue Repro and Reporting Skill

## Purpose

Provide a repeatable workflow for:

1. Translating a GitHub issue into a reproducible Playwright scenario.
2. Capturing evidence artifacts (`failure.png`, `trace.zip`, `video/`).
3. Posting a structured issue comment with repro details and links.

## Use This Skill When

- A UI bug in GitHub needs reliable repro steps.
- You need shareable evidence (screenshots, video, trace).
- You want a consistent issue comment format across triage sessions.

## Required Inputs

1. Issue URL or issue number.
2. Expected behavior and observed behavior from the issue.
3. App URL and environment context.
4. Repro preconditions (user role, seed data, feature flags).

## Workflow

1. Build context from the issue:

- Extract expected behavior, observed behavior, and acceptance clues.
- Convert narrative to deterministic steps.

2. Draft or adapt a Playwright repro test:

- Prefer semantic locators.
- Keep assertions user-visible.
- Avoid hard sleeps and implementation-detail assertions.

3. Run repro and capture artifacts:

- Use existing project artifact flow under `TestResults/PlaywrightArtifacts/`.
- Collect screenshot, trace, and video paths.

4. Publish an issue comment:

- Summarize repro result.
- Include environment, exact steps, and evidence links.
- Add short risk/impact notes and suggested next action.

## Output Contract

Return:

1. Repro script/test snippet (or reference to updated test).
2. Artifact paths and what each artifact proves. Call out specific evidence for each assertion.
3. Ready-to-post issue comment body.

## Reusable Assets

- Workflow details: [references/WORKFLOW.md](references/WORKFLOW.md)
- Repro prompt template: [templates/issue-repro-prompt.md](templates/issue-repro-prompt.md)
- Issue comment template: [templates/issue-comment-template.md](templates/issue-comment-template.md)

## Guardrails

- Do not claim reproduction unless assertion and artifacts agree.
- Do not include secrets or sensitive environment values in issue comments.
- Always distinguish "reproduced" vs "not reproduced" vs "inconclusive".
