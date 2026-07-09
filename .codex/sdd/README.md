# Repo-Attached SDD Operating System

This folder contains a reusable Specification Driven Development process for deterministic agentic changes in this repository.

Use it for every meaningful change request:

1. Capture the change request.
2. Author the SPEC and acceptance criteria.
3. Build the traceability matrix.
4. Build the dependency graph.
5. Generate the implementation plan from the dependency graph.
6. Verify the SPEC before code changes.
7. Implement only after the SPEC gate passes.
8. Test against acceptance criteria.
9. Update traceability with evidence.
10. Run the hard completion gate before declaring done.

Core files:

- `agentic-sdlc.md`: phase-by-phase Agentic SDLC workflow.
- `skills.md`: skill routing rules and evidence requirements.
- `agent-roles.md`: reusable agent responsibilities.
- `prompts/`: copy/paste prompts for repeatable change agents.
- `templates/`: reusable SDD document templates.
- `gates.md`: pass/fail rules for each delivery gate.
- `scripts/Invoke-SddGate.ps1`: deterministic gate checker.

## Current Configuration

Default change docs path: `docs`

The current TAL Life Shield chat assistant documents live in `docs` and can be validated with:

```powershell
.\scripts\Invoke-SddGate.ps1
```

For the final completion gate, after implementation and final summary are complete:

```powershell
.\scripts\Invoke-SddGate.ps1 -RequireFinal
```

## Reuse Pattern

For a new change, either:

- Reuse the root `docs` folder for the active change, or
- Create `docs/changes/<change-id>` and run:

```powershell
.\scripts\Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\<change-id>
```

The agent should never skip the SPEC gate just because a change is small. Small changes may have smaller specs, but they still need acceptance criteria, traceability, dependency order, and verification.

## Repeatable Agent Sequence

For deterministic changes, run agents in this order:

1. Intake Agent.
2. SPEC Agent.
3. Acceptance Criteria Agent.
4. Dependency Graph Agent.
5. Planning Agent.
6. Traceability Agent.
7. SPEC Review Agent.
8. Implementation Agents with bounded ownership.
9. Verification Agent.
10. Final Gate Agent.

Use the prompts in `.codex/sdd/prompts` for each role.
