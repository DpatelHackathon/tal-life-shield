# TAL Life Shield Agentic Quote Assistant SPEC

## 1. Purpose

Build a local demo solution for a TAL CoverBuilder-like website with a React chat agent widget and a .NET mock backend. The assistant helps customers understand insurance cover options, provide the minimum information needed for an indicative quote, receive a sample premium, and optionally continue by providing additional contact details.

The solution is SPEC-driven. Implementation must not begin until the acceptance criteria, traceability matrix, dependency graph, implementation plan, test plan, guardrails, and SPEC verification gate are authored and reviewed.

## 2. Source Inputs Reviewed

Reference files:

- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\tal_life_shield_recording_mode_demo.html`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\createquote.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\GetEligibilityApplication.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\Getoccupations.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\GetProductCovers.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\getSelectCover.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\quotePremiumCalculation.txt`
- `C:\Users\dharmeshpatel\OneDrive - tal-hackiverse\Desktop\Hackathon\updateQuote.txt`

Key source facts extracted:

- Product covers are `LI`, `IP`, `CI`, and `TPD`.
- Product descriptions include Life Insurance Plan, Income Protection Plan, Critical Illness Insurance Plan, and TPD Insurance Plan.
- Occupation search for pattern `acc` returns accountant and account-related occupations, including code `1067`.
- Create quote returns quote reference `Q3350008` and eligibility flags.
- Select cover returns basic customer information, product cover eligibility, payment frequency, and quote state.
- Premium calculation sample returns Life Insurance premium `220.93`.
- Update quote sample returns total premium `220.93`, policy fee `8.0`, stamp duty `0.0`, and Life Insurance sum insured `5000000.0`.
- Eligibility application status is `NewBusiness`.
- Demo chat flow includes life event discovery, product explanation, quote detail capture, quote card, 5% discount question, height/weight capture, final discounted quote, underwriting link, customer service officer contact details, and workflow status steps.

## 3. Constraints And Assumptions

- The current workspace is empty and not a git repository.
- .NET SDK 7.0.401 is available.
- Node and npm are not available on PATH.
- To satisfy the React frontend requirement without a Node build chain, the frontend will be implemented as static React assets served by the .NET backend. React will be loaded in the browser using UMD scripts, and app code will use `React.createElement` rather than JSX.
- The backend is a mock/demo implementation. It must preserve the important shape and fields from supplied API payloads but does not call real TAL systems.
- All premium, quote, eligibility, and underwriting content is demo-only and indicative.

## 3.1 Demo Deviations

The supplied demo collects first name, last name, mobile number, email address, and postcode before the first quote is shown. This solution intentionally deviates from that demo sequence to satisfy the privacy requirement in the user request: PII must be requested only after the customer has seen an indicative quote and chooses to continue.

To keep API payload compatibility while honoring that rule, backend mock endpoints may accept optional PII fields, but the chat flow must not request or require those fields before premium calculation. Pre-continuation quote calculations must use non-PII quote details only.

## 4. Roles And Agent Workstreams

The Lead Agent owns overall SDLC flow, verification gates, and final integration.

Specialist workstreams:

- SPEC and Traceability Agent: checks that requirements, ACs, plan items, source files, and test evidence remain linked.
- Backend Agent: owns .NET mock endpoints and mock payload fidelity.
- AI Orchestration Agent: owns knowledge base lookup, intent routing, guardrails, and chat state transitions.
- Frontend Agent: owns TAL-like home page, React widget, quote cards, responsive UI, and accessibility behavior.
- Test Agent: owns repeatable verification, guardrail tests, and hard completion gate evidence.
- Privacy and Safety Agent: owns PII timing, general advice wording, and anti-hallucination checks.

## 5. Functional Requirements

### R-001 SPEC Driven Delivery

The project must include SDLC artifacts before implementation:

- `docs/SPEC.md`
- `docs/ACCEPTANCE_CRITERIA.md`
- `docs/TRACEABILITY_MATRIX.md`
- `docs/DEPENDENCY_GRAPH.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/TEST_PLAN.md`
- `docs/AI_GUARDRAILS.md`
- `docs/SPEC_VERIFICATION.md`

### R-002 TAL CoverBuilder-Like Homepage

The frontend must render a TAL CoverBuilder-like home page with:

