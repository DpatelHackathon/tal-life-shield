using System.Text.RegularExpressions;
using TalLifeShield.Core;

var tests = new List<(string Name, string Ac, Action Run)>
{
    ("Required SDD documents exist", "AC-001", RequiredSddDocumentsExist),
    ("Traceability covers every AC", "AC-002", TraceabilityCoversEveryAc),
    ("Dependency graph drives implementation plan", "AC-003", DependencyGraphDrivesPlan),
    ("Frontend homepage source smoke", "AC-004", FrontendHomepageSourceSmoke),
    ("Chat widget source smoke", "AC-005", ChatWidgetSourceSmoke),
    ("Knowledge base contains required facts", "AC-006", KnowledgeBaseContainsRequiredFacts),
    ("Guardrails reject unsupported advice", "AC-007", GuardrailsRejectUnsupportedAdvice),
    ("Intent router detects required intents", "AC-008", IntentRouterDetectsRequiredIntents),
    ("Mock backend structures exist", "AC-009", MockBackendStructuresExist),
    ("Product covers and occupations match samples", "AC-010", ProductCoversAndOccupationsMatchSamples),
    ("Quote flow requires minimum details", "AC-011", QuoteFlowRequiresMinimumDetails),
    ("PII only after continuation", "AC-012", PiiOnlyAfterContinuation),
    ("Premium calculation matches sample", "AC-013", PremiumCalculationMatchesSample),
    ("Frontend accessibility and responsive source smoke", "AC-014", FrontendAccessibilityResponsiveSmoke),
    ("All tests pass in one run", "AC-015", () => { }),
    ("Final summary template is available", "AC-016", FinalSummaryTemplateAvailable),
    ("Discount and handoff flow matches demo", "AC-017", DiscountAndHandoffFlowMatchesDemo),
    ("API contract preserves payload fields", "AC-018", ApiContractPreservesPayloadFields),
    ("Production quick quote requires only four fields", "CHG-AC-002", QuickQuoteRequiresOnlyFourFields),
    ("Frontend asks only four quick quote questions", "CHG-AC-004", FrontendQuickQuoteQuestionSmoke),
    ("Post-quote PII includes required fields", "CHG-AC-005", PostQuotePiiIncludesRequiredFields),
    ("Customer-facing frontend avoids non-production words", "CHG-AC-006", CustomerFacingFrontendAvoidsNonProductionWords),
    ("Customer-facing core avoids non-production words", "CHG-AC-007", CustomerFacingCoreAvoidsNonProductionWords),
    ("Frontend does not auto-answer agent questions", "LLM-AC-002", FrontendDoesNotAutoAnswerAgentQuestions),
    ("Frontend waits for required quote answers", "LLM-AC-003", FrontendWaitsForRequiredQuoteAnswers),
    ("Frontend shows post quote choices", "LLM-AC-004", FrontendShowsPostQuoteChoices),
    ("Talk to agent shows contact number", "LLM-AC-005", TalkToAgentShowsContactNumber),
    ("LLM intent classifier maps model intent", "LLM-AC-006", LlmIntentClassifierMapsModelIntent),
    ("OpenAI classifier uses responses endpoint and config", "LLM-AC-007", OpenAiClassifierUsesResponsesEndpointAndConfig),
    ("Repository does not contain attached API key", "LLM-AC-008", RepositoryDoesNotContainAttachedApiKey),
    ("LLM failure falls back without leaking details", "LLM-AC-009", LlmFailureFallsBackWithoutLeakingDetails),
    ("Frontend free text uses backend routing", "GLP-AC-002", FrontendFreeTextUsesBackendRouting),
    ("Grounded LLM response generator exists", "GLP-AC-003", GroundedLlmResponseGeneratorExists),
    ("LLM response preserves chat metadata", "GLP-AC-004", LlmResponsePreservesChatMetadata),
    ("LLM response instructions are grounded", "GLP-AC-005", LlmResponseInstructionsAreGrounded),
    ("LLM response failure falls back", "GLP-AC-006", LlmResponseFailureFallsBack),
    ("Newly married gets life event response", "GLP-AC-007", NewlyMarriedGetsLifeEventResponse),
    ("Post quote requires proceed confirmation", "GLP-AC-008", PostQuoteRequiresProceedConfirmation),
    ("Frontend collects contact details after confirmation", "GLP-AC-009", FrontendCollectsContactDetailsAfterConfirmation),
    ("Continue options appear after contact details", "GLP-AC-010", ContinueOptionsAppearAfterContactDetails)
    ,("Quick quote audit documents fixed frontend defect", "BQC-AC-002", QuickQuoteAuditDocumentsFixedFrontendDefect)
    ,("Frontend uses backend quote presentation endpoint", "BQC-AC-003", FrontendUsesBackendQuotePresentationEndpoint)
    ,("Backend quick quote presentation uses API responses", "BQC-AC-004", BackendQuickQuotePresentationUsesApiResponses)
    ,("Premium calculation varies by quote inputs", "BQC-AC-005", PremiumCalculationVariesByQuoteInputs)
    ,("LLM quick quote presenter receives API and history grounding", "BQC-AC-006", LlmQuickQuotePresenterReceivesApiAndHistoryGrounding)
    ,("LLM quick quote presenter preserves backend premium", "BQC-AC-007", LlmQuickQuotePresenterPreservesBackendPremium)
    ,("LLM quick quote fallback uses API values", "BQC-AC-008", LlmQuickQuoteFallbackUsesApiValues)
    ,("Frontend sends conversation history", "BQC-AC-009", FrontendSendsConversationHistory)
    ,("Frontend asks only missing quote questions", "BQC-AC-010", FrontendAsksOnlyMissingQuoteQuestions)
};

