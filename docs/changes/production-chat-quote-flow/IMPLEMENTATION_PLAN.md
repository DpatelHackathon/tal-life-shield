# Implementation Plan

This plan is ordered from `DEPENDENCY_GRAPH.md`.

## Phase 0: SPEC Gate

- Create scoped SDD documents under `docs/changes/production-chat-quote-flow`.
- Run `.\scripts\Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\production-chat-quote-flow`.

## Phase 1: Core Flow

- Update `QuoteFlowState` so quick quote readiness requires only smoker status, year of birth, gender, and occupation.
- Update missing field messages accordingly.
- Keep PII gate after quote continuation.

## Phase 2: AI Orchestration And KB

- Update `ChatOrchestrator` wording to remove customer-facing demo/mock/sample language.
- Ensure safe fallback references available knowledge base/backend API/context only.
- Ensure post-quote PII request includes name, date of birth, postcode, email, and phone.
- Update `KnowledgeBase` quote/disclaimer wording.

## Phase 3: Frontend

- Update guided quote questions to smoker status, year of birth, gender, and occupation.
- Remove pre-quote annual income, employment status, cover, and sum insured questions.
- Remove customer-facing demo/mock/sample wording.
- Update continue step to ask for name, date of birth, postcode, email, and phone.

## Phase 4: Verification

- Update console verification tests.
- Add production wording scans.
- Run build and full tests.
- Update traceability matrix and final summary.
- Run final SDD gate.

