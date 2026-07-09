# Acceptance Criteria

| ID | Acceptance Criterion | Verification Method |
| --- | --- | --- |
| AC-001 | Scoped SDD docs exist and pass the SPEC gate before implementation. | SDD gate script. |
| AC-002 | Quick quote readiness requires smoker status, year of birth, gender, and occupation only. | Automated quote flow test. |
| AC-003 | Quick quote readiness does not require annual income, employment status, cover selection, sum insured, or PII. | Automated quote flow test. |
| AC-004 | Chat flow asks only the four required quick-quote questions before the first quote. | Frontend source smoke test. |
| AC-005 | After quote confirmation/continuation, chat asks for name, date of birth, postcode, email, and phone number. | Automated orchestration test and frontend source smoke test. |
| AC-006 | Customer-facing frontend text does not include the words demo, mock, or sample. | Automated source text scan. |
| AC-007 | Customer-facing orchestrator and knowledge base text does not include the words demo, mock, or sample. | Automated source/response scan. |
| AC-008 | Responses remain grounded in KB/API/context and unsupported personal-advice requests still use safe fallback. | Existing and new guardrail tests. |
| AC-009 | Existing product cover, occupation, premium, API contract, and UI smoke tests continue to pass. | Full verification test run. |
| AC-010 | Traceability and final summary record AC-by-AC evidence with no planned rows remaining. | Final SDD gate. |

