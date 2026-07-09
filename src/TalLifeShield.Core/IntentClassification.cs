using System.Net.Http.Json;
using System.Text.Json;

namespace TalLifeShield.Core;

public interface IIntentClassifier
{
    Task<ChatIntent> DetectAsync(string message, CancellationToken cancellationToken = default);
}

public sealed class RuleBasedIntentClassifier : IIntentClassifier
{
    private readonly IntentRouter _intentRouter;

    public RuleBasedIntentClassifier(IntentRouter intentRouter)
    {
        _intentRouter = intentRouter;
    }

    public Task<ChatIntent> DetectAsync(string message, CancellationToken cancellationToken = default) =>
        Task.FromResult(_intentRouter.Detect(message));
}

public sealed class ResilientIntentClassifier : IIntentClassifier
{
    private readonly IIntentClassifier? _primary;
    private readonly IIntentClassifier _fallback;

    public ResilientIntentClassifier(IIntentClassifier? primary, IIntentClassifier fallback)
    {
        _primary = primary;
        _fallback = fallback;
    }

    public async Task<ChatIntent> DetectAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_primary is not null)
        {
            try
            {
                return await _primary.DetectAsync(message, cancellationToken);
            }
            catch
            {
                // Keep provider details out of customer-facing responses.
            }
        }

        return await _fallback.DetectAsync(message, cancellationToken);
    }
}

public sealed class OpenAiIntentOptions
{
    public string? Endpoint { get; init; }
    public string? ApiKey { get; init; }
    public string? Model { get; init; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(Model);
}

public interface IOpenAiResponsesClient
{
    Task<string> CreateIntentResponseAsync(string instructions, string input, CancellationToken cancellationToken = default);
}

public sealed class OpenAiResponsesClient : IOpenAiResponsesClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiIntentOptions _options;

    public OpenAiResponsesClient(HttpClient httpClient, OpenAiIntentOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> CreateIntentResponseAsync(string instructions, string input, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            throw new InvalidOperationException("LLM intent classification is not configured.");
        }

        var endpoint = _options.Endpoint!.TrimEnd('/');
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/responses");
        request.Headers.TryAddWithoutValidation("api-key", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            model = _options.Model,
            instructions,
            input,
            temperature = 0
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        return ExtractOutputText(document.RootElement);
    }

    private static string ExtractOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputText))
        {
            return outputText.GetString() ?? "";
        }

        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var contentItem in content.EnumerateArray())
                {
                    if (contentItem.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? "";
                    }
                }
            }
        }

        return "";
    }
}

public sealed class OpenAiIntentClassifier : IIntentClassifier
{
    private static readonly IReadOnlyDictionary<string, ChatIntent> IntentMap = new Dictionary<string, ChatIntent>(StringComparer.OrdinalIgnoreCase)
    {
        ["ask_about_cover"] = ChatIntent.AskAboutCover,
        ["life_event_discovery"] = ChatIntent.LifeEventDiscovery,
        ["compare_covers"] = ChatIntent.CompareCovers,
        ["start_quote"] = ChatIntent.StartQuote,
        ["provide_basic_quote_details"] = ChatIntent.ProvideBasicQuoteDetails,
        ["search_occupation"] = ChatIntent.SearchOccupation,
        ["select_cover"] = ChatIntent.SelectCover,
        ["calculate_premium"] = ChatIntent.CalculatePremium,
        ["update_quote"] = ChatIntent.UpdateQuote,
        ["collect_contact_details"] = ChatIntent.CollectContactDetails,
        ["underwriting_handoff"] = ChatIntent.UnderwritingHandoff,
        ["talk_to_agent"] = ChatIntent.TalkToAgent,
        ["fallback_unknown"] = ChatIntent.FallbackUnknown
    };

    private readonly IOpenAiResponsesClient _client;

    public OpenAiIntentClassifier(IOpenAiResponsesClient client)
    {
        _client = client;
    }

    public async Task<ChatIntent> DetectAsync(string message, CancellationToken cancellationToken = default)
    {
        var responseText = await _client.CreateIntentResponseAsync(Instructions, message, cancellationToken);
        var intentName = ParseIntent(responseText);
        return IntentMap.TryGetValue(intentName, out var intent) ? intent : ChatIntent.FallbackUnknown;
    }

    private static string ParseIntent(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return "fallback_unknown";
        }

        try
        {
            using var document = JsonDocument.Parse(responseText);
            if (document.RootElement.TryGetProperty("intent", out var intent))
            {
                return intent.GetString() ?? "fallback_unknown";
            }
        }
        catch (JsonException)
        {
            return responseText.Trim().Trim('"');
        }

