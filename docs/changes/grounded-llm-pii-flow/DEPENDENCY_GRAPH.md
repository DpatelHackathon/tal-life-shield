# Dependency Graph

```mermaid
graph TD
    A["R-001: Scoped SDD package"] --> B["R-002: Backend-owned routing"]
    A --> C["R-003: Grounded LLM response generator"]
    C --> D["R-004/R-005: Guarded fallback behavior"]
    B --> E["R-006: Newly married life-event handling"]
    B --> F["R-007: Post-quote confirmation"]
    F --> G["R-008: Contact detail capture"]
    G --> H["R-009: Continue/Talk options after contact details"]
    D --> I["R-010: Verification and final gate"]
    E --> I
    H --> I
```

## Implementation Order

1. Author and gate the scoped SDD documents.
2. Add grounded LLM response generation interfaces and fallback behavior.
3. Wire response generation into `ChatOrchestrator`.
4. Move frontend free-text routing to backend response intent handling.
5. Add post-quote confirmation and contact-detail capture state.
6. Add tests for LLM grounding, frontend routing, life-event behavior, and PII sequencing.
7. Run build, test suite, traceability update, and final gate.
