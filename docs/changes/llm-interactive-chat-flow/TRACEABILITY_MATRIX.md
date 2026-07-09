# Traceability Matrix

| Req ID | AC ID | Design Decision | Implementation Task | Source Files / Modules | Test Case | Verification Result |
| --- | --- | --- | --- | --- | --- | --- |
| R-001 | AC-001 | Use scoped SDD change docs. | Author docs and run SPEC gate. | `docs/changes/llm-interactive-chat-flow/*` | SDD gate | Pass |
| R-002 | AC-002 | Frontend owns turn-by-turn guided quote state. | Replace auto-answer quote script with user-driven state machine. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendDoesNotAutoAnswerAgentQuestions` | Pass |
| R-002 | AC-003 | Quote answers are captured from user messages. | Track smoker, birth year, gender, occupation before quote. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendWaitsForRequiredQuoteAnswers` | Pass |
| R-003 | AC-004 | Quote completion exposes explicit user choices. | Show Continue application and Talk to an agent options. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendShowsPostQuoteChoices` | Pass |
| R-003 | AC-005 | Talk to agent uses KB support number. | Implement talk-to-agent branch. | `src/TalLifeShield.Api/wwwroot/app.js`, `src/TalLifeShield.Core/ChatOrchestrator.cs` | `TalkToAgentShowsContactNumber` | Pass |
| R-004 | AC-006 | Intent routing depends on classifier abstraction. | Add LLM intent classifier and inject into orchestrator. | `src/TalLifeShield.Core/IntentClassification.cs`, `src/TalLifeShield.Api/Program.cs` | `LlmIntentClassifierMapsModelIntent` | Pass |
| R-004 | AC-007 | LLM calls Responses API-compatible endpoint. | Implement HTTP client and config. | `src/TalLifeShield.Core/IntentClassification.cs` | `OpenAiClassifierUsesResponsesEndpointAndConfig` | Pass |
| R-004 | AC-008 | Secrets stay in environment variables. | Add config docs and secret scan. | `docs/changes/llm-interactive-chat-flow/*`, `tests/TalLifeShield.Tests/Program.cs` | `RepositoryDoesNotContainAttachedApiKey` | Pass |
| R-004 | AC-009 | Failures fall back safely. | Add fallback classifier wrapper. | `src/TalLifeShield.Core/IntentClassification.cs` | `LlmFailureFallsBackWithoutLeakingDetails` | Pass |
| R-005 | AC-010 | LLM only classifies intent; responses stay grounded. | Preserve orchestrator response generation. | `src/TalLifeShield.Core/ChatOrchestrator.cs` | `GuardrailsRejectUnsupportedAdvice` | Pass |
| R-005 | AC-011 | Existing behavior remains covered. | Update tests and run full suite. | `tests/TalLifeShield.Tests/Program.cs` | Full test run | Pass |
| R-001 | AC-012 | Final gate closes evidence. | Update matrix and final summary. | `docs/changes/llm-interactive-chat-flow/FINAL_SDLC_SUMMARY.md` | Final SDD gate | Pass |
