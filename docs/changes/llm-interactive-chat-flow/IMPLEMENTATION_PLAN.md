# Implementation Plan

This plan is ordered from `DEPENDENCY_GRAPH.md`.

## Phase 0: SPEC Gate

- Author scoped docs.
- Run scoped SPEC gate.

## Phase 1: LLM Intent Infrastructure

- Add intent classifier interface.
- Add deterministic fallback classifier adapter.
- Add Azure/OpenAI Responses API client using `HttpClient`.
- Add LLM intent classifier that requests JSON-only intent output.
- Add resilient wrapper that falls back without leaking details.

## Phase 2: Backend Wiring

- Register LLM options from configuration/environment.
- Register classifier services in API DI.
- Update chat endpoint to use async orchestrator path.

## Phase 3: Frontend Interaction

- Replace auto-answer quote script with user-driven state machine.
- Ask one quote question at a time.
- Capture smoker, year of birth, gender, and occupation from customer input.
- Generate quote after required answers.
- Present Continue application and Talk to an agent choices.
- Show TAL phone number when Talk to an agent is selected.

## Phase 4: Verification

- Add tests for no auto-answering, post-quote choices, LLM classifier, fallback, and secret scan.
- Run build, full verification tests, and final SDD gate.

