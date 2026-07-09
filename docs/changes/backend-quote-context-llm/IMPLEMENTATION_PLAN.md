# Implementation Plan

This plan is ordered from `DEPENDENCY_GRAPH.md`.

## Phase 0: SPEC Gate

- Create the scoped SDD change package.
- Run the initial SDD gate.

## Phase 1: Backend Quote Orchestration

- Add DTOs for conversation turns, quote answers, quote card rows, and quote presentation response.
- Add a backend service that calls create quote and premium calculation.
- Make premium calculation vary from stored quote inputs while preserving the sample baseline.

## Phase 2: AI Presentation

- Add an LLM quote presenter that receives backend API JSON and conversation history.
- Parse constrained JSON output into display fields.
- Preserve backend numeric values if the LLM is missing, invalid, or tries to omit required data.

## Phase 3: Frontend Integration

- Send recent conversation history with chat and quote presentation requests.
- Replace direct frontend quote-card assembly with the backend quote presentation response.
- Do not reset already captured quote answers when resuming a quote.
- Ask only missing quote questions.

## Phase 4: Verification And Deployment

- Add source and unit tests.
- Build and run the full test suite.
- Update traceability and final summary.
- Run final hard gate.
- Package and deploy to the Azure Web App `staging-one` slot only.