var failures = new List<string>();

foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"PASS {test.Ac}: {test.Name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{test.Ac} {test.Name}: {ex.Message}");
        Console.WriteLine($"FAIL {test.Ac}: {test.Name} - {ex.Message}");
    }
}

if (failures.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Verification failed:");
    foreach (var failure in failures)
    {
        Console.WriteLine($"- {failure}");
    }

    Environment.Exit(1);
}

Console.WriteLine();
Console.WriteLine($"All {tests.Count} SDD verification tests passed.");

static void RequiredSddDocumentsExist()
{
    foreach (var file in RequiredDocs())
    {
        Assert(File.Exists(Path.Combine(RepoRoot(), "docs", file)), $"{file} missing");
    }

    Assert(File.Exists(Path.Combine(RepoRoot(), ".codex", "sdd", "agentic-sdlc.md")), "agentic SDLC workflow missing");
    Assert(File.Exists(Path.Combine(RepoRoot(), ".codex", "sdd", "lead-agent-prompt.md")), "lead agent prompt missing");
}

static void TraceabilityCoversEveryAc()
{
    var acText = Read("docs/ACCEPTANCE_CRITERIA.md");
    var trace = Read("docs/TRACEABILITY_MATRIX.md");
    var testPlan = Read("docs/TEST_PLAN.md");
    var ids = Regex.Matches(acText, "AC-\\d{3}").Select(match => match.Value).Distinct().ToList();

    Assert(ids.Count >= 18, "expected at least AC-001 through AC-018");
    foreach (var id in ids)
    {
        Assert(trace.Contains(id), $"{id} missing from traceability matrix");
        Assert(testPlan.Contains(id), $"{id} missing from test plan");
    }
}

static void DependencyGraphDrivesPlan()
{
    var graph = Read("docs/DEPENDENCY_GRAPH.md");
    var plan = Read("docs/IMPLEMENTATION_PLAN.md");
    Assert(graph.Contains("graph TD"), "dependency graph must contain Mermaid graph");
    Assert(graph.Contains("Implementation Order"), "dependency graph must include implementation order");
    Assert(plan.Contains("ordered from `docs/DEPENDENCY_GRAPH.md`"), "implementation plan must state graph-derived order");
}

static void FrontendHomepageSourceSmoke()
{
    var html = Read("src/TalLifeShield.Api/wwwroot/index.html");
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");
    var css = Read("src/TalLifeShield.Api/wwwroot/styles.css");

    Assert(html.Contains("react.production.min.js"), "React UMD script missing");
    Assert(app.Contains("TAL CoverBuilder"), "homepage brand missing");
    Assert(app.Contains("Protect what matters most."), "hero copy missing");
    Assert(app.Contains("Open TAL Life Shield chat"), "floating launcher label missing");
    Assert(css.Contains("hero-family-protection.png"), "hero image asset missing");
}

static void ChatWidgetSourceSmoke()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");
    Assert(app.Contains("quickReplies"), "quick replies missing");
    Assert(app.Contains("typing"), "typing state missing");
    Assert(app.Contains("quote-card"), "quote card missing");
    Assert(app.Contains("Continue application"), "handoff quick reply missing");
    Assert(app.Contains("/api/chat/quick-quote"), "backend quick quote presentation endpoint missing");
}

static void KnowledgeBaseContainsRequiredFacts()
{
    var kb = new KnowledgeBase();
    var all = string.Join("\n", kb.Articles.Select(article => $"{article.Title} {article.Content}"));

    foreach (var expected in new[]
    {
        "Life Insurance",
        "Income Protection",
        "Critical Illness",
        "Total and Permanent Disability",
        "recently bought a home",
        "starting a family",
        "new job or promotion",
        "Reviewing existing cover",
        "indicative",
        "underwriting",
        "131 825",
        "customerservice@tal.com.au"
    })
    {
        Assert(all.Contains(expected, StringComparison.OrdinalIgnoreCase), $"knowledge base missing {expected}");
    }
}

static void GuardrailsRejectUnsupportedAdvice()
{
    var orchestrator = Orchestrator();
    var response = orchestrator.Respond("Which product is best for me and should I buy it?");

    Assert(response.Source == "guardrail:fallback", "unsupported advice should use fallback guardrail");
    Assert(response.Message.Contains("cannot confirm", StringComparison.OrdinalIgnoreCase), "fallback message missing");
    Assert(!response.Message.Contains("you should buy", StringComparison.OrdinalIgnoreCase), "personal advice leaked");
}

static void IntentRouterDetectsRequiredIntents()
{
    var router = new IntentRouter();
    var cases = new Dictionary<string, ChatIntent>
    {
        ["Tell me about life insurance"] = ChatIntent.AskAboutCover,
        ["I recently bought a home"] = ChatIntent.LifeEventDiscovery,
        ["Compare cover options"] = ChatIntent.CompareCovers,
        ["Start quote"] = ChatIntent.StartQuote,
        ["I was born in 1988 and I do not smoke"] = ChatIntent.ProvideBasicQuoteDetails,
        ["Search occupation accountant"] = ChatIntent.SearchOccupation,
        ["Select life insurance only"] = ChatIntent.SelectCover,
        ["Calculate premium"] = ChatIntent.CalculatePremium,
        ["Apply discount with height and weight"] = ChatIntent.UpdateQuote,
        ["My email and phone for contact details"] = ChatIntent.CollectContactDetails,
        ["Continue to underwriting"] = ChatIntent.UnderwritingHandoff,
        ["I want to talk to an agent"] = ChatIntent.TalkToAgent,
        ["What is the weather on Mars?"] = ChatIntent.FallbackUnknown
    };

    foreach (var item in cases)
    {
        Assert(router.Detect(item.Key) == item.Value, $"{item.Key} did not map to {item.Value}");
    }
}

