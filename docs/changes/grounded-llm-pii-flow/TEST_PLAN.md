# Test Plan

| AC ID | Verification |
| --- | --- |
| AC-001 | `Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\grounded-llm-pii-flow` |
| AC-002 | Source test verifies frontend no longer contains local free-text keyword branches and still posts to `/api/chat/message`. |
| AC-003 | Source/unit test verifies grounded LLM response generator and Responses API client usage. |
| AC-004 | Unit test verifies LLM generator preserves metadata while replacing only message/source. |
| AC-005 | Source test verifies grounding instructions include knowledge base, deterministic response, and personal advice guardrails. |
| AC-006 | Unit test verifies failed LLM response generation falls back without leaking provider details. |
| AC-007 | Unit test verifies "I am newly married" maps to life-event response and avoids generic-only disclaimer. |
| AC-008 | Source test verifies post-quote quick replies are "I'm happy to go ahead" and "Talk to an agent". |
| AC-009 | Source test verifies contact-detail steps include name, date of birth, postcode, email, phone number, and address. |
| AC-010 | Source test verifies Continue application and Talk to an agent are set after contact details are captured. |
| AC-011 | `dotnet build` and full `TalLifeShield.Tests` run. |
| AC-012 | `Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\grounded-llm-pii-flow -RequireFinal` |
