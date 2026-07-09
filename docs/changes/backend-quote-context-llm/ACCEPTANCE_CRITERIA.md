# Acceptance Criteria

| AC ID | Criterion |
| --- | --- |
| AC-001 | Scoped SDD documents exist and pass the initial SPEC gate before implementation. |
| AC-002 | Audit identifies that the current frontend assembles a quote card from fixed values and direct API calls. |
| AC-003 | Frontend quick quote completion calls a backend orchestration endpoint for quote presentation instead of building fixed quote rows locally. |
| AC-004 | Backend quick quote orchestration calls create quote and premium calculation services and returns a quote card DTO. |
| AC-005 | Premium calculation varies for different quote inputs while preserving the existing baseline sample premium behavior. |
| AC-006 | Backend LLM presentation receives create quote response, premium response, quote answers, and conversation history as grounding. |
| AC-007 | LLM output may shape the quick quote display but cannot override numeric premium values outside backend API responses. |
| AC-008 | If LLM quick quote presentation fails, fallback quote display still uses backend API response values. |
| AC-009 | Frontend sends conversation history to backend chat and quote orchestration endpoints. |
| AC-010 | Starting or resuming quote asks only missing quote questions, using previously captured answers from conversation state. |
| AC-011 | Existing post-quote PII and Continue/Talk sequencing remains verified. |
| AC-012 | Build and full verification tests pass. |
| AC-013 | Final traceability and final SDLC summary close every AC and pass the hard gate. |
