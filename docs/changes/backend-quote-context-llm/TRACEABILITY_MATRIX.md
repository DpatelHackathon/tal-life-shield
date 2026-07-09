# Traceability Matrix

| Req ID | AC ID | Design Decision | Implementation Task | Source Files / Modules | Test Case | Verification Result |
| --- | --- | --- | --- | --- | --- | --- |
| R-001 | AC-001 | Use scoped SDD docs. | Author and gate docs. | `docs/changes/backend-quote-context-llm/*` | SDD gate | Pass |
| R-002 | AC-002 | Document current defect. | Add audit evidence. | `AUDIT.md` | `QuickQuoteAuditDocumentsFixedFrontendDefect` | Pass |
| R-003 | AC-003 | Frontend delegates quote presentation. | Replace direct quote card assembly. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendUsesBackendQuotePresentationEndpoint` | Pass |
| R-004 | AC-004 | Backend owns quote orchestration. | Add service and endpoint. | `src/TalLifeShield.Core/QuickQuotePresentation.cs`, `src/TalLifeShield.Api/Program.cs` | `BackendQuickQuotePresentationUsesApiResponses` | Pass |
| R-004 | AC-005 | Premium varies from stored quote inputs. | Update quote service calculation. | `src/TalLifeShield.Core/MockEligibilityService.cs` | `PremiumCalculationVariesByQuoteInputs` | Pass |
| R-005 | AC-006 | LLM receives backend JSON and history. | Add LLM quote presenter. | `src/TalLifeShield.Core/QuickQuotePresentation.cs` | `LlmQuickQuotePresenterReceivesApiAndHistoryGrounding` | Pass |
| R-006 | AC-007 | Backend numeric values are authoritative. | Preserve backend premium after LLM. | `src/TalLifeShield.Core/QuickQuotePresentation.cs` | `LlmQuickQuotePresenterPreservesBackendPremium` | Pass |
| R-006 | AC-008 | Deterministic fallback uses API values. | Add fallback presenter. | `src/TalLifeShield.Core/QuickQuotePresentation.cs` | `LlmQuickQuoteFallbackUsesApiValues` | Pass |
| R-007 | AC-009 | Frontend passes history. | Add history projection to requests. | `src/TalLifeShield.Api/wwwroot/app.js`, `src/TalLifeShield.Api/Program.cs` | `FrontendSendsConversationHistory` | Pass |
| R-008 | AC-010 | Ask only missing fields. | Preserve answers and skip answered steps. | `src/TalLifeShield.Api/wwwroot/app.js` | `FrontendAsksOnlyMissingQuoteQuestions` | Pass |
| R-009 | AC-011 | Preserve PII flow. | Keep existing contact sequence. | `src/TalLifeShield.Api/wwwroot/app.js` | Existing PII tests | Pass |
| R-010 | AC-012 | Verify all behavior. | Build and test. | `tests/TalLifeShield.Tests/Program.cs` | Full test run | Pass |
| R-001 | AC-013 | Close evidence. | Final summary and final gate. | `FINAL_SDLC_SUMMARY.md` | Final SDD gate | Pass |
