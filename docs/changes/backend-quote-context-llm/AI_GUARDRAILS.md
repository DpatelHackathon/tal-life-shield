# AI Guardrails

- LLM quick quote presentation receives only conversation history, quote answers, create quote API response, premium calculation API response, and deterministic fallback presentation.
- LLM must return constrained JSON for display only.
- Backend must preserve numeric premium, sum insured, quote reference, and cover code from API responses.
- LLM must not invent premiums, eligibility status, underwriting outcomes, discounts, product rules, or contact details.
- If LLM output is invalid, empty, unsafe, or unavailable, return deterministic presentation built from backend API responses.
- API keys remain runtime configuration and must not be committed.
- PII collection timing remains after quote and explicit continuation only.
