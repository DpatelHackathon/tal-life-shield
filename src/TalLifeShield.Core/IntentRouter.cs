namespace TalLifeShield.Core;

public enum ChatIntent
{
    AskAboutCover,
    LifeEventDiscovery,
    CompareCovers,
    StartQuote,
    ProvideBasicQuoteDetails,
    SearchOccupation,
    SelectCover,
    CalculatePremium,
    UpdateQuote,
    CollectContactDetails,
    UnderwritingHandoff,
    TalkToAgent,
    FallbackUnknown
}

public sealed class IntentRouter
{
    public ChatIntent Detect(string message)
    {
        var text = message.ToLowerInvariant();

        if (ContainsAny(text, "email", "phone", "postcode", "mobile", "contact details", "continue my application", "go ahead", "proceed", "happy to", "confirm"))
            return ChatIntent.CollectContactDetails;
        if (ContainsAny(text, "talk to an agent", "speak to an agent", "call tal", "contact support", "talk to a person"))
            return ChatIntent.TalkToAgent;
        if (ContainsAny(text, "height", "weight", "discount", "bmi", "recalculate", "final quote"))
            return ChatIntent.UpdateQuote;
        if (ContainsAny(text, "underwriting", "application", "handoff", "apply for cover"))
            return ChatIntent.UnderwritingHandoff;
        if (ContainsAny(text, "premium", "calculate", "price", "per month", "monthly cost"))
            return ChatIntent.CalculatePremium;
        if (ContainsAny(text, "select", "include", "life only", "life insurance only", "life + income", "tpd"))
            return ChatIntent.SelectCover;
        if (ContainsAny(text, "occupation", "accountant", "accounts clerk", "job title"))
            return ChatIntent.SearchOccupation;
        if (ContainsAny(text, "born", "birth", "smoker", "income", "salary", "male", "female", "self employed", "full time"))
            return ChatIntent.ProvideBasicQuoteDetails;
        if (ContainsAny(text, "start quote", "quick quote", "get a quote", "quote me"))
            return ChatIntent.StartQuote;
        if (ContainsAny(text, "compare", "difference", "which cover", "options"))
            return ChatIntent.CompareCovers;
        if (ContainsAny(text, "home", "mortgage", "family", "promotion", "new job", "reviewing existing", "married", "marriage", "partner", "spouse"))
            return ChatIntent.LifeEventDiscovery;
        if (ContainsAny(text, "life insurance", "income protection", "critical illness", "total and permanent", "cover", "insurance"))
            return ChatIntent.AskAboutCover;

        return ChatIntent.FallbackUnknown;
    }

    public static string ToWireName(ChatIntent intent) => intent switch
    {
        ChatIntent.AskAboutCover => "ask_about_cover",
        ChatIntent.LifeEventDiscovery => "life_event_discovery",
        ChatIntent.CompareCovers => "compare_covers",
        ChatIntent.StartQuote => "start_quote",
        ChatIntent.ProvideBasicQuoteDetails => "provide_basic_quote_details",
        ChatIntent.SearchOccupation => "search_occupation",
        ChatIntent.SelectCover => "select_cover",
        ChatIntent.CalculatePremium => "calculate_premium",
        ChatIntent.UpdateQuote => "update_quote",
        ChatIntent.CollectContactDetails => "collect_contact_details",
        ChatIntent.UnderwritingHandoff => "underwriting_handoff",
        ChatIntent.TalkToAgent => "talk_to_agent",
        _ => "fallback_unknown"
    };

    private static bool ContainsAny(string text, params string[] tokens) =>
        tokens.Any(token => text.Contains(token, StringComparison.OrdinalIgnoreCase));
}
