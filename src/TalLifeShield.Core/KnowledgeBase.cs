namespace TalLifeShield.Core;

public sealed record KnowledgeArticle(
    string Key,
    string Title,
    string Category,
    string Content,
    IReadOnlyList<string> Tags);

public sealed class KnowledgeBase
{
    private readonly IReadOnlyList<KnowledgeArticle> _articles = new List<KnowledgeArticle>
    {
        new(
            "protect-what-matters",
            "Protect what matters most",
            "positioning",
            "TAL Life Shield helps customers explore life insurance options in minutes with a conversational quote assistant. This is general information only.",
            new[] { "protect", "matters", "intro" }),
        new(
            "life-insurance",
            "Life Insurance",
            "cover",
            "Life Insurance can provide a lump sum payment to your family if you pass away. Any quote shown is indicative and subject to eligibility and underwriting.",
            new[] { "li", "life", "lump sum", "family" }),
        new(
            "income-protection",
            "Income Protection",
            "cover",
            "Income Protection can provide monthly payments if illness or injury prevents you from working. Benefit and waiting periods depend on available product options.",
            new[] { "ip", "income", "monthly", "work" }),
        new(
            "critical-illness",
            "Critical Illness",
            "cover",
            "Critical Illness cover can provide support if you suffer a serious illness. The assistant does not make underwriting or claims decisions.",
            new[] { "ci", "critical", "illness", "serious" }),
        new(
            "tpd",
            "Total and Permanent Disability",
            "cover",
            "TPD cover can provide support if you are totally and permanently disabled and unable to work. This is general information only.",
            new[] { "tpd", "disability", "unable to work" }),
        new(
            "recently-bought-home",
            "Recently bought a home",
            "life-event",
            "People who have recently bought a home often explore cover that may help protect their mortgage, income, and family.",
            new[] { "home", "mortgage", "recently bought" }),
        new(
            "starting-family",
            "Starting a family",
            "life-event",
            "People starting a family often explore cover that may help provide financial support for loved ones if illness, injury, disability, or death affects household income.",
            new[] { "family", "children", "starting" }),
        new(
            "newly-married",
            "Newly married",
            "life-event",
            "People who are newly married or building a life with a partner often explore cover that may help protect shared financial responsibilities, income, and loved ones.",
            new[] { "married", "marriage", "newly married", "partner", "spouse" }),
        new(
            "new-job",
            "New job or promotion",
            "life-event",
            "A new job or promotion can be a good time to review income, debts, and cover needs. This assistant can provide general information and an indicative quote.",
            new[] { "job", "promotion", "income" }),
        new(
            "review-cover",
            "Reviewing existing cover",
            "life-event",
            "Reviewing existing cover can help customers compare general cover types and decide whether to continue with a new indicative quote.",
            new[] { "review", "existing", "cover" }),
        new(
            "quote-disclaimer",
            "Quote disclaimer",
            "disclaimer",
            "Quotes shown are indicative only and are subject to eligibility and underwriting.",
            new[] { "quote", "indicative", "underwriting" }),
        new(
            "underwriting-handoff",
            "Underwriting handoff",
            "handoff",
            "To continue after an indicative quote, the customer can complete underwriting questions through the standard application flow.",
            new[] { "underwriting", "continue", "application" }),
        new(
            "support-fallback",
            "Customer support fallback",
            "handoff",
            "If the customer would like to discuss before proceeding, a TAL Customer Service Officer can help on 131 825 or customerservice@tal.com.au.",
            new[] { "support", "cso", "phone", "email" })
    };

    public IReadOnlyList<KnowledgeArticle> Articles => _articles;

    public KnowledgeArticle? FindBestMatch(string text)
    {
        var normalized = text.ToLowerInvariant();
        return _articles
            .Select(article => new
            {
                Article = article,
                Score = article.Tags.Count(tag => normalized.Contains(tag, StringComparison.OrdinalIgnoreCase))
                    + (normalized.Contains(article.Title.ToLowerInvariant()) ? 2 : 0)
            })
            .OrderByDescending(match => match.Score)
            .FirstOrDefault(match => match.Score > 0)
            ?.Article;
    }

    public IEnumerable<KnowledgeArticle> ByCategory(string category) =>
        _articles.Where(article => article.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
}
