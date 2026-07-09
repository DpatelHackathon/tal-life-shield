using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading;

namespace TalLifeShield.Core;

public sealed record BasicInformationInput(
    string? BirthDate,
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? PhoneNo,
    string? GenderCode,
    bool? IsSmoker,
    string? OccupationCode,
    decimal? AnnualIncome,
    bool? IsSelfEmployed,
    string? Postcode);

public sealed record CreateQuoteRequest(
    BasicInformationInput? BasicInformation,
    string? MediaCode,
    string? SourceOfBusiness,
    string? RecaptchaToken,
    string? RefSession,
    string? RefSessionId);

public sealed record PremiumCalculationRequest(string? ProductCoverCode, decimal? SumInsured);

public sealed record UpdateQuoteRequest(
    string? ProductCoverCode,
    decimal? SumInsured,
    decimal? HeightCm,
    decimal? WeightKg,
    bool ApplyDiscount);

public sealed class MockEligibilityService
{
    public const string SampleQuoteRefNo = "Q3350008";
    public const decimal SampleLifePremium = 220.93m;
    private readonly ConcurrentDictionary<string, BasicInformationInput> _quoteInputs = new();
    private int _quoteSequence = 3350007;

    public Dictionary<string, object?> GetEligibilityApplication() => Envelope("getEligibilityApplication",
        new Dictionary<string, object?>
        {
            ["applicationStatus"] = new Dictionary<string, object?>
            {
                ["status"] = Ref("NewBusiness", "New Business"),
                ["__typename"] = "ApplicationStatusType"
            },
            ["__typename"] = "EligibilityType"
        });

    public Dictionary<string, object?> GetOccupations(string? pattern)
    {
        var all = new List<Dictionary<string, object?>>
        {
            Ref("1067", "Accountant - Not university qualified"),
            Ref("1144", "Accounts Clerk"),
            Ref("1232", "Advertising - Agent / Account Executive / Clerical staff"),
            Ref("1244", "Advertising - Principal / Account Executive - Office only"),
            Ref("1909", "Account Executive"),
            Ref("2020", "Accountant - University qualified")
        };

        var normalized = pattern?.ToLowerInvariant() ?? "";
        var occupations = string.IsNullOrWhiteSpace(normalized)
            ? all
            : all.Where(item => item["description"]?.ToString()?.ToLowerInvariant().Contains(normalized) == true
                || item["code"]?.ToString()?.Contains(normalized) == true
                || normalized == "acc").ToList();

        return Envelope("getOccupations", new Dictionary<string, object?>
        {
            ["occupations"] = occupations,
            ["__typename"] = "EligibilityType"
        });
    }

    public Dictionary<string, object?> GetProductCovers() => Envelope("getProductCovers",
        new Dictionary<string, object?>
        {
            ["productCovers"] = new List<Dictionary<string, object?>>
            {
                ProductCover("CI", "Critical Illness Insurance Plan",
                new List<Dictionary<string, object?>>
                {
                    CoverLevel("ST", "Standard", true, new List<Dictionary<string, object?>>(), new List<Dictionary<string, object?>>()),
                    CoverLevel("PR", "Premier", false, new List<Dictionary<string, object?>>(), new List<Dictionary<string, object?>>())
                }),
                ProductCover("IP", "Income Protection Plan",
                new List<Dictionary<string, object?>>
                {
                    CoverLevel("ST", "Standard", true,
                    new List<Dictionary<string, object?>>
                    {
                        Ref("1Y", "1 Year"),
                        Ref("2Y", "2 Year"),
                        Ref("5Y", "5 Year"),
                        Ref("TO65", "to age 65"),
                        Ref("TO70", "to age 70")
                    },
                    new List<Dictionary<string, object?>>
                    {
                        Ref("2W", "2 Weeks"),
                        Ref("4W", "4 Weeks"),
                        Ref("8W", "8 Weeks"),
                        Ref("13W", "13 Weeks"),
                        Ref("26W", "26 Weeks"),
                        Ref("52W", "52 Weeks"),
                        Ref("104W", "104 Weeks")
                    })
                }),
                ProductCover("LI", "Life Insurance Plan", new List<Dictionary<string, object?>>()),
                ProductCover("TPD", "TPD Insurance Plan", new List<Dictionary<string, object?>>())
            },
            ["__typename"] = "EligibilityType"
        });