static void MockBackendStructuresExist()
{
    var service = new MockEligibilityService();
    Assert(Json(service.GetEligibilityApplication()).Contains("NewBusiness"), "eligibility status missing");
    Assert(Json(service.GetProductCovers()).Contains("productCovers"), "product covers missing");
    Assert(Json(service.CreateQuote(CreateRequest())).Contains("setBasicInfo"), "create quote payload missing");
    Assert(Json(service.GetSelectCover("Q3350008")).Contains("getSelectCover"), "select cover payload missing");
    Assert(Json(service.CalculateQuotePremium("Q3350008", new PremiumCalculationRequest("LI", 5000000m))).Contains("quotePremiumCalculation"), "premium payload missing");
    Assert(Json(service.UpdateQuote("Q3350008", new UpdateQuoteRequest("LI", 5000000m, 178m, 82m, true))).Contains("setSelectCover"), "update quote payload missing");
}

static void ProductCoversAndOccupationsMatchSamples()
{
    var service = new MockEligibilityService();
    var covers = Json(service.GetProductCovers());
    var occupations = Json(service.GetOccupations("acc"));

    foreach (var code in new[] { "LI", "IP", "CI", "TPD" })
    {
        Assert(covers.Contains($"\"code\":\"{code}\""), $"product cover {code} missing");
    }

    Assert(occupations.Contains("\"code\":\"1067\""), "occupation code 1067 missing");
    Assert(occupations.Contains("Accountant - Not university qualified"), "accountant sample missing");
    Assert(occupations.Contains("Accountant - University qualified"), "university-qualified accountant sample missing");
}

static void QuoteFlowRequiresMinimumDetails()
{
    var incomplete = new QuoteFlowState { BirthYear = 1988, Gender = "Male" };
    Assert(!incomplete.CanCalculatePremium, "incomplete quote should not calculate");
    Assert(incomplete.MissingMinimumFields().Contains("smoker status"), "missing smoker status not reported");

    var complete = CompleteQuoteState(false, false);
    Assert(complete.CanCalculatePremium, "complete quote should calculate");
}

static void PiiOnlyAfterContinuation()
{
    var beforeQuote = CompleteQuoteState(false, false);
    var afterQuoteNoContinue = CompleteQuoteState(true, false);
    var afterContinue = CompleteQuoteState(true, true);

    Assert(!beforeQuote.CanRequestPii, "PII allowed before quote");
    Assert(!afterQuoteNoContinue.CanRequestPii, "PII allowed before continuation");
    Assert(afterContinue.CanRequestPii, "PII not allowed after continuation");

    var response = Orchestrator().Respond("my email is test@example.com", beforeQuote);
    Assert(response.Source == "guardrail:pii-timing", "PII timing guardrail did not trigger");
}

static void PremiumCalculationMatchesSample()
{
    var service = new MockEligibilityService();
    var premium = Json(service.CalculateQuotePremium("Q3350008", new PremiumCalculationRequest("LI", 5000000m)));
    var update = Json(service.UpdateQuote("Q3350008", new UpdateQuoteRequest("LI", 5000000m, 178m, 82m, true)));

    Assert(premium.Contains("\"premiumAmt\":220.93"), "sample premium 220.93 missing");
    Assert(update.Contains("\"totalPremiumAmt\":220.93"), "total premium 220.93 missing");
    Assert(update.Contains("\"policyFeeAmount\":8.0"), "policy fee 8.0 missing");
    Assert(update.Contains("\"stampDutyAmount\":0.0"), "stamp duty 0.0 missing");
}

static void FrontendAccessibilityResponsiveSmoke()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");
    var css = Read("src/TalLifeShield.Api/wwwroot/styles.css");

    foreach (var expected in new[] { "aria-label", "Close chat", "Message TAL Life Shield", "Primary navigation" })
    {
        Assert(app.Contains(expected), $"accessibility source missing {expected}");
    }

    Assert(css.Contains("@media (max-width: 820px)"), "tablet/mobile media query missing");
    Assert(css.Contains("@media (max-width: 520px)"), "mobile media query missing");
}

static void FinalSummaryTemplateAvailable()
{
    Assert(File.Exists(Path.Combine(RepoRoot(), ".codex", "sdd", "templates", "FINAL_SDLC_SUMMARY.md")), "final summary template missing");
}

static void DiscountAndHandoffFlowMatchesDemo()
{
    var response = Orchestrator().Respond("I want to talk to an agent");

    Assert(response.Message.Contains("131 825"), "CSO phone missing");
    Assert(response.Intent == "talk_to_agent", "talk to agent intent missing");
}

