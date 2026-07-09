# Test Plan

| AC ID | Verification |
| --- | --- |
| AC-001 | `Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\backend-quote-context-llm` |
| AC-002 | `QuickQuoteAuditDocumentsFixedFrontendDefect` |
| AC-003 | `FrontendUsesBackendQuotePresentationEndpoint` |
| AC-004 | `BackendQuickQuotePresentationUsesApiResponses` |
| AC-005 | `PremiumCalculationVariesByQuoteInputs` |
| AC-006 | `LlmQuickQuotePresenterReceivesApiAndHistoryGrounding` |
| AC-007 | `LlmQuickQuotePresenterPreservesBackendPremium` |
| AC-008 | `LlmQuickQuoteFallbackUsesApiValues` |
| AC-009 | `FrontendSendsConversationHistory` |
| AC-010 | `FrontendAsksOnlyMissingQuoteQuestions` |
| AC-011 | Existing PII sequencing tests plus `ContinueOptionsAppearAfterContactDetails` |
| AC-012 | `dotnet build` and full `TalLifeShield.Tests` run |
| AC-013 | `Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\backend-quote-context-llm -RequireFinal` |
