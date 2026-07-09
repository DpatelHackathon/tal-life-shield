# Acceptance Criteria

| AC ID | Criterion |
| --- | --- |
| AC-001 | Scoped SDD documents exist and pass the initial SPEC gate before implementation. |
| AC-002 | Frontend free-text messages call `/api/chat/message` and do not use local keyword branches for life events, cover options, quote start, continue, or talk-to-agent routing. |
| AC-003 | Backend orchestration includes a grounded LLM response generator that uses the Azure/OpenAI Responses API client when configured. |
| AC-004 | LLM-generated responses preserve deterministic response metadata such as intent, quick replies, quote payload, workflow steps, and PII flags. |
| AC-005 | LLM response generation is grounded with knowledge base content and deterministic backend response text, and guardrail instructions prohibit unsupported personal advice. |
| AC-006 | If LLM response generation fails, customer response falls back without exposing provider errors. |
| AC-007 | "I am newly married" maps to a relevant life-event response and does not return only the generic quote disclaimer. |
| AC-008 | After quote generation, the frontend offers confirmation to proceed or talk to an agent, not immediate Continue application. |
| AC-009 | When the customer confirms they are happy to go ahead after quote, the frontend collects name, date of birth, postcode, email, phone number, and address. |
| AC-010 | Continue application and Talk to an agent options appear only after contact details are captured. |
| AC-011 | All existing SDD verification tests continue to pass. |
| AC-012 | Final traceability and final SDLC summary close every AC and pass the hard gate. |