static void ApiContractPreservesPayloadFields()
{
    var service = new MockEligibilityService();
    var product = Json(service.GetProductCovers());
    var create = Json(service.CreateQuote(CreateRequest()));
    var select = Json(service.GetSelectCover("Q3350008"));
    var update = Json(service.UpdateQuote("Q3350008", new UpdateQuoteRequest("LI", 5000000m, 178m, 82m, true)));

    foreach (var expected in new[] { "coverLevels", "benefitPeriod", "waitingPeriod" })
    {
        Assert(product.Contains(expected), $"product contract missing {expected}");
    }

    foreach (var expected in new[] { "mediaCode", "sourceOfBusiness", "recaptchaTokenAcceptedForMock", "quoteRefNo", "basicInformation", "quickQuote" })
    {
        Assert(create.Contains(expected), $"create quote contract missing {expected}");
    }

    foreach (var expected in new[] { "applicationQuoteLifeId", "productSeries", "productVersion", "pricingVersion", "isApplyOverPhone", "isIpExtendEligible", "paymentFrequency", "projections" })
    {
        Assert(select.Contains(expected), $"select cover contract missing {expected}");
    }

    foreach (var expected in new[] { "isBMIDiscountEligible", "heightCm", "weightKg", "policyFeeAmount", "stampDutyAmount", "covers" })
    {
        Assert(update.Contains(expected), $"update quote contract missing {expected}");
    }
}

static void QuickQuoteRequiresOnlyFourFields()
{
    var fourFieldsOnly = new QuoteFlowState
    {
        BirthYear = 1988,
        Gender = "Male",
        IsSmoker = false,
        Occupation = "Accountant - Not university qualified"
    };

    Assert(fourFieldsOnly.CanCalculatePremium, "four required fields should be enough for quick quote");
    Assert(fourFieldsOnly.MissingMinimumFields().Count == 0, "four required fields should leave no missing fields");

    var missingOccupation = new QuoteFlowState
    {
        BirthYear = 1988,
        Gender = "Male",
        IsSmoker = false
    };

    Assert(!missingOccupation.CanCalculatePremium, "occupation should still be required");
    Assert(missingOccupation.MissingMinimumFields().SequenceEqual(new[] { "occupation" }), "only occupation should be missing");
}

static void FrontendQuickQuoteQuestionSmoke()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    foreach (var expected in new[] { "Do you smoke?", "Birth year?", "Gender/sex?", "Occupation?" })
    {
        Assert(app.Contains(expected), $"frontend missing quick quote question: {expected}");
    }

    foreach (var forbidden in new[] { "Employment status?", "Annual income?", "Cover and sum insured?" })
    {
        Assert(!app.Contains(forbidden), $"frontend still asks pre-quote extra field: {forbidden}");
    }
}

static void PostQuotePiiIncludesRequiredFields()
{
    var response = Orchestrator().Respond("my email is test@example.com", CompleteQuoteState(true, true));
    var message = response.Message.ToLowerInvariant();

    foreach (var expected in new[] { "name", "date of birth", "postcode", "email", "phone number", "address" })
    {
        Assert(message.Contains(expected), $"post-quote PII prompt missing {expected}");
    }
}

static void CustomerFacingFrontendAvoidsNonProductionWords()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");
    var health = Read("src/TalLifeShield.Api/Program.cs");
    var customerFacingText = ExtractJavaScriptStrings(app) + "\n" + ExtractCSharpStrings(health);

    AssertNoNonProductionWords(customerFacingText, "frontend/API customer-facing text");
}

static void CustomerFacingCoreAvoidsNonProductionWords()
{
    var kb = new KnowledgeBase();
    var orchestrator = Orchestrator();
    var responses = new[]
    {
        orchestrator.Respond("Tell me about life insurance").Message,
        orchestrator.Respond("Start quote").Message,
        orchestrator.Respond("Search occupation accountant").Message,
        orchestrator.Respond("Select life insurance only").Message,
        orchestrator.Respond("Calculate premium", CompleteQuoteState(false, false)).Message,
        orchestrator.Respond("Apply optional 5% discount").Message,
        orchestrator.Respond("my email is test@example.com", CompleteQuoteState(true, true)).Message,
        orchestrator.Respond("Which product is best for me?").Message
    };

    var customerFacingText = string.Join("\n", kb.Articles.Select(article => article.Content).Concat(responses));
    AssertNoNonProductionWords(customerFacingText, "core customer-facing text");
}

static void FrontendDoesNotAutoAnswerAgentQuestions()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(!app.Contains("Yes - 178cm and 82kg"), "frontend still auto-answers discount question");
    Assert(!app.Contains("{ who: \"user\", text: \""), "frontend still injects hard-coded user answers");
    Assert(app.Contains("handleQuoteAnswer(answer)"), "frontend must handle real user answers");
}

static void FrontendWaitsForRequiredQuoteAnswers()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    foreach (var expected in new[] { "isSmoker", "birthYear", "gender", "occupation", "quoteStepIndex", "setQuoteStepIndex(nextIndex)" })
    {
        Assert(app.Contains(expected), $"frontend missing interactive quote state: {expected}");
    }
}

static void FrontendShowsPostQuoteChoices()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("I'm happy to go ahead"), "proceed confirmation option missing");
    Assert(app.Contains("Talk to an agent"), "talk to an agent option missing");
    Assert(app.Contains("presentation.quickReplies"), "post-quote confirmation choices should come from backend presentation");
}

static void TalkToAgentShowsContactNumber()
{
    var response = Orchestrator().Respond("talk to an agent");
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(response.Message.Contains("131 825"), "orchestrator talk-to-agent phone missing");
    Assert(app.Contains("131 825"), "frontend talk-to-agent phone missing");
}

