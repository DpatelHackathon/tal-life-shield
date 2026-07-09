# SDD Gates

## Gate 1: Intake Gate

Pass criteria:

- Change request is captured.
- Source inputs are listed.
- Open questions or assumptions are explicit.

Fail if:

- The requested change is ambiguous and a safe assumption cannot be made.
- Required source files are missing.

## Gate 2: SPEC Gate

Pass criteria:

- SPEC exists.
- Acceptance criteria exist with stable AC IDs.
- Traceability matrix maps each AC.
- Dependency graph exists.
- Implementation plan is derived from the graph.
- Test plan maps ACs to verification.
- Guardrails exist when AI behavior, privacy, regulated content, or external data is involved.
- SPEC verification explicitly says the gate passed.
- `scripts/Invoke-SddGate.ps1` passes for the change docs path.

Fail if:

- Any AC has no planned verification.
- Any source requirement is missing from the SPEC.
- Any implementation task appears before its dependency.
- Any claim is unsupported by source evidence or documented assumptions.

## Gate 3: Implementation Gate

Pass criteria:

- Code changes match the implementation plan.
- Traceability matrix is updated with real source files.
- Tests are implemented for all automatable ACs.
- Manual checks are documented for non-automatable ACs.

Fail if:

- Code changes are outside scope without documented rationale.
- Acceptance criteria are changed to match implementation without review.

## Gate 4: Verification Gate

Pass criteria:

- Automated tests pass.
- Guardrail tests pass where applicable.
- Traceability matrix has no unverified ACs.
- Final SDLC summary exists.
- `scripts/Invoke-SddGate.ps1 -RequireFinal` passes.

Fail if:

- Tests are failing or skipped without accepted limitation.
- Traceability rows remain planned.
- Final summary lacks AC-by-AC status.

