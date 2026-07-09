# Test Plan

## Test Approach

The repository will use a console-based .NET test runner to avoid external package dependencies. The runner will fail fast with a non-zero exit code if any verification fails.

Planned command:

```powershell
dotnet run --project tests/TalLifeShield.Tests
```

## Automated Tests

| Test ID | Covers AC | Purpose |
| --- | --- | --- |
| T-001 | AC-006 | Verify knowledge base contains LI, IP, CI, TPD, life-event scenarios, disclaimers, handoff, and support fallback. |
| T-002 | AC-007 | Verify unsupported or personal-advice prompts return a safe fallback and do not invent facts. |
| T-003 | AC-008 | Verify intent router detects all required intents. |
| T-004 | AC-009 | Verify mock service exposes structures equivalent to endpoint payloads. |
| T-005 | AC-010 | Verify product covers include LI, IP, CI, TPD and occupations for `acc` include supplied accountant examples. |
| T-006 | AC-011 | Verify quote flow blocks premium calculation until minimum details are present. |
| T-007 | AC-012 | Verify PII fields are requested only after continuation. |
| T-008 | AC-013 | Verify premium calculation returns 220.93 and update quote returns total premium 220.93, policy fee 8.0, stamp duty 0.0. |
| T-009 | AC-002 | Verify traceability matrix includes every AC ID. |
| T-010 | AC-015 | Verify all test cases pass in one run. |
| T-011 | AC-017 | Verify discount flow asks height/weight after initial quote, applies final discount presentation, includes CSO phone/email, and exposes workflow steps. |
| T-012 | AC-018 | Verify mock contract responses include field-level coverage for cover levels, benefit/waiting periods, quote metadata, application quote ID, product/pricing versions, discount/BMI fields, payment frequency, and projections. |
| T-013 | AC-001 | Verify required SDD documents exist before implementation. |
| T-014 | AC-003 | Verify dependency graph exists and implementation plan declares graph-derived order. |

## Manual / Smoke Verification

| Check ID | Covers AC | Purpose |
| --- | --- | --- |
| M-001 | AC-004 | Open home page and confirm TAL-like header, hero, CTA, and launcher. |
| M-002 | AC-005 | Open chat, use quick replies and free text, confirm messages, typing state, quote card, final quote, and handoff. |
| M-003 | AC-014 | Resize viewport and tab through controls to confirm responsive and accessible behavior. |
| M-004 | AC-016 | Confirm final SDLC summary is present and AC-by-AC status is complete. |

## Hard Gate Criteria

The hard gate passes only when:

- `dotnet run --project tests/TalLifeShield.Tests` exits with code 0.
- Manual smoke verification is completed or a limitation is documented.
- `docs/TRACEABILITY_MATRIX.md` has no planned rows remaining.
- `docs/FINAL_SDLC_SUMMARY.md` is authored.
