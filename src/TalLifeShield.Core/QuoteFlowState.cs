namespace TalLifeShield.Core;

public sealed class QuoteFlowState
{
    public int? BirthYear { get; init; }
    public string? Gender { get; init; }
    public bool? IsSmoker { get; init; }
    public string? Occupation { get; init; }
    public string? EmploymentStatus { get; init; }
    public decimal? AnnualIncome { get; init; }
    public string? CoverCode { get; init; }
    public decimal? SumInsured { get; init; }
    public bool IndicativeQuoteShown { get; init; }
    public bool WantsToContinue { get; init; }

    public bool HasMinimumQuoteDetails =>
        BirthYear.HasValue
        && !string.IsNullOrWhiteSpace(Gender)
        && IsSmoker.HasValue
        && !string.IsNullOrWhiteSpace(Occupation);

    public bool CanCalculatePremium => HasMinimumQuoteDetails;

    public bool CanRequestPii => IndicativeQuoteShown && WantsToContinue;

    public IReadOnlyList<string> MissingMinimumFields()
    {
        var missing = new List<string>();
        if (!BirthYear.HasValue) missing.Add("birth year");
        if (string.IsNullOrWhiteSpace(Gender)) missing.Add("gender/sex");
        if (!IsSmoker.HasValue) missing.Add("smoker status");
        if (string.IsNullOrWhiteSpace(Occupation)) missing.Add("occupation");
        return missing;
    }

    public static IReadOnlyList<string> WorkflowSteps => new[]
    {
        "Quote Generated",
        "Underwriting Questions",
        "Review Cover",
        "Submit Application",
        "Application Received"
    };
}
