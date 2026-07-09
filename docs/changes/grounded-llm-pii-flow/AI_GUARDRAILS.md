# AI Guardrails

- LLM output is allowed only for customer-facing wording.
- LLM output must not change intent, quote values, quick replies, workflow steps, PII flags, API payloads, or backend decisions.
- LLM prompts must include the deterministic backend response and relevant knowledge base grounding.
- LLM prompts must instruct the model to avoid personal advice, underwriting decisions, eligibility guarantees, claims decisions, and facts outside the supplied grounding.
- If LLM output is empty, invalid, unsafe, or the provider call fails, return the deterministic backend response.
- API keys must remain runtime environment variables and must not be written to repository files.
- Contact details are requested only after an indicative quote and explicit customer continuation confirmation.