- Header and brand treatment.
- Hero copy including "Protect what matters most."
- Insurance navigation cues.
- Floating chat launch button.
- Chat panel attached to the page.

### R-003 Chat Widget Experience

The chat widget must provide:

- Open and close behavior.
- Bot and user messages.
- Quick reply buttons.
- Free text input.
- Typing/loading state.
- Product explanation messages.
- Quote summary card.
- Final quote/handoff state.
- Underwriting continuation and customer support fallback language.
- Optional discount path that asks for height and weight after the initial indicative quote, applies the demo 5% discount presentation, and shows the final discounted quote.
- Workflow status steps: Quote Generated, Underwriting Questions, Review Cover, Submit Application, Application Received.

### R-004 Knowledge Base

Create a local, predefined knowledge base containing factual general information for:

- Life Insurance.
- Income Protection.
- Critical Illness.
- Total and Permanent Disability.
- Recently bought a home scenario.
- Starting a family scenario.
- New job or promotion scenario.
- Reviewing existing cover scenario.
- Quote disclaimers.
- Underwriting handoff.
- Customer support fallback.

### R-005 Guarded AI Orchestration

Implement a deterministic, LLM-compatible orchestration layer that:

- Detects customer intent.
- Retrieves knowledge base facts.
- Calls mock backend APIs when a quote action is needed.
- Does not invent unsupported product facts, prices, eligibility, or underwriting outcomes.
- Gives general information only, not personal financial advice.
- Provides a safe fallback when information is unavailable.

### R-006 Intent Detection

The intent layer must support at least:

- `ask_about_cover`
- `life_event_discovery`
- `compare_covers`
- `start_quote`
- `provide_basic_quote_details`
- `search_occupation`
- `select_cover`
- `calculate_premium`
- `update_quote`
- `collect_contact_details`
- `underwriting_handoff`
- `fallback_unknown`

### R-007 Mock Backend API

Create .NET mock endpoints based on supplied payloads:

- `GET /api/eligibility/application`
- `GET /api/occupations?pattern=acc`
- `GET /api/product-covers`
- `POST /api/quotes`
- `GET /api/quotes/{quoteRefNo}/select-cover`
- `POST /api/quotes/{quoteRefNo}/premium-calculation`
- `PUT /api/quotes/{quoteRefNo}`

The endpoints must preserve important source fields:

- `quoteRefNo`
- `caseId`
- `basicInformation`
- `quickQuote`
- `productCover`
- `productBenefits`
- `sumInsured`
- `premiumAmt`
- `totalPremiumAmt`
- `policyFeeAmount`
- `stampDutyAmount`
- `paymentFrequency`
- `projections`
- `applicationStatus`

### R-007.1 API Contract Mapping

| Source File | Mock Endpoint | Required Request Handling | Required Response Fields |
| --- | --- | --- | --- |
| `GetEligibilityApplication.txt` | `GET /api/eligibility/application` | No request body. | `data.getEligibilityApplication.applicationStatus.status.code`, `description`, `__typename`. |
| `Getoccupations.txt` | `GET /api/occupations?pattern=acc` | Accepts occupation pattern and filters sample records. | `occupations[].code`, `occupations[].description`, `__typename`; must include code `1067` and accountant examples. |
| `GetProductCovers.txt` | `GET /api/product-covers` | No request body. | `productCovers[].code`, `description`, `coverLevels[].code`, `description`, `isDefault`, `benefitPeriod[].code`, `description`, `waitingPeriod[].code`, `description`, `__typename`; must include `CI`, `IP`, `LI`, `TPD`. |
| `createquote.txt` | `POST /api/quotes` | Accepts `basicInformation`, `mediaCode`, `sourceOfBusiness`, `recaptchaToken`, `refSession`, and `refSessionId`; mock may ignore recaptcha validation but must accept metadata. | `quoteRefNo`, `basicInformation.firstName`, `lastName`, `birthDate`, `gender.code`, `isSmoker`, `occupation.code`, `quickQuote.covers[].sumInsured`, `premiumAmt`, `isEligible`. |
| `getSelectCover.txt` | `GET /api/quotes/{quoteRefNo}/select-cover` | Uses quote reference path parameter. | `quoteRefNo`, `caseId`, `basicInformation`, `isApplyOverPhone`, `isIpExtendEligible`, `productSeries`, `productVersion`, `pricingVersion`, `quickQuote.applicationQuoteLifeId`, `totalPremiumAmt`, `policyFeeAmount`, `stampDutyAmount`, `isDiscountSelected`, `bMIRating`, `weightKg`, `heightCm`, `paymentFrequency`, `covers`, `projections`. |
| `quotePremiumCalculation.txt` | `POST /api/quotes/{quoteRefNo}/premium-calculation` | Accepts eligibility/quote payload. | `quickQuote.covers[].productCover.code`, `description`, `premiumAmt`; Life Insurance sample must return `220.93`. |
| `updateQuote.txt` | `PUT /api/quotes/{quoteRefNo}` | Accepts eligibility/selected cover payload including optional height/weight for discount path. | `quoteRefNo`, `productSeries`, `productVersion`, `pricingVersion`, `basicInformation`, `isApplyOverPhone`, `quickQuote.applicationQuoteLifeId`, `totalPremiumAmt`, `policyFeeAmount`, `stampDutyAmount`, `isBMIDiscountEligible`, `isDiscountSelected`, `bMIRating`, `weightKg`, `heightCm`, `paymentFrequency`, `covers`, `projections`. |

