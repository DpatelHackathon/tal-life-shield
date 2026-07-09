# Agentic SDLC Workflow

This workflow turns each change request into a repeatable deterministic delivery cycle.

## Phase A: Intake Agent

Goal: convert the user request into a bounded change request.

Outputs:

- Change ID.
- Source files and evidence list.
- Assumptions.
- Open questions.
- Out-of-scope list.

Gate:

- No SPEC writing starts until source inputs and assumptions are explicit.

## Phase B: SPEC Agent

Goal: author the SPEC before implementation.

Outputs:

- `SPEC.md`
- Requirement IDs.
- Data/API contracts.
- Guardrails for AI, privacy, regulated content, or external data.

Gate:

- SPEC must avoid invented facts.
- All requirements must be linked to source evidence or assumptions.

## Phase C: Acceptance Criteria Agent

Goal: make the change testable.

Outputs:

- `ACCEPTANCE_CRITERIA.md`
- Stable AC IDs.
- Verification method for each AC.

Gate:

- No AC may be vague, subjective, or unverifiable.

## Phase D: Dependency Graph Agent

Goal: determine safe implementation order.

Outputs:

- `DEPENDENCY_GRAPH.md`
- Topological implementation order.

Gate:

- No implementation plan may be written before the graph exists.

## Phase E: Planning Agent

Goal: convert graph order into implementation tasks.

Outputs:

- `IMPLEMENTATION_PLAN.md`
- Task ownership by agent role.
- Planned source files/modules.

Gate:

- Plan must follow dependency graph order.

## Phase F: Traceability Agent

Goal: link every requirement and AC to planned code and verification.

Outputs:

- `TRACEABILITY_MATRIX.md`

Gate:

- Every AC must map to requirement, design decision, implementation task, planned source file/module, test case, and status.

## Phase G: SPEC Review Agent

Goal: independently verify the SPEC package before implementation.

Outputs:

- `SPEC_VERIFICATION.md`
- Pass/fail decision.
- Required remediations if failed.

Gate:

- Implementation cannot start until this phase passes.
- `scripts/Invoke-SddGate.ps1` must pass.

## Phase H: Implementation Agents

Goal: implement the change in bounded, non-overlapping slices.

Potential agents:

- Backend Agent.
- Frontend Agent.
- AI Orchestration Agent.
- Data/Contract Agent.
- Security/Privacy Agent.
- Test Agent.

Gate:

- Implementation must not change ACs to fit code.
- New discoveries must update SPEC and traceability before continuing.

## Phase I: Verification Agent

Goal: verify every AC.

Outputs:

- Test results.
- Manual verification notes when needed.
- Updated traceability matrix.

Gate:

- No AC may remain in Planned status.

## Phase J: Final Gate Agent

Goal: close the audit trail.

Outputs:

- `FINAL_SDLC_SUMMARY.md`
- AC-by-AC completion status.
- Limitations and accepted risks.

Gate:

- `scripts/Invoke-SddGate.ps1 -RequireFinal` must pass before completion is declared.

