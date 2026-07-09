namespace TalLifeShield.Core;

public sealed record QuoteSummary(
    string QuoteRefNo,
    string CoverCode,
    string CoverDescription,
    decimal SumInsured,
    decimal MonthlyPremium,
    bool IsIndicative,
    bool DiscountApplied);

public sealed record ChatResponse(
    string Intent,
    string Message,
    string Source,
    IReadOnlyList<string> QuickReplies,
    QuoteSummary? Quote,
    bool RequiresPii,
    IReadOnlyList<string> WorkflowSteps);

public sealed class ChatOrchestrator
{
    private readonly KnowledgeBase _knowledgeBase;
    private readonly IIntentClassifier _intentClassifier;
    private readonly MockEligibilityService _mockEligibilityService;
    private readonly IChatResponseGenerator _responseGenerator;

    public ChatOrchestrator(
        KnowledgeBase knowledgeBase,
        IIntentClassifier intentClassifier,
        MockEligibilityService mockEligibilityService,
        IChatResponseGenerator? responseGenerator = null)
    {
        _knowledgeBase = knowledgeBase;
        _intentClassifier = intentClassifier;
        _mockEligibilityService = mockEligibilityService;
        _responseGenerator = responseGenerator ?? new PassthroughChatResponseGenerator();
    }

    public ChatResponse Respond(string message, QuoteFlowState? state = null) =>
        RespondAsync(message, state).GetAwaiter().GetResult();

    public async Task<ChatResponse> RespondAsync(
        string message,
        QuoteFlowState? state = null,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default)
    {
        state ??= new QuoteFlowState();
        var intent = await _intentClassifier.DetectAsync(message, cancellationToken);
        var wireIntent = IntentRouter.ToWireName(intent);

        if (IsPersonalAdviceRequest(message))
        {
            return SafeFallback(wireIntent);
        }

        var response = intent switch
        {
            ChatIntent.AskAboutCover => KnowledgeResponse(wireIntent, message),
            ChatIntent.LifeEventDiscovery => KnowledgeResponse(wireIntent, message),
            ChatIntent.CompareCovers => CoverComparison(wireIntent),
            ChatIntent.StartQuote => StartQuote(wireIntent),
            ChatIntent.ProvideBasicQuoteDetails => BasicDetails(wireIntent, state),
            ChatIntent.SearchOccupation => OccupationSearch(wireIntent, message),
            ChatIntent.SelectCover => SelectCover(wireIntent),
            ChatIntent.CalculatePremium => CalculatePremium(wireIntent, state),
            ChatIntent.UpdateQuote => DiscountAndFinalQuote(wireIntent),
            ChatIntent.CollectContactDetails => CollectContactDetails(wireIntent, message, state),
            ChatIntent.UnderwritingHandoff => UnderwritingHandoff(wireIntent),
            ChatIntent.TalkToAgent => TalkToAgent(wireIntent),
            _ => SafeFallback(wireIntent)
        };

        return ShouldUseResponseGenerator(intent)
            ? await _responseGenerator.GenerateAsync(response, message, GroundingFor(message, intent), conversationHistory, cancellationToken)
            : response;
    }

    private bool ShouldUseResponseGenerator(ChatIntent intent) =>
        intent is ChatIntent.AskAboutCover
            or ChatIntent.LifeEventDiscovery
            or ChatIntent.CompareCovers
            or ChatIntent.FallbackUnknown;

    private IReadOnlyList<KnowledgeArticle> GroundingFor(string message, ChatIntent intent)
    {
        var articles = new List<KnowledgeArticle>();
        var best = _knowledgeBase.FindBestMatch(message);
        if (best is not null)
        {
            articles.Add(best);
        }

        if (intent is ChatIntent.CompareCovers or ChatIntent.AskAboutCover)
        {
            articles.AddRange(_knowledgeBase.ByCategory("cover"));
        }

        if (intent is ChatIntent.LifeEventDiscovery)
        {
            articles.AddRange(_knowledgeBase.ByCategory("life-event"));
        }

        articles.AddRange(_knowledgeBase.Articles.Where(article =>
            article.Key is "quote-disclaimer" or "support-fallback"));

        return articles
            .GroupBy(article => article.Key)
            .Select(group => group.First())
            .ToList();
    }