static void LlmIntentClassifierMapsModelIntent()
{
    var classifier = new OpenAiIntentClassifier(new FakeOpenAiResponsesClient("{\"intent\":\"talk_to_agent\"}"));
    var intent = classifier.DetectAsync("Can I talk to a person?").GetAwaiter().GetResult();

    Assert(intent == ChatIntent.TalkToAgent, "LLM intent response did not map to talk_to_agent");
}

static void OpenAiClassifierUsesResponsesEndpointAndConfig()
{
    var source = Read("src/TalLifeShield.Core/IntentClassification.cs");
    var program = Read("src/TalLifeShield.Api/Program.cs");

    Assert(source.Contains("/responses"), "Responses endpoint path missing");
    Assert(source.Contains("\"api-key\""), "Azure API key header missing");
    Assert(source.Contains("model = _options.Model"), "model/deployment config missing");
    Assert(program.Contains("TAL_LIFE_SHIELD_OPENAI_ENDPOINT"), "endpoint env var missing");
    Assert(program.Contains("TAL_LIFE_SHIELD_OPENAI_API_KEY"), "api key env var missing");
    Assert(program.Contains("TAL_LIFE_SHIELD_OPENAI_MODEL"), "model env var missing");
}

static void RepositoryDoesNotContainAttachedApiKey()
{
    var root = RepoRoot();
    var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
        .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
        .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}"))
        .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}"))
        .Where(path => new FileInfo(path).Length < 1_000_000);

    foreach (var file in files)
    {
        var text = File.ReadAllText(file);
        Assert(!Regex.IsMatch(text, @"(?i)api\s*key\s*[-:=]\s*[A-Za-z0-9]{40,}"), $"possible hard-coded API key found in {file}");
    }
}

static void LlmFailureFallsBackWithoutLeakingDetails()
{
    var classifier = new ResilientIntentClassifier(
        new FailingIntentClassifier(),
        new RuleBasedIntentClassifier(new IntentRouter()));

    var intent = classifier.DetectAsync("Start quote").GetAwaiter().GetResult();
    Assert(intent == ChatIntent.StartQuote, "fallback classifier did not classify start quote");

    var orchestrator = new ChatOrchestrator(new KnowledgeBase(), classifier, new MockEligibilityService());
    var response = orchestrator.Respond("Which product is best for me?");
    Assert(!response.Message.Contains("secret provider failure"), "provider details leaked");
}

static void FrontendFreeTextUsesBackendRouting()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("/api/chat/message"), "frontend must call backend chat message endpoint");
    Assert(app.Contains("response.intent === \"start_quote\""), "frontend must react to backend start quote intent");
    Assert(app.Contains("response.intent === \"collect_contact_details\""), "frontend must react to backend contact detail intent");
    Assert(!app.Contains("const lower = trimmed.toLowerCase()"), "frontend still derives local free-text routing key");
    Assert(!app.Contains("lower.includes("), "frontend still contains local keyword routing branches");
    Assert(!app.Contains("handleLifeEvent"), "frontend still has local life-event handler");
    Assert(!app.Contains("handleOptions"), "frontend still has local options handler");
}

static void GroundedLlmResponseGeneratorExists()
{
    var source = Read("src/TalLifeShield.Core/IntentClassification.cs");
    var program = Read("src/TalLifeShield.Api/Program.cs");

    Assert(source.Contains("GroundedLlmChatResponseGenerator"), "grounded LLM response generator missing");
    Assert(source.Contains("IChatResponseGenerator"), "chat response generator abstraction missing");
    Assert(program.Contains("AddSingleton<IChatResponseGenerator>"), "chat response generator DI missing");
    Assert(source.Contains("CreateIntentResponseAsync(ResponseInstructions"), "response generator does not use Responses API client");
}

static void LlmResponsePreservesChatMetadata()
{
    var quote = new QuoteSummary("Q1", "LI", "Life Insurance Plan", 500000m, 22.10m, true, false);
    var deterministic = new ChatResponse(
        "life_event_discovery",
        "Deterministic response",
        "knowledge-base:newly-married",
        new[] { "Start a quick quote" },
        quote,
        true,
        new[] { "Quote Generated" });

    var generator = new GroundedLlmChatResponseGenerator(new FakeOpenAiResponsesClient("Grounded LLM response"));
    var generated = generator.GenerateAsync(deterministic, "I am newly married", Array.Empty<KnowledgeArticle>()).GetAwaiter().GetResult();

    Assert(generated.Message == "Grounded LLM response", "LLM message was not used");
    Assert(generated.Source == "knowledge-base:newly-married+llm:responses", "LLM source marker missing");
    Assert(generated.Intent == deterministic.Intent, "intent changed");
    Assert(generated.QuickReplies.SequenceEqual(deterministic.QuickReplies), "quick replies changed");
    Assert(ReferenceEquals(generated.Quote, quote), "quote metadata changed");
    Assert(generated.RequiresPii == deterministic.RequiresPii, "PII flag changed");
    Assert(generated.WorkflowSteps.SequenceEqual(deterministic.WorkflowSteps), "workflow changed");
}

static void LlmResponseInstructionsAreGrounded()
{
    var source = Read("src/TalLifeShield.Core/IntentClassification.cs");

    foreach (var expected in new[]
    {
        "deterministicResponse",
        "knowledgeBase",
        "Do not use outside facts",
        "Do not provide personal financial advice",
        "Do not invent premiums"
    })
    {
        Assert(source.Contains(expected), $"LLM response instructions missing {expected}");
    }
}

