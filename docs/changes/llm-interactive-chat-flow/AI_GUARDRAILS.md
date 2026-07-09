# AI Guardrails

## LLM Scope

The LLM is used for intent classification only. It must not generate free-form insurance advice for the customer in this change.

## Grounding

Customer-facing responses continue to come from:

- knowledge base
- backend API responses
- conversation context
- deterministic quote flow state

## Secret Handling

- API keys are read only from environment variables.
- API keys must not be written to repository files, logs, browser assets, or final output.
- LLM errors shown to customers must not include endpoint URLs, keys, exception details, stack traces, or provider diagnostics.

## Fallback

If LLM configuration is missing or a classifier call fails, the app uses deterministic fallback classification and continues safely.

## Advice Boundary

The assistant gives general information only and does not make personal product recommendations, eligibility guarantees, underwriting decisions, or claims outcomes.

