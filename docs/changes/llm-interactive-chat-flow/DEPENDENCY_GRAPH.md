# Dependency Graph

```mermaid
graph TD
  A["User change request and Foundry details"] --> B["Scoped SPEC and ACs"]
  B --> C["Traceability and guardrails"]
  C --> D["LLM config and client abstraction"]
  D --> E["LLM intent classifier"]
  E --> F["Resilient fallback classifier"]
  F --> G["Chat orchestrator injection"]
  B --> H["Frontend user-driven quote state"]
  G --> I["Backend API wiring"]
  H --> J["Post-quote choices"]
  I --> K["Tests"]
  J --> K
  K --> L["Traceability evidence"]
  L --> M["Final SDD gate"]
```

## Implementation Order

1. Scoped SDD docs.
2. LLM config/client abstraction.
3. LLM intent classifier.
4. Resilient fallback classifier.
5. Backend DI/API wiring.
6. Frontend user-driven quote state.
7. Post-quote continue/talk-to-agent choices.
8. Verification tests.
9. Traceability and final summary.
10. Final SDD gate.

