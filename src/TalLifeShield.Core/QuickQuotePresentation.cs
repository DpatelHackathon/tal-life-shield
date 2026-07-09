using System.Text.Json;

namespace TalLifeShield.Core;

public sealed record ConversationTurn(string Who, string Text);

public sealed record QuoteAnswerInput(
    bool? IsSmoker,
    int? BirthYear,
    string? Gender,
    string? Occupation);

public sealed record QuickQuotePresentationRequest(
    QuoteAnswerInput? Answers,
    IReadOnlyList<ConversationTurn>? ConversationHistory);

public sealed record QuoteCardRow(string Label, string Value);

public sealed record QuickQuotePresentationResponse(
    string Message,
    string Source,
    string QuoteRefNo,
    string Title,
    IReadOnlyList<QuoteCardRow> Rows,
    string Price,
    bool Discounted,
    IReadOnlyList<string> QuickReplies);

public interface IQuickQuotePresenter
{
    Task<QuickQuotePresentationResponse> PresentAsync(
        QuoteAnswerInput answers,
        IReadOnlyList<ConversationTurn> conversationHistory,
        string createQuoteResponseJson,
        string premiumResponseJson,
        QuickQuotePresentationResponse deterministicPresentation,
        CancellationToken cancellationToken = default);
}

public sealed class PassthroughQuickQuotePresenter : IQuickQuotePresenter
{
    public Task<QuickQuotePresentationResponse> PresentAsync(
        QuoteAnswerInput answers,
        IReadOnlyList<ConversationTurn> conversationHistory,
        string createQuoteResponseJson,
        string premiumResponseJson,
        QuickQuotePresentationResponse deterministicPresentation,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(deterministicPresentation);
}

public sealed class ResilientQuickQuotePresenter : IQuickQuotePresenter
{
    private readonly IQuickQuotePresenter? _primary;
    private readonly IQuickQuotePresenter _fallback;

    public ResilientQuickQuotePresenter(IQuickQuotePresenter? primary, IQuickQuotePresenter fallback)
    {
        _primary = primary;
        _fallback = fallback;
    }

    public async Task<QuickQuotePresentationResponse> PresentAsync(
        QuoteAnswerInput answers,
        IReadOnlyList<ConversationTurn> conversationHistory,
        string createQuoteResponseJson,
        string premiumResponseJson,
        QuickQuotePresentationResponse deterministicPresentation,
        CancellationToken cancellationToken = default)
    {
        if (_primary is not null)
        {
            try
            {
                var generated = await _primary.PresentAsync(
                    answers,
                    conversationHistory,
                    createQuoteResponseJson,
                    premiumResponseJson,
                    deterministicPresentation,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(generated.Message) && generated.Rows.Count > 0)
                {
                    return generated;
                }
            }
            catch
            {
                // Keep provider details out of customer-facing responses.
            }
        }

        return await _fallback.PresentAsync(
            answers,
            conversationHistory,
            createQuoteResponseJson,
            premiumResponseJson,
            deterministicPresentation,
            cancellationToken);
    }
}

public sealed class GroundedLlmQuickQuotePresenter : IQuickQuotePresenter
{
    private readonly IOpenAiResponsesClient _client;

    public GroundedLlmQuickQuotePresenter(IOpenAiResponsesClient client)
    {
        _client = client;
    }

    public async Task<QuickQuotePresentationResponse> PresentAsync(
        QuoteAnswerInput answers,
        IReadOnlyList<ConversationTurn> conversationHistory,
        string createQuoteResponseJson,
        string premiumResponseJson,
        QuickQuotePresentationResponse deterministicPresentation,
        CancellationToken cancellationToken = default)
    {
        var input = JsonSerializer.Serialize(new
        {
            quoteAnswers = answers,
            conversationHistory,
            createQuoteResponse = createQuoteResponseJson,
            premiumCalculationResponse = premiumResponseJson,
            deterministicPresentation
        });

        var output = await _client.CreateIntentResponseAsync(Instructions, input, cancellationToken);
        var parsed = Parse(output, deterministicPresentation);

        return parsed with
        {
            QuoteRefNo = deterministicPresentation.QuoteRefNo,
            Price = deterministicPresentation.Price,
            Discounted = deterministicPresentation.Discounted,
            QuickReplies = deterministicPresentation.QuickReplies,
            Source = $"{deterministicPresentation.Source}+llm:quick-quote"
        };
    }

