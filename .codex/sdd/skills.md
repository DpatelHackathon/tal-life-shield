# Skills Routing

Use skills deliberately. A skill is selected only when it materially improves correctness, repeatability, or verification.

## Core Skills

| Skill | Use When | Required Output |
| --- | --- | --- |
| SPEC authoring | Requirements need formalization. | SPEC with requirement IDs and source evidence. |
| AC authoring | Requirements need testable completion criteria. | Stable AC IDs and verification method. |
| Dependency graphing | Implementation has multiple components or order risk. | Graph and topological implementation order. |
| Traceability | Auditability is required. | Matrix from requirement to AC to implementation to test. |
| Backend engineering | API, service, database, or contract changes are needed. | Backend implementation and tests. |
| Frontend engineering | UI, UX, accessibility, or client integration changes are needed. | Frontend implementation and smoke evidence. |
| AI orchestration | LLM, retrieval, prompts, tool use, or intent routing is involved. | Grounded orchestration and guardrail tests. |
| Privacy and safety | PII, regulated advice, security, or safety constraints exist. | Risk controls and tests. |
| Verification | Any change reaches implementation. | Test execution and AC evidence. |

## Skill Selection Rules

- Use the smallest set of skills that covers the change.
- Prefer repository patterns over new abstractions.
- For AI behavior, always include AI orchestration, privacy/safety, and verification skills.
- For user-facing UI, always include frontend and accessibility checks.
- For API or data contracts, always include backend and traceability checks.

## Skill Evidence

Each selected skill must leave evidence in at least one of:

- `IMPLEMENTATION_PLAN.md`
- `TRACEABILITY_MATRIX.md`
- Test output.
- `FINAL_SDLC_SUMMARY.md`