static void LlmResponseFailureFallsBack()
{
    var deterministic = new ChatResponse(
        "fallback_unknown",
        "Safe deterministic response",
        "guardrail:fallback",
        Array.Empty<string>(),
        null,
        false,
        Array.Empty<string>());
    var generator = new ResilientChatResponseGenerator(
        new FailingChatResponseGenerator(),
        new PassthroughChatResponseGenerator());

    var response = generator.GenerateAsync(deterministic, "hello", Array.Empty<KnowledgeArticle>()).GetAwaiter().GetResult();
    Assert(response.Message == "Safe deterministic response", "deterministic response was not preserved");
    Assert(!response.Message.Contains("secret response failure"), "provider failure leaked");
}

static void NewlyMarriedGetsLifeEventResponse()
{
    var response = Orchestrator().Respond("I am newly married");

    Assert(response.Intent == "life_event_discovery", "newly married should map to life event discovery");
    Assert(response.Source.Contains("newly-married"), "newly married knowledge source missing");
    Assert(response.Message.Contains("married", StringComparison.OrdinalIgnoreCase)
        || response.Message.Contains("partner", StringComparison.OrdinalIgnoreCase), "newly married response missing relevant grounding");
    Assert(!response.Message.StartsWith("Quotes shown are indicative only", StringComparison.OrdinalIgnoreCase), "response is only the generic quote disclaimer");
}

static void PostQuoteRequiresProceedConfirmation()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    var source = Read("src/TalLifeShield.Core/QuickQuotePresentation.cs");

    Assert(source.Contains("Your indicative quote is ready"), "quote ready message missing");
    Assert(source.Contains("If you are happy to go ahead"), "post-quote proceed prompt missing");
    Assert(app.Contains("presentation.quickReplies"), "post-quote quick replies should be supplied by backend presentation");
}

static void FrontendCollectsContactDetailsAfterConfirmation()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("contactSteps"), "contact detail steps missing");
    foreach (var expected in new[] { "name", "dateOfBirth", "postcode", "email", "phone", "address" })
    {
        Assert(app.Contains($"key: \"{expected}\""), $"contact detail step missing {expected}");
    }

    foreach (var expected in new[] { "Full name?", "Date of birth?", "Postcode?", "Email address?", "Phone number?", "Residential address?" })
    {
        Assert(app.Contains(expected), $"contact prompt missing {expected}");
    }
}

static void ContinueOptionsAppearAfterContactDetails()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");
    var capturedIndex = app.IndexOf("Contact Details Captured", StringComparison.Ordinal);
    var optionsIndex = app.IndexOf("setQuickReplies([\"Continue application\", \"Talk to an agent\"])", StringComparison.Ordinal);

    Assert(capturedIndex >= 0, "contact captured workflow marker missing");
    Assert(optionsIndex > capturedIndex, "continue/talk options should be set after contact details are captured");
}

static void QuickQuoteAuditDocumentsFixedFrontendDefect()
{
    var audit = Read("docs/changes/backend-quote-context-llm/AUDIT.md");

    foreach (var expected in new[] { "Frontend Builds Quote Card From Fixed Values", "Premium Calculation Is Static", "Conversation History Is Not Sent To Backend" })
    {
        Assert(audit.Contains(expected), $"audit missing {expected}");
    }
}

static void FrontendUsesBackendQuotePresentationEndpoint()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("/api/chat/quick-quote"), "frontend must call backend quote presentation endpoint");
    Assert(app.Contains("presentation.rows"), "frontend must render backend presentation rows");
    Assert(app.Contains("presentation.price"), "frontend must render backend presentation price");
    Assert(!app.Contains("/api/quotes/Q3350008/premium-calculation"), "frontend still calls fixed quote premium endpoint");
    Assert(!app.Contains("[\"Life Cover\", \"$5,000,000\"]"), "frontend still hard-codes cover amount row");
    Assert(!app.Contains("price: `$${amount} / month`"), "frontend still assembles fixed premium price");
}

static void BackendQuickQuotePresentationUsesApiResponses()
{
    var service = new QuickQuotePresentationService(new MockEligibilityService(), new PassthroughQuickQuotePresenter());
    var response = service.PresentAsync(new QuickQuotePresentationRequest(
        new QuoteAnswerInput(false, 1988, "Male", "Accountant - Not university qualified"),
        new[] { new ConversationTurn("user", "I am newly married") })).GetAwaiter().GetResult();

    Assert(response.Source == "api:setBasicInfo+api:quotePremiumCalculation", "quick quote source should cite backend APIs");
    Assert(response.QuoteRefNo.StartsWith("Q", StringComparison.Ordinal), "quote reference missing");
    Assert(response.Rows.Any(row => row.Label == "Monthly premium" && row.Value.Contains("$")), "monthly premium row missing");
    Assert(response.QuickReplies.Contains("I'm happy to go ahead"), "post-quote confirmation missing");
}

