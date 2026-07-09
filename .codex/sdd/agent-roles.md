# Agent Roles

Use these roles as bounded workstreams. A Lead Agent may perform the work directly or delegate to subagents when parallel review or implementation slices are useful.

## Lead Agent

- Owns the SDD flow.
- Keeps the dependency graph, plan, and traceability aligned.
- Decides when gates pass or fail.
- Integrates implementation and final evidence.

## SPEC Reviewer

- Independently checks source inputs against SPEC, ACs, tests, and guardrails.
- Reports Pass/Fail before implementation.
- Does not edit files unless explicitly assigned.

## Backend Agent

- Owns API contracts, services, data models, migrations, and backend tests.
- Must preserve existing contracts unless the SPEC explicitly changes them.

## Frontend Agent

- Owns UI states, accessibility, responsiveness, API client integration, and frontend tests.
- Must follow existing design and interaction patterns.

## AI Orchestration Agent

- Owns prompts, tools, intent routing, retrieval, guardrails, and model integration.
- Must prove grounding and refusal behavior with tests.

## Test Agent

- Owns automated verification against AC IDs.
- Finds missing or weak evidence.
- Does not lower criteria to make tests pass.

## Privacy And Safety Agent

- Owns PII handling, data minimization, safety disclaimers, and domain-specific boundaries.
- Ensures user-facing AI behavior remains inside the SPEC guardrails.

## Traceability Agent

- Owns the matrix from requirement to implementation and evidence.
- Ensures every AC has source files, test cases, and final status.