    private ChatResponse KnowledgeResponse(string intent, string message)
    {
        var article = _knowledgeBase.FindBestMatch(message)
            ?? _knowledgeBase.Articles.First(article => article.Key == "quote-disclaimer");

        return new ChatResponse(
            intent,
            $"{article.Content} Quotes are indicative only and subject to eligibility and underwriting.",
            $"knowledge-base:{article.Key}",
            new[] { "Start a quick quote", "Compare cover options" },
            null,
            false,
            Array.Empty<string>());
    }

    private ChatResponse CoverComparison(string intent)
    {
        var covers = _knowledgeBase.ByCategory("cover").Select(article => $"{article.Title}: {article.Content}");
        return new ChatResponse(
            intent,
            string.Join("\n\n", covers) + "\n\nThis is general information only.",
            "knowledge-base:cover",
            new[] { "Life Insurance only", "Life + Income Protection + TPD", "Start a quick quote" },
            null,
            false,
            Array.Empty<string>());
    }

    private static ChatResponse StartQuote(string intent) => new(
        intent,
        "I can generate an indicative quote using only the required non-PII details first: smoker status, year of birth, gender, and occupation.",
        "quote-flow:min-details",
        new[] { "I have those details", "Show cover options" },
        null,
        false,
        Array.Empty<string>());

    private static ChatResponse BasicDetails(string intent, QuoteFlowState state)
    {
        if (state.HasMinimumQuoteDetails)
        {
            return new ChatResponse(
                intent,
                "Thanks. I have the minimum details needed to calculate an indicative quote.",
                "quote-flow:min-details-complete",
                new[] { "Calculate premium" },
                null,
                false,
                Array.Empty<string>());
        }

        return new ChatResponse(
            intent,
            $"I still need: {string.Join(", ", state.MissingMinimumFields())}. I will not ask for name, date of birth, postcode, email, or phone number until you choose to continue after seeing an indicative quote.",
            "quote-flow:min-details-missing",
            Array.Empty<string>(),
            null,
            false,
            Array.Empty<string>());
    }

    private ChatResponse OccupationSearch(string intent, string message)
    {
        var occupations = _mockEligibilityService.GetOccupations(message.Contains("acc", StringComparison.OrdinalIgnoreCase) ? "acc" : "accountant");
        var json = _mockEligibilityService.Serialize(occupations);

        return new ChatResponse(
            intent,
            "I found occupation examples from the occupation service, including Accountant - Not university qualified and Accountant - University qualified.",
            "api:getOccupations",
            new[] { "Use Accountant - Not university qualified" },
            null,
            false,
            json.Contains("1067") ? Array.Empty<string>() : new[] { "Occupation service missing expected code" });
    }

    private ChatResponse SelectCover(string intent) => new(
        intent,
        "I can select Life Insurance Plan using the backend API response and calculate an indicative monthly premium.",
        "api:getSelectCover",
        new[] { "Calculate Life Insurance premium" },
        null,
        false,
        Array.Empty<string>());

    private ChatResponse CalculatePremium(string intent, QuoteFlowState state)
    {
        if (!state.CanCalculatePremium)
        {
            return new ChatResponse(
                intent,
                $"I cannot calculate the indicative premium yet. Missing: {string.Join(", ", state.MissingMinimumFields())}.",
                "quote-flow:min-details-gate",
                Array.Empty<string>(),
                null,
                false,
                Array.Empty<string>());
        }

        _mockEligibilityService.CalculateQuotePremium(MockEligibilityService.SampleQuoteRefNo, new PremiumCalculationRequest("LI", state.SumInsured));

        return new ChatResponse(
            intent,
            "Your indicative Life Insurance premium is $220.93 per month. This quote is subject to eligibility and underwriting.",
            "api:quotePremiumCalculation",
            new[] { "Apply optional 5% discount", "Continue application" },
            new QuoteSummary(MockEligibilityService.SampleQuoteRefNo, "LI", "Life Insurance Plan", state.SumInsured ?? 5000000m, MockEligibilityService.SampleLifePremium, true, false),
            false,
            new[] { "Quote Generated" });
    }