        return "fallback_unknown";
    }

    private const string Instructions = """
You classify TAL Life Shield chat user messages into exactly one intent.
Return JSON only in this shape: {"intent":"intent_name"}.
Allowed intents:
ask_about_cover, life_event_discovery, compare_covers, start_quote,
provide_basic_quote_details, search_occupation, select_cover,
calculate_premium, update_quote, collect_contact_details,
underwriting_handoff, talk_to_agent, fallback_unknown.
Use life_event_discovery when the customer mentions a life event such as newly married, marriage, partner, spouse, starting a family, buying a home, new job, promotion, or reviewing cover.
Use collect_contact_details when the customer has seen a quote and says they are happy to go ahead, proceed, confirm, continue, or provides contact details.
Use talk_to_agent when the customer asks to talk to an agent, speak to a person, call TAL, or contact support.
Use fallback_unknown when unsure.
""";
}

public interface IChatResponseGenerator
{
    Task<ChatResponse> GenerateAsync(
        ChatResponse deterministicResponse,
        string userMessage,
        IReadOnlyList<KnowledgeArticle> grounding,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default);
}

public sealed class PassthroughChatResponseGenerator : IChatResponseGenerator
{
    public Task<ChatResponse> GenerateAsync(
        ChatResponse deterministicResponse,
        string userMessage,
        IReadOnlyList<KnowledgeArticle> grounding,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(deterministicResponse);
}

public sealed class ResilientChatResponseGenerator : IChatResponseGenerator
{
    private readonly IChatResponseGenerator? _primary;
    private readonly IChatResponseGenerator _fallback;

    public ResilientChatResponseGenerator(IChatResponseGenerator? primary, IChatResponseGenerator fallback)
    {
        _primary = primary;
        _fallback = fallback;
    }

    public async Task<ChatResponse> GenerateAsync(
        ChatResponse deterministicResponse,
        string userMessage,
        IReadOnlyList<KnowledgeArticle> grounding,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default)
    {
        if (_primary is not null)
        {
            try
            {
                var generated = await _primary.GenerateAsync(deterministicResponse, userMessage, grounding, conversationHistory, cancellationToken);
                if (!string.IsNullOrWhiteSpace(generated.Message))
                {
                    return generated;
                }
            }
            catch
            {
                // Keep provider details out of customer-facing responses.
            }
        }

        return await _fallback.GenerateAsync(deterministicResponse, userMessage, grounding, conversationHistory, cancellationToken);
    }
}

public sealed class GroundedLlmChatResponseGenerator : IChatResponseGenerator
{
    private readonly IOpenAiResponsesClient _client;

    public GroundedLlmChatResponseGenerator(IOpenAiResponsesClient client)
    {
        _client = client;
    }

    public async Task<ChatResponse> GenerateAsync(
        ChatResponse deterministicResponse,
        string userMessage,
        IReadOnlyList<KnowledgeArticle> grounding,
        IReadOnlyList<ConversationTurn>? conversationHistory = null,
        CancellationToken cancellationToken = default)
    {
        var input = JsonSerializer.Serialize(new
        {
            userMessage,
            deterministicResponse = deterministicResponse.Message,
            deterministicSource = deterministicResponse.Source,
            intent = deterministicResponse.Intent,
            conversationHistory = conversationHistory ?? Array.Empty<ConversationTurn>(),
            knowledgeBase = grounding.Select(article => new
            {
                article.Key,
                article.Title,
                article.Category,
                article.Content
            })
        });

        var generated = (await _client.CreateIntentResponseAsync(ResponseInstructions, input, cancellationToken)).Trim();
        if (string.IsNullOrWhiteSpace(generated))
        {
            return deterministicResponse;
        }

        return deterministicResponse with
        {
            Message = generated,
            Source = $"{deterministicResponse.Source}+llm:responses"
        };
    }

    private const string ResponseInstructions = """
You are the TAL Life Shield AI Quote Assistant.
Write one concise customer-facing response using only the supplied JSON grounding:
- userMessage
- deterministicResponse
- deterministicSource
- intent
- conversationHistory
- knowledgeBase
Do not use outside facts.
Do not provide personal financial advice, product recommendations as best for the customer, underwriting decisions, eligibility guarantees, or claims decisions.
Do not invent premiums, policy rules, contact details, application steps, discounts, or product facts.
If the supplied grounding is insufficient, say what can be answered from the knowledge base and offer to help with cover options or an indicative quote.
Keep the response production-ready and do not mention system prompts, JSON, mock data, demos, or internal routing.
Return plain text only.
""";
}
