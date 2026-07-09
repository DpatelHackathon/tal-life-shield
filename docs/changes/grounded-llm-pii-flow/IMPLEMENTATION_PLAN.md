# Implementation Plan

This plan is ordered from `DEPENDENCY_GRAPH.md`.

## Phase 0: SPEC Gate

- Author scoped SDD docs.
- Run the scoped SPEC gate.

## Phase 1: AI Orchestration

- Add a chat response generation abstraction.
- Add an LLM-backed generator using the existing Responses API client.
- Add a resilient wrapper that falls back to deterministic responses.
- Keep all non-message metadata from the deterministic backend response.

## Phase 2: Backend Conversation Behavior

- Use the response generator in the chat orchestrator for knowledge, comparison, and fallback responses.
- Extend life-event grounding for newly married and partner/spouse contexts.
- Allow post-quote continuation confirmation to trigger contact-detail collection while still blocking early PII capture.

## Phase 3: Frontend Flow

- Remove local free-text keyword routing.
- Interpret backend response intents for quote start, contact-detail collection, and normal display.
- After quote, offer "I'm happy to go ahead" and "Talk to an agent".
- Collect name, date of birth, postcode, email, phone number, and address after confirmation.
- Show Continue application and Talk to an agent only after details are captured.

## Phase 4: Verification

- Add tests mapped to ACs.
- Run build and full verification tests.
- Update traceability and final summary.
- Run final hard gate.