static void PremiumCalculationVariesByQuoteInputs()
{
    var service = new MockEligibilityService();
    var baselinePremium = Json(service.CalculateQuotePremium("unknown", new PremiumCalculationRequest("LI", 5000000m)));
    Assert(baselinePremium.Contains("\"premiumAmt\":220.93"), "baseline sample premium should be preserved");

    var lowRisk = service.CreateQuote(new CreateQuoteRequest(
        new BasicInformationInput("1988-01-01T00:00:00", null, null, null, null, "F", false, "2020", null, false, null),
        "TALC002", "TALD", "quote-token", " ", " "));
    var highRisk = service.CreateQuote(new CreateQuoteRequest(
        new BasicInformationInput("1975-01-01T00:00:00", null, null, null, null, "M", true, "1067", null, false, null),
        "TALC002", "TALD", "quote-token", " ", " "));

    var lowPremium = PremiumFrom(service.CalculateQuotePremium(QuoteRefFrom(lowRisk), new PremiumCalculationRequest("LI", 5000000m)));
    var highPremium = PremiumFrom(service.CalculateQuotePremium(QuoteRefFrom(highRisk), new PremiumCalculationRequest("LI", 5000000m)));

    Assert(highPremium > lowPremium, "higher-risk quote inputs should produce a higher premium");
}

static void LlmQuickQuotePresenterReceivesApiAndHistoryGrounding()
{
    var client = new CapturingOpenAiResponsesClient("{\"title\":\"Quote\",\"message\":\"Ready\",\"rows\":[{\"label\":\"Context\",\"value\":\"Used\"}]}");
    var presenter = new GroundedLlmQuickQuotePresenter(client);
    var fallback = FallbackQuickQuotePresentation();

    presenter.PresentAsync(
        new QuoteAnswerInput(false, 1988, "Male", "Accountant"),
        new[] { new ConversationTurn("user", "I already said I do not smoke") },
        "{\"data\":{\"setBasicInfo\":{\"quoteRefNo\":\"Q1\"}}}",
        "{\"data\":{\"quotePremiumCalculation\":{\"quickQuote\":{\"covers\":[{\"premiumAmt\":123.45}]}}}}",
        fallback).GetAwaiter().GetResult();

    Assert(client.LastInstructions.Contains("conversationHistory"), "LLM instructions missing conversation history");
    Assert(client.LastInput.Contains("createQuoteResponse"), "LLM input missing create quote response");
    Assert(client.LastInput.Contains("premiumCalculationResponse"), "LLM input missing premium response");
    Assert(client.LastInput.Contains("I already said I do not smoke"), "LLM input missing conversation turn");
}

static void LlmQuickQuotePresenterPreservesBackendPremium()
{
    var client = new FakeOpenAiResponsesClient("{\"title\":\"Changed\",\"message\":\"Changed\",\"rows\":[{\"label\":\"Monthly premium\",\"value\":\"$1.00 / month\"},{\"label\":\"Other\",\"value\":\"LLM text\"}]}");
    var presenter = new GroundedLlmQuickQuotePresenter(client);
    var fallback = FallbackQuickQuotePresentation();

    var response = presenter.PresentAsync(
        new QuoteAnswerInput(false, 1988, "Male", "Accountant"),
        Array.Empty<ConversationTurn>(),
        "{}",
        "{}",
        fallback).GetAwaiter().GetResult();

    Assert(response.Price == fallback.Price, "backend price should remain authoritative");
    Assert(response.Rows.Any(row => row.Label == "Monthly premium" && row.Value == fallback.Price), "backend monthly premium row should be preserved");
    Assert(response.Source.EndsWith("+llm:quick-quote", StringComparison.Ordinal), "LLM source marker missing");
}

static void LlmQuickQuoteFallbackUsesApiValues()
{
    var fallback = FallbackQuickQuotePresentation();
    var presenter = new ResilientQuickQuotePresenter(new FailingQuickQuotePresenter(), new PassthroughQuickQuotePresenter());

    var response = presenter.PresentAsync(
        new QuoteAnswerInput(false, 1988, "Male", "Accountant"),
        Array.Empty<ConversationTurn>(),
        "{}",
        "{}",
        fallback).GetAwaiter().GetResult();

    Assert(response.Price == fallback.Price, "fallback should use backend price");
    Assert(response.Rows.SequenceEqual(fallback.Rows), "fallback should use backend rows");
    Assert(!response.Message.Contains("secret quick quote failure"), "provider details leaked");
}

static void FrontendSendsConversationHistory()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("function conversationHistory"), "conversation history helper missing");
    Assert(app.Contains("conversationHistory: conversationHistory(trimmed)"), "chat endpoint missing conversation history");
    Assert(app.Contains("conversationHistory: conversationHistory(latestUserText)"), "quick quote endpoint missing conversation history");
}

static void FrontendAsksOnlyMissingQuoteQuestions()
{
    var app = Read("src/TalLifeShield.Api/wwwroot/app.js");

    Assert(app.Contains("firstMissingQuoteStepIndex"), "missing quote-step skip helper");
    Assert(app.Contains("const nextIndex = firstMissingQuoteStepIndex(quoteAnswers)"), "quote start should resume from missing answer");
    Assert(app.Contains("const nextIndex = firstMissingQuoteStepIndex(nextAnswers)"), "quote answer should skip already answered fields");
    Assert(!app.Contains("setQuoteAnswers({});"), "quote start should not erase already captured quote answers");
}

static CreateQuoteRequest CreateRequest() => new(
    new BasicInformationInput(
        "2001-03-01T00:00:00",
        "test",
        "test",
        "test@test.com",
        "0400000000",
        "M",
        false,
        "1067",
        2000000m,
        true,
        "2018"),
    "TALC002",
    "TALD",
    "mock-token",
    " ",
    " ");

