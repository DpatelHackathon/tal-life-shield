# Audit Findings

## Finding 1: Frontend Builds Quote Card From Fixed Values

The current `src/TalLifeShield.Api/wwwroot/app.js` quote flow calls `/api/quotes` and `/api/quotes/Q3350008/premium-calculation` directly, then assembles a quote card in the browser using fixed values including quote reference `Q3350008`, Life Cover `$5,000,000`, occupation code `1067`, and static row labels.

Impact: The user sees the same quote structure every time and the LLM does not read the backend API response to convert it into display details.

## Finding 2: Premium Calculation Is Static

`MockEligibilityService.CalculateQuotePremium` returns `SampleLifePremium` for every request.

Impact: Even when user details differ, the quick quote premium does not vary.

## Finding 3: Conversation History Is Not Sent To Backend

The frontend sends only the current message and a static quote state to `/api/chat/message`.

Impact: Backend AI response generation cannot use prior turns to avoid repeated questions or improve grounded context.