    public Dictionary<string, object?> CreateQuote(CreateQuoteRequest request)
    {
        var basic = request.BasicInformation ?? new BasicInformationInput(
            "2001-03-01T00:00:00",
            "Demo",
            "Customer",
            null,
            null,
            "M",
            false,
            "1067",
            2000000m,
            true,
            null);

        var quoteRefNo = $"Q{Interlocked.Increment(ref _quoteSequence)}";
        _quoteInputs[quoteRefNo] = basic;

        return Envelope("setBasicInfo", new Dictionary<string, object?>
        {
            ["quoteRefNo"] = quoteRefNo,
            ["acceptedRequestMetadata"] = new Dictionary<string, object?>
            {
                ["mediaCode"] = request.MediaCode ?? "TALC002",
                ["sourceOfBusiness"] = request.SourceOfBusiness ?? "TALD",
                ["refSession"] = request.RefSession ?? " ",
                ["refSessionId"] = request.RefSessionId ?? " ",
                ["recaptchaTokenAcceptedForMock"] = !string.IsNullOrWhiteSpace(request.RecaptchaToken)
            },
            ["basicInformation"] = new Dictionary<string, object?>
            {
                ["firstName"] = basic.FirstName ?? "Demo",
                ["lastName"] = basic.LastName ?? "Customer",
                ["birthDate"] = basic.BirthDate ?? "2001-03-01T00:00:00",
                ["gender"] = Ref(basic.GenderCode ?? "M", null),
                ["isSmoker"] = basic.IsSmoker ?? false,
                ["occupation"] = Ref(basic.OccupationCode ?? "1067", null),
                ["__typename"] = "BasicInformationType"
            },
            ["quickQuote"] = new Dictionary<string, object?>
            {
                ["covers"] = new[] { "LI", "TPD", "CI", "IP" }
                    .Select(_ => new Dictionary<string, object?>
                    {
                        ["sumInsured"] = null,
                        ["premiumAmt"] = null,
                        ["isEligible"] = true,
                        ["__typename"] = "QuickQuoteCoverType"
                    }).ToList(),
                ["__typename"] = "QuickQuoteType"
            },
            ["__typename"] = "EligibilityType"
        });
    }

    public Dictionary<string, object?> GetSelectCover(string quoteRefNo) => Envelope("getSelectCover",
        SelectCoverPayload(quoteRefNo, selected: false, discounted: false));

    public Dictionary<string, object?> CalculateQuotePremium(string quoteRefNo, PremiumCalculationRequest request)
    {
        var premium = CalculatePremiumAmount(quoteRefNo, request);

        return Envelope("quotePremiumCalculation", new Dictionary<string, object?>
        {
            ["quickQuote"] = new Dictionary<string, object?>
            {
                ["covers"] = new List<Dictionary<string, object?>>
                {
                    new()
                    {
                        ["productCover"] = Ref(request.ProductCoverCode ?? "LI", null),
                        ["premiumAmt"] = premium,
                        ["__typename"] = "QuickQuoteCoverType"
                    }
                },
                ["__typename"] = "QuickQuoteType"
            },
            ["__typename"] = "EligibilityType"
        });
    }

    public Dictionary<string, object?> UpdateQuote(string quoteRefNo, UpdateQuoteRequest request) =>
        Envelope("setSelectCover", SelectCoverPayload(quoteRefNo, selected: true, discounted: request.ApplyDiscount, request.HeightCm, request.WeightKg));

