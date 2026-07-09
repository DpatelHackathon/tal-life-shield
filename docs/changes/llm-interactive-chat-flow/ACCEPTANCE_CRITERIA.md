# Acceptance Criteria

| ID | Acceptance Criterion | Verification Method |
| --- | --- | --- |
| AC-001 | Scoped SDD docs exist and pass the SPEC gate before implementation. | SDD gate script. |
| AC-002 | Frontend guided quote flow asks one question at a time and does not auto-answer agent questions. | Automated source smoke test. |
| AC-003 | The quick-quote flow waits for customer answers for smoker status, year of birth, gender, and occupation before calculating quote. | Automated source smoke test. |
| AC-004 | After quick quote generation, frontend presents Continue application and Talk to an agent options. | Automated source smoke test. |
| AC-005 | Talk to an agent path presents TAL contact number `131 825`. | Automated orchestration/frontend test. |
| AC-006 | Backend primary intent classification uses an LLM classifier abstraction when configured. | Automated unit test with fake LLM client. |
| AC-007 | LLM classifier calls Azure/OpenAI Responses API-compatible `/responses` endpoint using endpoint, API key, and model/deployment configuration. | Automated source/configuration test. |
| AC-008 | API key is not committed to repository files. | Automated secret scan. |
| AC-009 | If LLM configuration is missing or fails, routing falls back safely without exposing secrets/errors to customers. | Automated fallback test. |
| AC-010 | Responses remain grounded in KB/API/context and unsupported personal-advice requests still use safe fallback. | Automated guardrail tests. |
| AC-011 | Existing product cover, occupation, premium, production wording, API contract, and UI smoke tests continue to pass. | Full verification test run. |
| AC-012 | Traceability and final summary record AC-by-AC evidence with no planned rows remaining. | Final SDD gate. |