Deliberate omissions:

- Real recaptcha verification is not implemented.
- Real TAL session references are not implemented.
- Real underwriting outcomes are not implemented.
- Mock endpoints preserve required field names and representative values, not full GraphQL execution semantics.

### R-008 Quote Flow

Before calculating a premium, the chat flow must collect minimum quote details:

- Date/year of birth or age.
- Gender/sex where required by the mock model.
- Smoker status.
- Occupation.
- Employment/self-employed status where needed.
- Annual income where relevant.
- Desired cover and sum insured or monthly benefit.

The quote flow must use the mock backend service and display the supplied sample premium when the Life Insurance sample path is selected.

### R-009 PII Collection Timing

The assistant must request contact PII only after the customer chooses to continue after seeing an indicative quote. PII includes:

- First name.
- Last name.
- Email.
- Phone.
- Postcode.

### R-010 Testing And Verification

Automated tests must verify:

- Mock backend response structures.
- Product covers include `LI`, `IP`, `CI`, and `TPD`.
- Occupation search returns supplied accountant examples.
- Premium calculation returns `220.93` for the sample Life Insurance flow.
- Intent detection maps known customer utterances correctly.
- Guardrails prevent unsupported/personal-advice hallucination.
- Quote flow collects minimum details before premium calculation.
- PII is requested only after quote continuation.
- Traceability matrix includes every AC and test evidence.

### R-011 Hard Completion Gate

The project cannot be declared complete until:

- Tests pass.
- All ACs have implementation mapping.
- All ACs have test or documented verification evidence.
- Guardrail tests pass.
- Final SDLC summary is authored.

## 6. Non-Functional Requirements

- UI must be responsive on desktop and mobile.
- UI must use accessible labels, keyboard-friendly controls, and readable contrast.
- Backend must be deterministic and safe for local demo use.
- Data in responses must be sample/mock/demo only.
- The app must run locally with one .NET backend process.

## 7. Architecture

Planned structure:

- `src/TalLifeShield.Core`: mock data, knowledge base, intent routing, chat orchestration, quote flow logic.
- `src/TalLifeShield.Api`: .NET minimal API, static frontend host, mock endpoint mapping.
- `src/TalLifeShield.Api/wwwroot`: static React frontend assets.
- `tests/TalLifeShield.Tests`: console-based automated verification suite.
- `docs`: SDLC audit artifacts and final evidence.

This architecture keeps domain behavior testable without external packages and allows the frontend to consume the backend through HTTP endpoints.

## 8. Out Of Scope

- Real TAL API integration.
- Real LLM integration or external model calls.
- Real underwriting decisions.
- Real customer data storage.
- Production authentication, authorization, or deployment hardening.
- Product advice or personal financial advice.

## 9. SPEC Gate

Implementation may start only after `docs/SPEC_VERIFICATION.md` confirms:

- All supplied API payload files are represented.
- All demo conversation stages are represented.
- All guardrails are represented.
- All ACs are testable.
- Every AC has a planned verification method.
