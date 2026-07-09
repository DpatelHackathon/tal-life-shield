# SPEC: Production Chat Quick Quote Flow

## Purpose

Update the TAL Life Shield chat agent so its customer-facing quote flow behaves like a production-ready assistant:

- Ask only the required quick-quote details first: smoker status, year of birth, gender, and occupation.
- Generate an indicative quote once those details are available.
- After the customer confirms the quote or chooses to continue, ask for PII: name, date of birth, postcode, email, and phone number.
- Remove customer-facing words such as "demo", "mock", and "sample".
- Continue grounding answers in the local knowledge base, conversation context, and backend API responses.

## Source Inputs Reviewed

- User change request in the current Codex thread.
- `.codex/sdd/lead-agent-prompt.md`
- `.codex/sdd/agentic-sdlc.md`
- `.codex/sdd/gates.md`
- `.codex/sdd/skills.md`
- `src/TalLifeShield.Core/ChatOrchestrator.cs`
- `src/TalLifeShield.Core/QuoteFlowState.cs`
- `src/TalLifeShield.Core/KnowledgeBase.cs`
- `src/TalLifeShield.Api/wwwroot/app.js`
- `tests/TalLifeShield.Tests/Program.cs`

## Assumptions

- "Must not use words like demo or mock" applies to customer-facing chat, frontend copy, and API health text, not internal class names or historical first-build audit documents.
- The backend remains a deterministic local API implementation for this repository, but customer-facing responses must present it as the backend API, not as mock/demo data.
- The quote remains indicative and subject to eligibility and underwriting.
- Existing acceptance coverage for the original first build must remain green after the change, but wording expectations should be updated where they conflict with this change.

## Functional Requirements

### R-001 SDD Change Governance

This change must follow the repo-attached Agentic SDD workflow with scoped docs, SPEC gate, tests, traceability, final summary, and final gate.

### R-002 Four-Field Quick Quote Capture

Before calculating a quick quote, the chat agent must require only:

- smoker status
- year of birth
- gender
- occupation

It must not require annual income, employment status, cover selection, sum insured, name, date of birth, postcode, email, or phone before the first quick quote.

### R-003 Post-Quote PII Collection

After the indicative quote is shown and the customer confirms or chooses to continue, the chat agent must ask for:

- name
- date of birth
- postcode
- email
- phone number

Date of birth is deliberately collected after quote confirmation, while year of birth is collected before quick quote.

### R-004 Production-Ready Customer-Facing Language

Customer-facing chat, homepage, quote card, guardrail fallback, and knowledge base responses must not use the words:

- demo
- mock
- sample

The assistant must refer to knowledge base information, backend API responses, indicative quote, eligibility, and underwriting without exposing implementation labels.

### R-005 Grounded Intent-Based Responses

The assistant must continue to:

- detect intent
- use conversation context
- retrieve knowledge base facts
- call backend API services when quote or occupation actions require it
- use safe fallback when KB/API/context cannot answer
- avoid personal advice

## Non-Functional Requirements

- Existing API contracts must continue to work.
- Existing tests must continue to pass after being updated for this change.
- Customer-facing language must remain concise, general-information-only, and underwriting-safe.

## Out Of Scope

- Real external TAL API integration.
- Replacing deterministic local orchestration with a live LLM.
- Removing internal class names such as `MockEligibilityService`.
- Rewriting historical root-level SDD docs from the first build.

## SPEC Gate

Implementation may start only after acceptance criteria, traceability, dependency graph, implementation plan, test plan, AI guardrails, and SPEC verification are complete and `scripts/Invoke-SddGate.ps1 -ChangeDocsPath docs/changes/production-chat-quote-flow` passes.