    private static QuickQuotePresentationResponse Parse(string output, QuickQuotePresentationResponse fallback)
    {
        using var document = JsonDocument.Parse(output);
        var root = document.RootElement;

        var title = root.TryGetProperty("title", out var titleElement)
            ? titleElement.GetString() ?? fallback.Title
            : fallback.Title;
        var message = root.TryGetProperty("message", out var messageElement)
            ? messageElement.GetString() ?? fallback.Message
            : fallback.Message;

        var rows = new List<QuoteCardRow>();
        if (root.TryGetProperty("rows", out var rowsElement) && rowsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var row in rowsElement.EnumerateArray())
            {
                var label = row.TryGetProperty("label", out var labelElement) ? labelElement.GetString() : null;
                var value = row.TryGetProperty("value", out var valueElement) ? valueElement.GetString() : null;
                if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(value))
                {
                    rows.Add(new QuoteCardRow(label, value));
                }
            }
        }

        if (rows.Count == 0)
        {
            rows.AddRange(fallback.Rows);
        }

        rows = PreserveBackendRows(rows, fallback).ToList();

        return fallback with
        {
            Title = title,
            Message = message,
            Rows = rows
        };
    }

    private static IEnumerable<QuoteCardRow> PreserveBackendRows(
        IReadOnlyList<QuoteCardRow> generatedRows,
        QuickQuotePresentationResponse fallback)
    {
        var protectedLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Quote reference",
            "Monthly premium",
            "Cover amount"
        };
        var protectedRows = fallback.Rows.Where(row => protectedLabels.Contains(row.Label)).ToList();
        var generatedWithoutProtected = generatedRows.Where(row => !protectedLabels.Contains(row.Label));

        return protectedRows.Concat(generatedWithoutProtected);
    }

    private const string Instructions = """
You are the TAL Life Shield AI Quote Assistant.
Read only the supplied JSON grounding:
- quoteAnswers
- conversationHistory
- createQuoteResponse
- premiumCalculationResponse
- deterministicPresentation
Convert the backend API responses into a concise quick quote display.
Return JSON only in this shape:
{"title":"...","message":"...","rows":[{"label":"...","value":"..."}]}
Do not use outside facts.
Do not invent or change premiums, quote references, sum insured values, eligibility, discounts, underwriting decisions, or product rules.
Use the conversation history only to avoid repeating details the customer has already provided.
Keep the response general information only and production-ready.
""";
}

public sealed class QuickQuotePresentationService
{
    private readonly MockEligibilityService _eligibilityService;
    private readonly IQuickQuotePresenter _presenter;

    public QuickQuotePresentationService(
        MockEligibilityService eligibilityService,
        IQuickQuotePresenter presenter)
    {
        _eligibilityService = eligibilityService;
        _presenter = presenter;
    }

