# SPEC: Grounded LLM Chat Responses and Post-Quote PII Flow

## Objective

Fix the TAL Life Shield chat agent so customer free-text messages are handled by the backend AI orchestration path, not frontend keyword routing, and so configured LLM support can generate grounded customer-facing responses using only the knowledge base, conversation intent, and backend API facts.

After an indicative quote is shown, the chat agent must wait for the customer to confirm they are happy to go ahead, then collect required contact details before showing Continue application or Talk to an agent choices.

## Source Inputs

- User request dated 2026-07-09.
- Screenshot showing repeated fixed response for "i am newly married".
- Existing SDD operating rules in `.codex/sdd`.
- Existing app source in `src/TalLifeShield.Api/wwwroot/app.js`.
- Existing backend orchestration in `src/TalLifeShield.Core`.
- Existing Azure/OpenAI runtime configuration from environment variables.

## Requirements

- R-001: Follow SPEC-driven development with a scoped change package and hard final gate.
- R-002: Free-text chat messages must be sent to backend chat orchestration so the AI/LLM path can classify and respond.
- R-003: The backend must support grounded LLM response generation when configured.
- R-004: LLM response generation must use only supplied grounding: knowledge base articles, deterministic backend response, intent, and API-derived facts.
- R-005: If LLM configuration fails or returns unusable content, the chat must fall back to deterministic safe responses.
- R-006: "newly married" and similar relationship life-event messages must produce a relevant grounded response, not the generic quote disclaimer.
- R-007: After an indicative quote, the next option must be customer confirmation to proceed or talk to an agent, not immediate Continue application.
- R-008: Once the customer confirms they are happy to proceed, the frontend must collect name, date of birth, postcode, email, phone number, and address details.
- R-009: Only after those contact details are captured may the frontend show Continue application or Talk to an agent options.
- R-010: Customer-facing text must remain production-ready and avoid non-production labels.

## Assumptions

- The LLM should augment response wording, not make underwriting, eligibility, or personal advice decisions.
- Quote calculation remains deterministic and uses the existing backend API endpoints.
- Contact details are held only in frontend session state for this prototype flow unless a later SPEC adds persistence.
