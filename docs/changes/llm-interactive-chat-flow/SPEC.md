# SPEC: LLM Intent Routing And Interactive Quote Flow

## Purpose

Update the TAL Life Shield chat agent so it waits for the customer to answer each question, presents post-quote choices, and uses an AI LLM for intent classification instead of primary hard-coded intent routing.

## Source Inputs Reviewed

- User change request in the current Codex thread.
- `.codex/sdd/lead-agent-prompt.md`
- `.codex/sdd/agentic-sdlc.md`
- `.codex/sdd/gates.md`
- `.codex/sdd/skills.md`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\foundry_project_details.txt`
- `src/TalLifeShield.Core/ChatOrchestrator.cs`
- `src/TalLifeShield.Core/IntentRouter.cs`
- `src/TalLifeShield.Api/wwwroot/app.js`
- `src/TalLifeShield.Api/Program.cs`
- `tests/TalLifeShield.Tests/Program.cs`

Official API references checked:

- Microsoft Learn Azure OpenAI Responses API guidance.
- Microsoft Learn Azure OpenAI v1 API guidance.
- OpenAI Responses API reference.

## Secret Handling

The attached Foundry file contains an API key. The key must not be committed to source, docs, tests, browser assets, or logs. The application must read it from environment variables at runtime.

Required runtime configuration:

- `TAL_LIFE_SHIELD_OPENAI_ENDPOINT` or `AZURE_OPENAI_ENDPOINT`
- `TAL_LIFE_SHIELD_OPENAI_API_KEY` or `AZURE_OPENAI_API_KEY`
- `TAL_LIFE_SHIELD_OPENAI_MODEL` or `AZURE_OPENAI_DEPLOYMENT`

The attached Azure OpenAI endpoint uses the v1 base URL shape. The model/deployment name was not present in the attached file and must be provided by environment variable.

## Assumptions

- "Replace hard coded intent based routing" means the primary classifier must be LLM-backed when configuration is present.
- A deterministic fallback classifier is allowed only when LLM configuration is missing, the LLM call fails, or automated tests inject a fake classifier.
- Contact number for "talk to an agent" is TAL customer service `131 825`, already present in the existing knowledge base.
- The frontend should not auto-fill customer answers in the guided quote flow.

## Functional Requirements

### R-001 SDD Governance

This change must follow the repo-attached Agentic SDD workflow with scoped docs, SPEC gate, traceability, test evidence, final summary, and final gate.

### R-002 Interactive Question Flow

The chat agent must ask one question at a time and wait for the user to answer before moving to the next quick-quote question.

The quick-quote questions are:

- smoker status
- year of birth
- gender
- occupation

The frontend must not auto-answer the agent's own questions.

### R-003 Post-Quote Choices

After generating the quick quote, the chat agent must show two choices:

- Continue application
- Talk to an agent

If the user chooses Talk to an agent, the chat agent must present the TAL contact number for the user to call.

### R-004 LLM Intent Classification

The backend must use an LLM-backed intent classifier as the primary intent classification path when configured.

The classifier must:

- call the Azure/OpenAI Responses API-compatible endpoint
- request JSON-only intent output
- map output to the existing intent enum
- fall back safely to deterministic classification if the LLM is unavailable
- avoid exposing API errors or secrets to the customer

### R-005 Guardrails

The LLM must be used only for intent classification, not for free-form insurance advice in this change. Customer responses remain grounded in knowledge base, conversation context, and backend API responses.

## Non-Functional Requirements

- No API key is stored in repository files.
- Existing deterministic tests continue to run without a live LLM.
- LLM integration is testable via an injected fake client.
- Customer-facing wording remains production-ready and does not expose implementation labels.

## Out Of Scope

- Live validation of the attached API key.
- Discovering Azure deployment names from the Foundry project.
- Generating free-form LLM insurance advice.
- Replacing knowledge-base/API-grounded response generation with model-generated prose.

## SPEC Gate

Implementation may begin only after this scoped SDD package passes:

```powershell
.\scripts\Invoke-SddGate.ps1 -ChangeDocsPath docs\changes\llm-interactive-chat-flow
```