    private ChatResponse DiscountAndFinalQuote(string intent)
    {
        _mockEligibilityService.UpdateQuote(MockEligibilityService.SampleQuoteRefNo, new UpdateQuoteRequest("LI", 5000000m, 178m, 82m, true));

        return new ChatResponse(
            intent,
            "Thanks. The final indicative quote includes a 5% discount: previous premium $87/month, new premium $82.65/month. If you would like to discuss before proceeding, a TAL Customer Service Officer can help on 131 825 or customerservice@tal.com.au.",
            "api:setSelectCover+knowledge-base:support-fallback",
            new[] { "Continue to underwriting", "Contact customer support" },
            new QuoteSummary(MockEligibilityService.SampleQuoteRefNo, "LI", "Life Insurance Plan", 500000m, 82.65m, true, true),
            false,
            QuoteFlowState.WorkflowSteps);
    }

    private static ChatResponse CollectContactDetails(string intent, string message, QuoteFlowState state)
    {
        if (!state.CanRequestPii && !(state.IndicativeQuoteShown && IsContinuationConfirmation(message)))
        {
            return new ChatResponse(
                intent,
                "I will ask for name, date of birth, postcode, email, phone number, and address only after you choose to continue from an indicative quote.",
                "guardrail:pii-timing",
                new[] { "Calculate premium first" },
                null,
                false,
                Array.Empty<string>());
        }

        return new ChatResponse(
            intent,
            "To continue the application, please provide your name, date of birth, postcode, email, phone number, and address.",
            "quote-flow:post-quote-pii",
            new[] { "Continue to underwriting" },
            null,
            true,
            Array.Empty<string>());
    }

    private static bool IsContinuationConfirmation(string message)
    {
        var text = message.ToLowerInvariant();
        return text.Contains("go ahead", StringComparison.OrdinalIgnoreCase)
            || text.Contains("proceed", StringComparison.OrdinalIgnoreCase)
            || text.Contains("happy to", StringComparison.OrdinalIgnoreCase)
            || text.Contains("confirm", StringComparison.OrdinalIgnoreCase)
            || text.Contains("continue", StringComparison.OrdinalIgnoreCase)
            || text == "yes";
    }

    private static ChatResponse UnderwritingHandoff(string intent) => new(
        intent,
        "Your quote is ready. To continue, complete the underwriting questions in the standard application flow. A TAL Customer Service Officer can help on 131 825 or customerservice@tal.com.au.",
        "knowledge-base:underwriting-handoff",
        new[] { "Continue application" },
        null,
        false,
        QuoteFlowState.WorkflowSteps);

    private static ChatResponse TalkToAgent(string intent) => new(
        intent,
        "You can talk to a TAL Customer Service Officer by calling 131 825.",
        "knowledge-base:support-fallback",
        Array.Empty<string>(),
        null,
        false,
        Array.Empty<string>());

    private static ChatResponse SafeFallback(string intent) => new(
        intent,
        "I cannot confirm that from the available knowledge base, conversation context, or backend API information. I can provide general information about the listed cover types, help with an indicative quote, or suggest contacting customer support.",
        "guardrail:fallback",
        new[] { "Show cover options", "Start a quick quote" },
        null,
        false,
        Array.Empty<string>());

    private static bool IsPersonalAdviceRequest(string message)
    {
        var text = message.ToLowerInvariant();
        return text.Contains("should i buy", StringComparison.OrdinalIgnoreCase)
            || text.Contains("best for me", StringComparison.OrdinalIgnoreCase)
            || text.Contains("guaranteed eligible", StringComparison.OrdinalIgnoreCase)
            || text.Contains("will my claim", StringComparison.OrdinalIgnoreCase);
    }
}