    public string Serialize(object payload) =>
        JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = false });

    private decimal CalculatePremiumAmount(string quoteRefNo, PremiumCalculationRequest request)
    {
        if (!_quoteInputs.TryGetValue(quoteRefNo, out var basic))
        {
            return SampleLifePremium;
        }

        var sumInsuredFactor = Math.Max(0.2m, (request.SumInsured ?? 5000000m) / 5000000m);
        var age = BirthYear(basic.BirthDate) is int birthYear ? Math.Max(18, 2026 - birthYear) : 25;
        var ageFactor = 1m + Math.Max(0, age - 25) * 0.012m;
        var smokerFactor = basic.IsSmoker == true ? 1.35m : 1m;
        var genderFactor = string.Equals(basic.GenderCode, "F", StringComparison.OrdinalIgnoreCase) ? 0.96m : 1m;
        var occupationFactor = string.Equals(basic.OccupationCode, "2020", StringComparison.OrdinalIgnoreCase) ? 0.97m : 1m;

        return Math.Round(SampleLifePremium * sumInsuredFactor * ageFactor * smokerFactor * genderFactor * occupationFactor, 2);
    }

    private static int? BirthYear(string? birthDate)
    {
        if (string.IsNullOrWhiteSpace(birthDate) || birthDate.Length < 4)
        {
            return null;
        }

        return int.TryParse(birthDate[..4], out var year) ? year : null;
    }

    private static Dictionary<string, object?> SelectCoverPayload(
        string quoteRefNo,
        bool selected,
        bool discounted,
        decimal? heightCm = null,
        decimal? weightKg = null)
    {
        var premium = selected ? SampleLifePremium : 0m;

        return new Dictionary<string, object?>
        {
            ["quoteRefNo"] = string.IsNullOrWhiteSpace(quoteRefNo) ? SampleQuoteRefNo : quoteRefNo,
            ["caseId"] = null,
            ["basicInformation"] = BasicInformation(),
            ["isApplyOverPhone"] = selected,
            ["isIpExtendEligible"] = true,
            ["productSeries"] = "46",
            ["productVersion"] = "1",
            ["pricingVersion"] = selected ? "2" : null,
            ["quickQuote"] = new Dictionary<string, object?>
            {
                ["applicationQuoteLifeId"] = "8cdbbbee-737b-49a4-bc27-abe61468a3f5",
                ["totalPremiumAmt"] = premium,
                ["policyFeeAmount"] = selected ? 8.0m : null,
                ["stampDutyAmount"] = 0.0m,
                ["isBMIDiscountEligible"] = selected ? null : null,
                ["isDiscountSelected"] = discounted,
                ["bMIRating"] = null,
                ["weightKg"] = weightKg,
                ["heightCm"] = heightCm,
                ["paymentFrequency"] = Ref("1", "Monthly"),
                ["covers"] = Covers(selected, premium),
                ["__typename"] = "QuickQuoteType"
            },
            ["projections"] = selected ? Projections() : null,
            ["__typename"] = "EligibilityType"
        };
    }

    private static Dictionary<string, object?> BasicInformation() => new()
    {
        ["gender"] = Ref("M", "Male"),
        ["birthDate"] = "2001-03-01T00:00:00",
        ["emailAddress"] = "test@test.com",
        ["phoneNo"] = "0400000000",
        ["isSmoker"] = false,
        ["isSelfEmployed"] = true,
        ["occupation"] = Ref("1067", "Accountant - Not university qualified"),
        ["annualIncome"] = 2000000.00m,
        ["firstName"] = "test",
        ["lastName"] = "test",
        ["postcode"] = "2018",
        ["title"] = "",
        ["__typename"] = "BasicInformationType"
    };

    private static List<Dictionary<string, object?>> Covers(bool selected, decimal premium)
    {
        return new List<Dictionary<string, object?>>
        {
            CoverQuote("LI", "Life Insurance Plan", selected, 5000000.0m, selected ? premium : null, null),
            CoverQuote("TPD", "TPD Insurance Plan", false, null, null,
            new List<Dictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["code"] = "TSO", ["isSelected"] = null, ["__typename"] = "QuickQuoteProductBenefitType" }
            }),
            CoverQuote("CI", "Critical Illness Insurance Plan", false, null, null, null),
            CoverQuote("IP", "Income Protection Plan", false, null, null, null)
        };
    }

    private static Dictionary<string, object?> CoverQuote(
        string code,
        string description,
        bool selected,
        decimal? sumInsured,
        decimal? premium,
        List<Dictionary<string, object?>>? benefits) => new()
    {
        ["isSelected"] = selected,
        ["isEligible"] = true,
        ["productCover"] = Ref(code, description),
        ["productBenefits"] = benefits ?? (code == "LI" ? new List<Dictionary<string, object?>>() : null),
        ["sumInsured"] = sumInsured,
        ["premiumAmt"] = premium,
        ["waitingPeriod"] = null,
        ["benefitPeriod"] = null,
        ["__typename"] = "QuickQuoteCoverType"
    };

    private static List<Dictionary<string, object?>> Projections()
    {
        var yearly = new[] { 2747.16m, 3081.36m, 3426.12m, 3632.76m, 3822.72m, 4011.36m, 4202.28m, 4417.80m, 4608.36m, 4717.44m };
        var projections = new List<Dictionary<string, object?>>();
        decimal cumulative = 0;
        decimal sumInsured = 5000000m;

        for (var i = 0; i < yearly.Length; i++)
        {
            cumulative += yearly[i];
            var year = i + 1;
            projections.Add(new Dictionary<string, object?>
            {
                ["projectionYearNum"] = year,
                ["projectionAgeNextBirthdayNum"] = 25 + year,
                ["totalAnnualisedPremiumAmt"] = yearly[i],
                ["cumulativeAnnualisedPremiumAmt"] = cumulative,
                ["covers"] = new List<Dictionary<string, object?>>
                {
                    new()
                    {
                        ["type"] = "Premium",
                        ["productCover"] = new Dictionary<string, object?>
                        {
                            ["code"] = "LI",
                            ["description"] = "Life Insurance",
                            ["__typename"] = "ReferenceTypeResponseType"
                        },
                        ["projectionYearNum"] = year,
                        ["projectionAgeNextBirthdayNum"] = 25 + year,
                        ["sumInsured"] = sumInsured,
                        ["quotedPremiumAmt"] = yearly[i],
                        ["cumulativeQuotedPremiumAmt"] = cumulative,
                        ["cumulativeYearlyCommissionAmt"] = 0,
                        ["__typename"] = "PricingEngineRetailQuoteLifeCoverProjectionType"
                    }
                },
                ["__typename"] = "PricingEngineRetailQuoteLifeProjectionResponseType"
            });
            sumInsured *= 1.05m;
        }

        return projections;
    }

    private static Dictionary<string, object?> ProductCover(string code, string description, List<Dictionary<string, object?>> coverLevels) => new()
    {
        ["code"] = code,
        ["description"] = description,
        ["coverLevels"] = coverLevels,
        ["__typename"] = "ProductCoverInputQLType"
    };

    private static Dictionary<string, object?> CoverLevel(
        string code,
        string description,
        bool isDefault,
        List<Dictionary<string, object?>> benefitPeriod,
        List<Dictionary<string, object?>> waitingPeriod) => new()
    {
        ["code"] = code,
        ["description"] = description,
        ["isDefault"] = isDefault,
        ["benefitPeriod"] = benefitPeriod,
        ["waitingPeriod"] = waitingPeriod,
        ["__typename"] = "CoverLevelInputQLType"
    };

    private static Dictionary<string, object?> Ref(string code, string? description) => new()
    {
        ["code"] = code,
        ["description"] = description,
        ["__typename"] = "ReferenceType"
    };

    private static Dictionary<string, object?> Envelope(string key, Dictionary<string, object?> payload) => new()
    {
        ["data"] = new Dictionary<string, object?> { [key] = payload }
    };
}
