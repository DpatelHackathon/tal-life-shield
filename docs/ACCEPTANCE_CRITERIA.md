# Acceptance Criteria

| ID | Acceptance Criterion | Verification Method |
| --- | --- | --- |
| AC-001 | SDLC artifacts are authored before implementation starts, including SPEC, ACs, traceability, dependency graph, implementation plan, test plan, guardrails, and SPEC verification. | Document review in `SPEC_VERIFICATION.md`. |
| AC-002 | Traceability matrix maps each AC to requirement, design decision, implementation task, source files/modules, test case, and verification result. | Automated traceability test plus document review. |
| AC-003 | A dependency graph exists and the implementation plan is ordered from that graph. | Document review. |
| AC-004 | React frontend renders a TAL CoverBuilder-like homepage with brand/header, hero copy, CTA, and floating chat launcher. | Frontend smoke/manual verification and source review. |
| AC-005 | Chat widget supports open/close, bot/user messages, quick replies, free text input, typing state, quote card, final quote, and handoff. | Frontend smoke/manual verification and source review. |
| AC-006 | Local knowledge base includes LI, IP, CI, TPD, life-event scenarios, disclaimers, underwriting handoff, and support fallback content aligned to the demo. | Automated knowledge base test. |
| AC-007 | Chat responses are grounded in local KB or mock API responses and unsupported questions use a safe fallback without personal advice. | Automated guardrail tests. |
| AC-008 | Intent router detects all required intents: ask_about_cover, life_event_discovery, compare_covers, start_quote, provide_basic_quote_details, search_occupation, select_cover, calculate_premium, update_quote, collect_contact_details, underwriting_handoff, fallback_unknown. | Automated intent tests. |
| AC-009 | .NET backend exposes mock endpoint equivalents for eligibility, occupations, product covers, create quote, select cover, premium calculation, and update quote. | Automated backend service tests and endpoint source review. |
| AC-010 | Product covers include LI, IP, CI, TPD and occupation search for `acc` includes supplied accountant examples including code 1067. | Automated backend data tests. |
| AC-011 | Quote flow collects minimum non-PII quote details before premium calculation. | Automated chat flow test. |
| AC-012 | PII fields are requested only after the customer chooses to continue after an indicative quote. | Automated chat flow privacy test. |
| AC-013 | Life Insurance sample premium calculation returns premium amount 220.93 and update quote returns total premium 220.93, policy fee 8.0, and stamp duty 0.0. | Automated premium tests. |
| AC-014 | UI is responsive and includes accessible labels/keyboard-friendly controls for chat launcher, close, input, and send actions. | Frontend smoke/manual verification and source review. |
| AC-015 | All automated tests pass before completion is declared. | Test run evidence. |
| AC-016 | Final SDLC summary reports AC-by-AC completion status, limitations, and verification evidence. | Final document review. |
| AC-017 | Chat flow includes the demo-inspired optional 5% discount path, height/weight prompt after the initial quote, final discounted quote presentation, CSO phone/email fallback, and workflow status steps. | Automated chat orchestration test plus frontend smoke/manual verification. |
| AC-018 | API contract mapping preserves field-level coverage for supplied payloads, including cover levels, benefit/waiting periods, quote metadata, application quote ID, product/pricing versions, discount/BMI fields, payment frequency, and projections. | Automated backend contract test plus document review. |