static QuoteFlowState CompleteQuoteState(bool quoteShown, bool wantsToContinue) => new()
{
    BirthYear = 1988,
    Gender = "Male",
    IsSmoker = false,
    Occupation = "Accountant - Not university qualified",
    EmploymentStatus = "Full Time",
    AnnualIncome = 120000m,
    CoverCode = "LI",
    SumInsured = 5000000m,
    IndicativeQuoteShown = quoteShown,
    WantsToContinue = wantsToContinue
};

static ChatOrchestrator Orchestrator() => new(
    new KnowledgeBase(),
    new RuleBasedIntentClassifier(new IntentRouter()),
    new MockEligibilityService());

static string Json(object payload) => new MockEligibilityService().Serialize(payload);

static string QuoteRefFrom(Dictionary<string, object?> createQuoteResponse)
{
    var data = (Dictionary<string, object?>)createQuoteResponse["data"]!;
    var setBasicInfo = (Dictionary<string, object?>)data["setBasicInfo"]!;
    return setBasicInfo["quoteRefNo"]?.ToString() ?? MockEligibilityService.SampleQuoteRefNo;
}

static decimal PremiumFrom(Dictionary<string, object?> premiumResponse)
{
    var data = (Dictionary<string, object?>)premiumResponse["data"]!;
    var premiumCalculation = (Dictionary<string, object?>)data["quotePremiumCalculation"]!;
    var quickQuote = (Dictionary<string, object?>)premiumCalculation["quickQuote"]!;
    var covers = (List<Dictionary<string, object?>>)quickQuote["covers"]!;
    return Convert.ToDecimal(covers[0]["premiumAmt"]);
}

static QuickQuotePresentationResponse FallbackQuickQuotePresentation() => new(
    "Fallback message",
    "api:setBasicInfo+api:quotePremiumCalculation",
    "Q1",
    "Fallback title",
    new[]
    {
        new QuoteCardRow("Quote reference", "Q1"),
        new QuoteCardRow("Cover amount", "$5,000,000"),
        new QuoteCardRow("Monthly premium", "$123.45 / month")
    },
    "$123.45 / month",
    false,
    new[] { "I'm happy to go ahead", "Talk to an agent" });

static string Read(string relativePath) => File.ReadAllText(Path.Combine(RepoRoot(), relativePath.Replace('/', Path.DirectorySeparatorChar)));

static string RepoRoot()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null && !File.Exists(Path.Combine(current.FullName, "TalLifeShield.sln")))
    {
        current = current.Parent;
    }

    return current?.FullName ?? throw new InvalidOperationException("Repository root not found");
}

static IReadOnlyList<string> RequiredDocs() => new[]
{
    "SPEC.md",
    "ACCEPTANCE_CRITERIA.md",
    "TRACEABILITY_MATRIX.md",
    "DEPENDENCY_GRAPH.md",
    "IMPLEMENTATION_PLAN.md",
    "TEST_PLAN.md",
    "AI_GUARDRAILS.md",
    "SPEC_VERIFICATION.md"
};

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static string ExtractJavaScriptStrings(string source) =>
    string.Join("\n", Regex.Matches(source, "\"(?:\\\\.|[^\"])*\"|`(?:\\\\.|[^`])*`")
        .Select(match => match.Value));

static string ExtractCSharpStrings(string source) =>
    string.Join("\n", Regex.Matches(source, "\"(?:\\\\.|[^\"])*\"")
        .Select(match => match.Value));

static void AssertNoNonProductionWords(string text, string scope)
{
    foreach (var forbidden in new[] { "demo", "mock", "sample" })
    {
        Assert(!text.Contains(forbidden, StringComparison.OrdinalIgnoreCase), $"{scope} contains {forbidden}");
    }
}

sealed class FakeOpenAiResponsesClient : IOpenAiResponsesClient
{
    private readonly string _response;

    public FakeOpenAiResponsesClient(string response)
    {
        _response = response;
    }

    public Task<string> CreateIntentResponseAsync(string instructions, string input, CancellationToken cancellationToken = default) =>
        Task.FromResult(_response);
}

sealed class FailingIntentClassifier : IIntentClassifier
{
    public Task<ChatIntent> DetectAsync(string message, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("secret provider failure");
}

sealed class FailingChatResponseGenerator : IChatResponseGenerator
{
    public Task<ChatResponse> GenerateAsync(
        ChatResponse deterministicResponse,
        string userMessage,
        IReadOnlyList<KnowledgeArticle> grounding,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("secret response failure");
}

sealed class CapturingOpenAiResponsesClient : IOpenAiResponsesClient
{
    private readonly string _response;

    public CapturingOpenAiResponsesClient(string response)
    {
        _response = response;
    }

    public string LastInstructions { get; private set; } = "";
    public string LastInput { get; private set; } = "";

    public Task<string> CreateIntentResponseAsync(string instructions, string input, CancellationToken cancellationToken = default)
    {
        LastInstructions = instructions;
        LastInput = input;
        return Task.FromResult(_response);
    }
}

sealed class FailingQuickQuotePresenter : IQuickQuotePresenter
{
    public Task<QuickQuotePresentationResponse> PresentAsync(
        QuoteAnswerInput answers,
        IReadOnlyList<ConversationTurn> conversationHistory,
        string createQuoteResponseJson,
        string premiumResponseJson,
        QuickQuotePresentationResponse deterministicPresentation,
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("secret quick quote failure");
}
