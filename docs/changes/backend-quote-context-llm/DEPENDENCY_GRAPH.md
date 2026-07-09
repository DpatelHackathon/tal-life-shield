# Dependency Graph

```mermaid
graph TD
    A["R-001: SDD package"] --> B["R-002: Audit current quote path"]
    B --> C["R-003/R-004: Backend quote orchestration endpoint"]
    C --> D["R-005/R-006: LLM quote presenter with fallback"]
    C --> E["R-004: Variable premium service behavior"]
    D --> F["R-007: Conversation history grounding"]
    E --> G["R-008: Frontend calls backend quote presenter"]
    F --> G
    G --> H["R-008/R-009: Avoid repeated quote questions and preserve PII flow"]
    H --> I["R-010: Build, tests, gate, staging deploy"]
```

## Implementation Order

1. Author and pass scoped SPEC gate.
2. Add audit evidence to docs.
3. Add backend quote presentation DTOs and service.
4. Add variable backend premium calculation from quote inputs.
5. Add LLM-backed quote presenter with deterministic fallback.
6. Update API endpoint wiring.
7. Update frontend to send conversation history and consume backend quote presentation.
8. Add tests for each AC.
9. Build, test, final gate, and deploy to staging slot only.
