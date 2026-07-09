# Dependency Graph

```mermaid
graph TD
  A["Source payload and demo extraction"] --> B["SPEC and ACs"]
  B --> C["Traceability matrix"]
  B --> D["AI guardrails"]
  B --> E["Test plan"]
  C --> F["Dependency-driven implementation plan"]
  D --> G["Knowledge base"]
  A --> H["Mock API contract model"]
  H --> I["Mock backend service"]
  G --> J["Intent router"]
  D --> J
  I --> K["Chat orchestration"]
  J --> K
  K --> L["Quote flow state machine"]
  I --> M[".NET API endpoints"]
  K --> N["Frontend API client"]
  L --> O["React chat widget"]
  N --> O
  M --> P["Automated backend tests"]
  G --> Q["Guardrail and KB tests"]
  J --> R["Intent tests"]
  L --> S["Quote flow privacy tests"]
  O --> T["Frontend smoke verification"]
  P --> U["Hard completion gate"]
  Q --> U
  R --> U
  S --> U
  T --> U
  U --> V["Final SDLC summary"]
```

## Implementation Order Derived From Graph

1. Source payload and demo extraction.
2. SPEC and acceptance criteria.
3. Traceability matrix, guardrails, test plan, dependency graph.
4. Mock API contract and local domain models.
5. Knowledge base.
6. Intent router.
7. Mock backend service.
8. Quote flow state machine.
9. Chat orchestration.
10. .NET API endpoints.
11. Frontend API client and React chat widget.
12. Automated tests.
13. Frontend smoke/manual verification.
14. Traceability matrix update.
15. Final SDLC summary.

