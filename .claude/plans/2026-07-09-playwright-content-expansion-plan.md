# Playwright Content Expansion Plan for eShopOnWeb

## Goal

Add high-value Playwright learning and demo content to this repository using eShopOnWeb as the real app under test, while keeping examples practical, maintainable, and aligned with current test infrastructure.

## Current Baseline in Repo

- Existing Playwright project: `tests/EndToEndTests`
- Existing browser fixture starts `src/Web/Web.csproj` and tests against `https://localhost:5001`
- Existing smoke test: `tests/EndToEndTests/Playwright/SmokeTests.cs`
- Existing setup docs: `tests/EndToEndTests/README.md`

## Outcomes

1. Add a structured set of Playwright demos organized into three tracks.
2. Demonstrate generated-to-cleaned test authoring workflow.
3. Demonstrate debugging with artifacts (HTML report, screenshot, video, trace).
4. Teach test strategy: what belongs in browser tests vs lower-level tests.
5. Add an AI-assisted workflow section with clear guardrails.

## Track 1: Get a Test Running Through the App URL

### Teaching Objective

Show that eShopOnWeb is tested as a running web application, not as in-memory/component tests.

### Planned Content

1. Add a simple, readable "home page catalog" scenario using the app URL.
2. Add a generated-to-cleaned progression:
   - Generate test via `playwright.ps1 codegen <url>`
   - Save generated draft (for teaching comparison)
   - Refactor to stable locators (role/text/test-id)
   - Extract intent-revealing helper methods/page object
3. Explain why role/text/test-id selectors are preferred over brittle CSS/XPath.

### Candidate Files

- `tests/EndToEndTests/Playwright/CatalogTests.cs` (new)
- `tests/EndToEndTests/Playwright/Pages/CatalogPage.cs` (new)
- `tests/EndToEndTests/README.md` (update with codegen workflow)

### Notes

- Keep assertions tied to user-observable behavior.
- Adapt sample syntax to xUnit style used in this repo.

## Track 2: Debugging with Traces, Screenshots, and Video

### Teaching Objective

Show how to diagnose failures quickly using Playwright execution artifacts.

### Planned Content

1. Add one intentionally failing test (example: wrong basket total assertion).
2. Enable/standardize artifact capture for failures:
   - HTML report
   - screenshot on failure
   - video on failure
   - trace capture
3. Document how to inspect traces locally and via `trace.playwright.dev`.
4. Add explicit guidance:
   - Artifacts are from the failed execution.
   - Visual comparison against prior baselines requires screenshot assertions and/or dedicated visual testing tooling.

### Candidate Files

- `tests/EndToEndTests/Playwright/BasketTests.cs` (new)
- `tests/EndToEndTests/Playwright/BrowserFixture.cs` (update for artifact config or shared context options)
- `tests/EndToEndTests/README.md` (artifact debugging section)
- `tests/EndToEndTests/EndToEndTests.csproj` (if reporter/output config is needed)

## Track 3: Designing Meaningful Tests

### Teaching Objective

Teach scope discipline: Playwright for critical user journeys and system wiring, not exhaustive business rule permutations.

### Planned Content

1. Add strategy guidance using eShopOnWeb examples.
2. Include a "bad strategy vs better strategy" section:
   - Bad: test every combination via browser.
   - Better: keep critical journeys in Playwright; push combinations to unit/integration/API tests.
3. Add a matrix mapping test types to layers for eShopOnWeb scenarios.
4. Include the core teaching phrase:
   - Use Playwright to verify the system is wired together correctly from the user perspective, not to exhaustively prove every business rule.

### Candidate Files

- `docs/explore/tests.md` (update with test strategy narrative)
- `tests/EndToEndTests/README.md` (short practical version)
- Optional: `tests/EndToEndTests/Playwright/LegacyScreenTestingGuidance.cs` (comment-only guidance file, if desired)

## AI and Agent Angle

### Level 1: Playwright Codegen

- Teach codegen as a drafting tool, not final test quality.
- Show: generate, edit manually, improve locators/assertions, extract helpers.

### Level 2: Copilot/LLM Refactor Prompt

Add a reusable prompt example in docs:

