# Implementation Plan

This plan is ordered from `docs/DEPENDENCY_GRAPH.md`.

## Phase 0: SPEC Gate

- Author SPEC, ACs, traceability matrix, dependency graph, implementation plan, test plan, guardrails, and SPEC verification.
- Run an independent SPEC review before implementation.
- Update missing items before writing implementation code.

## Phase 1: Solution Scaffold

- Create .NET solution.
- Create `TalLifeShield.Core` class library.
- Create `TalLifeShield.Api` ASP.NET Core minimal API.
- Create `TalLifeShield.Tests` console verification project.
- Wire project references.

## Phase 2: Core Domain And Mock Data

- Add DTOs using shapes aligned to supplied payloads.
- Add mock service responses for:
  - eligibility application
  - occupations
  - product covers
  - create quote
  - select cover
  - premium calculation
  - update quote
- Add structured local knowledge base.

## Phase 3: AI Orchestration

- Add intent enum and deterministic intent router.
- Add guardrail response handling.
- Add quote flow state validator.
- Add chat orchestrator that chooses KB retrieval, API/service actions, or safe fallback.

## Phase 4: Backend API

- Map REST endpoints to mock services.
- Enable static file hosting.
- Enable permissive local CORS for demo use.
- Serve the React home page at `/`.

## Phase 5: React Frontend

- Build static React app using UMD React scripts.
- Implement TAL-like homepage.
- Implement floating chat launcher and panel.
- Implement conversation flow, quick replies, free text, typing state, quote card, final quote, and handoff.
- Call backend endpoints for product covers, occupations, quote creation, premium calculation, and update quote.

## Phase 6: Tests And Verification

- Implement console test runner.
- Verify backend data structures and source facts.
- Verify KB and guardrails.
- Verify all required intents.
- Verify quote flow minimum data and PII timing.
- Verify traceability coverage.
- Run tests.

## Phase 7: Hard Gate And Summary

- Update traceability matrix with actual source paths and pass status.
- Author final SDLC summary.
- Start local app for user verification.
- Do not declare complete until the gate passes.

