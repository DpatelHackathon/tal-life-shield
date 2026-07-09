# Traceability Matrix

| Req ID | AC ID | Design Decision | Implementation Task | Source Files / Modules | Test Case | Verification Result |
| --- | --- | --- | --- | --- | --- | --- |
| R-001 | AC-001 | Use scoped SDD package. | Author and gate docs. | `docs/changes/grounded-llm-pii-flow/*` | SDD gate | Pass |
| R-002 | AC-002 | Backend owns free-text routing. | Remove frontend keyword branches. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendFreeTextUsesBackendRouting` | Pass |
| R-003 | AC-003 | Add response generator abstraction. | Implement LLM generator and DI. | `src/TalLifeShield.Core/IntentClassification.cs`, `src/TalLifeShield.Api/Program.cs` | `GroundedLlmResponseGeneratorExists` | Pass |
| R-003 | AC-004 | Preserve deterministic metadata. | Replace message only when LLM succeeds. | `src/TalLifeShield.Core/IntentClassification.cs` | `LlmResponsePreservesChatMetadata` | Pass |
| R-004 | AC-005 | Use explicit grounding instructions. | Add guarded response prompt. | `src/TalLifeShield.Core/IntentClassification.cs` | `LlmResponseInstructionsAreGrounded` | Pass |
| R-005 | AC-006 | Fail closed to deterministic response. | Add resilient response generator. | `src/TalLifeShield.Core/IntentClassification.cs` | `LlmResponseFailureFallsBack` | Pass |
| R-006 | AC-007 | Add relationship life-event grounding. | Extend KB/router and test. | `src/TalLifeShield.Core/KnowledgeBase.cs`, `src/TalLifeShield.Core/IntentRouter.cs` | `NewlyMarriedGetsLifeEventResponse` | Pass |
| R-007 | AC-008 | Gate contact flow after quote confirmation. | Change post-quote quick replies. | `src/TalLifeShield.Api/wwwroot/app.js` | `PostQuoteRequiresProceedConfirmation` | Pass |
| R-008 | AC-009 | Collect required contact details. | Add PII step state machine. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendCollectsContactDetailsAfterConfirmation` | Pass |
| R-009 | AC-010 | Show Continue/Talk after contact capture. | Set final post-contact quick replies. | `src/TalLifeShield.Api/wwwroot/app.js` | `ContinueOptionsAppearAfterContactDetails` | Pass |
| R-010 | AC-011 | Preserve existing behavior. | Run full test suite. | `tests/TalLifeShield.Tests/Program.cs` | Full test run | Pass |
| R-001 | AC-012 | Close final evidence. | Add final summary and final gate. | `docs/changes/grounded-llm-pii-flow/FINAL_SDLC_SUMMARY.md` | Final SDD gate | Pass |