- Refactor generated Playwright .NET test into readable xUnit tests.
- Prefer role-based locators.
- Extract helpers only when readability improves.
- Avoid implementation-detail assertions.
- Preserve user intent.

### Level 3: Playwright MCP/Agent Workflow

Demo script should include:

1. Ask agent to explore catalog flows.
2. Ask agent to propose candidate scenarios.
3. Ask agent to draft test code.
4. Human review/edit.
5. Run tests.
6. Inspect trace/artifacts on failure.
7. Ask agent to diagnose based on outputs.

### Candidate Files

- `docs/explore/tests.md` (AI-assisted workflow section)
- `tests/EndToEndTests/README.md` (quick-start version)

## Proposed Test Project Structure

Use existing project and extend foldering under `tests/EndToEndTests/Playwright`.

Planned shape:

- `tests/EndToEndTests/Playwright/CatalogTests.cs`
- `tests/EndToEndTests/Playwright/BasketTests.cs`
- `tests/EndToEndTests/Playwright/AuthenticationTests.cs`
- `tests/EndToEndTests/Playwright/CheckoutTests.cs`
- `tests/EndToEndTests/Playwright/Pages/CatalogPage.cs`
- `tests/EndToEndTests/Playwright/Pages/BasketPage.cs`
- `tests/EndToEndTests/Playwright/Pages/LoginPage.cs`
- `tests/EndToEndTests/Playwright/TestInfrastructure/PlaywrightTestBase.cs`
- `tests/EndToEndTests/Playwright/TestInfrastructure/AppSettings.cs`

## Recommended Demo Order

1. Start eShopOnWeb locally.
2. Run a simple hand-written test against app URL.
3. Generate a test with Playwright codegen.
4. Clean up generated test into maintainable test code.
5. Add and run an intentionally failing assertion.
6. Show HTML report, screenshot/video, and trace.
7. Refactor with page objects/helpers where they improve clarity.
8. Discuss test scope using eShopOnWeb examples.
9. Show AI-assisted workflow.
10. Close with explicit boundaries: Playwright vs lower-level tests.

## Implementation Phases

- [x] Phase 1: Foundation and Track 1
  - [x] Add `CatalogTests.cs` with one clear app-URL scenario.
  - [x] Introduce first page object/helper abstraction.
  - [x] Update README with codegen-to-cleaned workflow.

- [x] Phase 2: Track 2 Artifacts and Failure Diagnostics
  - [x] Add intentionally failing `BasketTests` scenario.
  - [x] Configure artifact output paths and reporter behavior.
  - [x] Document local and hosted trace viewing flow.

- [ ] Phase 3: Track 3 Strategy and AI Guidance
  - [ ] Add strategy section to docs and EndToEnd README.
  - [ ] Add AI workflow script and reusable prompt.
  - [ ] Add at least one example per category (smoke, critical journey, fragile legacy).

- [ ] Phase 4: Stabilization
  - [ ] Ensure tests are deterministic and not timing-fragile.
  - [ ] Validate local run instructions from clean environment.
  - [ ] Verify docs and commands are copy/paste ready.

## Validation Checklist

- `dotnet build tests/EndToEndTests/EndToEndTests.csproj`
- `pwsh tests/EndToEndTests/bin/Debug/net10.0/playwright.ps1 install`
- `dotnet test tests/EndToEndTests/EndToEndTests.csproj`
- Confirm HTML report and trace artifact generation works.
- Confirm documentation matches actual commands and paths.

## Risks and Mitigations

- Risk: brittle selectors in generated code.
  - Mitigation: require role/text/test-id cleanup before merging demo tests.
- Risk: slow/flaky browser tests.
  - Mitigation: keep scope to critical journeys, add reliable waits/assertions.
- Risk: confusion about visual regression scope.
  - Mitigation: explicitly separate "failure artifacts" from baseline visual comparison.
- Risk: agent-generated tests look plausible but miss intent.
  - Mitigation: require human review and intent-based assertions.

## Definition of Done

1. Three demo tracks exist in code and docs.
2. At least one intentionally failing scenario is documented for debugging demos.
3. AI-assisted authoring workflow is documented with practical guardrails.
4. Demo order is reproducible by another developer from repository docs.
5. Playwright scope boundaries are clearly stated against lower-level tests.
