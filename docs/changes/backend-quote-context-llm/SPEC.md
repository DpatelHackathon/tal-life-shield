# SPEC: Backend Quick Quote Presentation With Conversation Context

## Objective

Audit and fix the quick quote integration so the displayed quote is generated from backend API responses and conversation context. The frontend must not assemble a fixed quote card from hard-coded quote values. The backend must support AI LLM conversion of backend quote API responses into customer-facing quick quote details, with deterministic fallback when the LLM is not configured.

## Source Inputs

- User request dated 2026-07-09.
- Current staged app at `https://sre-rangers-web-tal-lifeplus-staging-one.azurewebsites.net/`.
- Existing local code in `src/TalLifeShield.Api`, `src/TalLifeShield.Core`, and `tests/TalLifeShield.Tests`.
- Existing SDD operating rules in `.codex/sdd`.

## Requirements

- R-001: Follow the repository lead-agent SPEC-driven development process.
- R-002: Audit the current quick quote integration and document the defect.
- R-003: Quick quote display must be driven by backend API responses, not hard-coded frontend quote values.
- R-004: Backend premium values must vary based on quote inputs so the same fixed quote is not always presented.
- R-005: A backend AI presentation layer must be able to read the create-quote and premium-calculation API responses and convert them into customer-facing quick quote details.
- R-006: If the LLM call is unavailable or invalid, deterministic fallback must still use backend API response values.
- R-007: Conversation history must be passed to backend orchestration and used as context for response generation.
- R-008: The quote flow must avoid asking already-answered quote questions again during the same conversation.
- R-009: Existing PII sequencing must remain intact: post-quote confirmation before contact capture, then Continue/Talk options.
- R-010: Build, tests, final SDD gate, and staging-only deployment must be attempted after implementation.

## Assumptions

- The LLM converts grounded backend JSON into display copy and rows; it does not invent premiums, eligibility, or underwriting decisions.
- The frontend may keep session-only conversation state for this demo.
- Azure slot app settings may still require separate MFA-enabled configuration; no API keys are committed to the repository.