    public async Task<QuickQuotePresentationResponse> PresentAsync(
        QuickQuotePresentationRequest request,
        CancellationToken cancellationToken = default)
    {
        var answers = request.Answers ?? new QuoteAnswerInput(false, 1988, "Male", "Accountant - Not university qualified");
        var history = request.ConversationHistory ?? Array.Empty<ConversationTurn>();
        var createQuoteRequest = CreateRequest(answers);
        var createQuoteResponse = _eligibilityService.CreateQuote(createQuoteRequest);
        var quoteRefNo = ExtractQuoteRefNo(createQuoteResponse);
        var premiumResponse = _eligibilityService.CalculateQuotePremium(quoteRefNo, new PremiumCalculationRequest("LI", 5000000m));
        var deterministic = BuildPresentation(answers, createQuoteResponse, premiumResponse);
        var createJson = _eligibilityService.Serialize(createQuoteResponse);
        var premiumJson = _eligibilityService.Serialize(premiumResponse);

        return await _presenter.PresentAsync(
            answers,
            history,
            createJson,
            premiumJson,
            deterministic,
            cancellationToken);
    }

    private static CreateQuoteRequest CreateRequest(QuoteAnswerInput answers) => new(
        new BasicInformationInput(
            $"{answers.BirthYear ?? 1988}-01-01T00:00:00",
            null,
            null,
            null,
            null,
            IsFemale(answers.Gender) ? "F" : "M",
            answers.IsSmoker ?? false,
            OccupationCode(answers.Occupation),
            null,
            false,
            null),
        "TALC002",
        "TALD",
        "quote-token",
        " ",
        " ");

    private static QuickQuotePresentationResponse BuildPresentation(
        QuoteAnswerInput answers,
        Dictionary<string, object?> createQuoteResponse,
        Dictionary<string, object?> premiumResponse)
    {
        var quoteRefNo = ExtractQuoteRefNo(createQuoteResponse);
        var premium = ExtractPremium(premiumResponse);

        return new QuickQuotePresentationResponse(
            "Your indicative quote is ready. This quote is based on the backend quote response and is subject to eligibility and underwriting. If you are happy to go ahead, I can collect your contact details next.",
            "api:setBasicInfo+api:quotePremiumCalculation",
            quoteRefNo,
            "Your Indicative Monthly Premium",
            new[]
            {
                new QuoteCardRow("Quote reference", quoteRefNo),
                new QuoteCardRow("Cover amount", "$5,000,000"),
                new QuoteCardRow("Monthly premium", $"${premium:0.00} / month"),
                new QuoteCardRow("Occupation", string.IsNullOrWhiteSpace(answers.Occupation) ? "Not provided" : answers.Occupation!),
                new QuoteCardRow("Smoker status", answers.IsSmoker == true ? "Smoker" : "Non-smoker"),
                new QuoteCardRow("Year of birth", (answers.BirthYear ?? 1988).ToString()),
                new QuoteCardRow("Payment frequency", "Monthly")
            },
            $"${premium:0.00} / month",
            false,
            new[] { "I'm happy to go ahead", "Talk to an agent" });
    }

    private static string ExtractQuoteRefNo(Dictionary<string, object?> createQuoteResponse)
    {
        var data = (Dictionary<string, object?>)createQuoteResponse["data"]!;
        var setBasicInfo = (Dictionary<string, object?>)data["setBasicInfo"]!;
        return setBasicInfo["quoteRefNo"]?.ToString() ?? MockEligibilityService.SampleQuoteRefNo;
    }

    private static decimal ExtractPremium(Dictionary<string, object?> premiumResponse)
    {
        var data = (Dictionary<string, object?>)premiumResponse["data"]!;
        var premiumCalculation = (Dictionary<string, object?>)data["quotePremiumCalculation"]!;
        var quickQuote = (Dictionary<string, object?>)premiumCalculation["quickQuote"]!;
        var covers = (List<Dictionary<string, object?>>)quickQuote["covers"]!;
        return Convert.ToDecimal(covers[0]["premiumAmt"]);
    }

    private static bool IsFemale(string? gender) =>
        !string.IsNullOrWhiteSpace(gender) && gender.Trim().StartsWith("f", StringComparison.OrdinalIgnoreCase);

    private static string OccupationCode(string? occupation) =>
        occupation?.Contains("university", StringComparison.OrdinalIgnoreCase) == true ? "2020" : "1067";
}
