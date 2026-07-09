using TalLifeShield.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<KnowledgeBase>();
builder.Services.AddSingleton<IntentRouter>();
builder.Services.AddSingleton<MockEligibilityService>();
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new OpenAiIntentOptions
    {
        Endpoint = configuration["TAL_LIFE_SHIELD_OPENAI_ENDPOINT"]
            ?? configuration["AZURE_OPENAI_ENDPOINT"],
        ApiKey = configuration["TAL_LIFE_SHIELD_OPENAI_API_KEY"]
            ?? configuration["AZURE_OPENAI_API_KEY"],
        Model = configuration["TAL_LIFE_SHIELD_OPENAI_MODEL"]
            ?? configuration["AZURE_OPENAI_DEPLOYMENT"]
    };
});
builder.Services.AddHttpClient<OpenAiResponsesClient>();
builder.Services.AddSingleton<RuleBasedIntentClassifier>();
builder.Services.AddSingleton<IIntentClassifier>(sp =>
{
    var options = sp.GetRequiredService<OpenAiIntentOptions>();
    var fallback = sp.GetRequiredService<RuleBasedIntentClassifier>();
    IIntentClassifier? primary = options.IsConfigured
        ? new OpenAiIntentClassifier(sp.GetRequiredService<OpenAiResponsesClient>())
        : null;
    return new ResilientIntentClassifier(primary, fallback);
});
builder.Services.AddSingleton<IChatResponseGenerator>(sp =>
{
    var options = sp.GetRequiredService<OpenAiIntentOptions>();
    var fallback = new PassthroughChatResponseGenerator();
    IChatResponseGenerator? primary = options.IsConfigured
        ? new GroundedLlmChatResponseGenerator(sp.GetRequiredService<OpenAiResponsesClient>())
        : null;
    return new ResilientChatResponseGenerator(primary, fallback);
});
builder.Services.AddSingleton<IQuickQuotePresenter>(sp =>
{
    var options = sp.GetRequiredService<OpenAiIntentOptions>();
    var fallback = new PassthroughQuickQuotePresenter();
    IQuickQuotePresenter? primary = options.IsConfigured
        ? new GroundedLlmQuickQuotePresenter(sp.GetRequiredService<OpenAiResponsesClient>())
        : null;
    return new ResilientQuickQuotePresenter(primary, fallback);
});
builder.Services.AddSingleton<QuickQuotePresentationService>();
builder.Services.AddSingleton<ChatOrchestrator>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "TAL Life Shield API" }));

app.MapGet("/api/eligibility/application", (MockEligibilityService service) =>
    Results.Json(service.GetEligibilityApplication()));

app.MapGet("/api/occupations", (string? pattern, MockEligibilityService service) =>
    Results.Json(service.GetOccupations(pattern)));

app.MapGet("/api/product-covers", (MockEligibilityService service) =>
    Results.Json(service.GetProductCovers()));

app.MapPost("/api/quotes", (CreateQuoteRequest request, MockEligibilityService service) =>
    Results.Json(service.CreateQuote(request)));

app.MapGet("/api/quotes/{quoteRefNo}/select-cover", (string quoteRefNo, MockEligibilityService service) =>
    Results.Json(service.GetSelectCover(quoteRefNo)));

app.MapPost("/api/quotes/{quoteRefNo}/premium-calculation", (string quoteRefNo, PremiumCalculationRequest request, MockEligibilityService service) =>
    Results.Json(service.CalculateQuotePremium(quoteRefNo, request)));

app.MapPut("/api/quotes/{quoteRefNo}", (string quoteRefNo, UpdateQuoteRequest request, MockEligibilityService service) =>
    Results.Json(service.UpdateQuote(quoteRefNo, request)));

app.MapGet("/api/knowledge-base", (KnowledgeBase knowledgeBase) =>
    Results.Ok(knowledgeBase.Articles));

app.MapPost("/api/chat/intent", async (ChatMessageRequest request, IIntentClassifier classifier, CancellationToken cancellationToken) =>
{
    var intent = await classifier.DetectAsync(request.Message ?? "", cancellationToken);
    return Results.Ok(new { intent = IntentRouter.ToWireName(intent) });
});

app.MapPost("/api/chat/message", async (ChatMessageRequest request, ChatOrchestrator orchestrator, CancellationToken cancellationToken) =>
    Results.Ok(await orchestrator.RespondAsync(request.Message ?? "", request.State, request.ConversationHistory, cancellationToken)));

app.MapPost("/api/chat/quick-quote", async (QuickQuotePresentationRequest request, QuickQuotePresentationService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.PresentAsync(request, cancellationToken)));

app.MapFallbackToFile("index.html");

app.Run();

public sealed record ChatMessageRequest(string? Message, QuoteFlowState? State, IReadOnlyList<ConversationTurn>? ConversationHistory);
