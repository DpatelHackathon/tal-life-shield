param(
    [string]$ChangeDocsPath = "docs",
    [switch]$RequireFinal
)

$ErrorActionPreference = "Stop"

function Fail([string]$Message) {
    Write-Host "SDD GATE FAIL: $Message" -ForegroundColor Red
    exit 1
}

function PassLine([string]$Message) {
    Write-Host "PASS: $Message" -ForegroundColor Green
}

$root = (Resolve-Path ".").Path
$docsPath = Join-Path $root $ChangeDocsPath

if (-not (Test-Path -LiteralPath $docsPath)) {
    Fail "Change docs path does not exist: $ChangeDocsPath"
}

$required = @(
    "SPEC.md",
    "ACCEPTANCE_CRITERIA.md",
    "TRACEABILITY_MATRIX.md",
    "DEPENDENCY_GRAPH.md",
    "IMPLEMENTATION_PLAN.md",
    "TEST_PLAN.md",
    "SPEC_VERIFICATION.md"
)

foreach ($file in $required) {
    $path = Join-Path $docsPath $file
    if (-not (Test-Path -LiteralPath $path)) {
        Fail "Required SDD document missing: $ChangeDocsPath/$file"
    }
}

PassLine "Required SDD documents exist"

$acPath = Join-Path $docsPath "ACCEPTANCE_CRITERIA.md"
$tracePath = Join-Path $docsPath "TRACEABILITY_MATRIX.md"
$testPath = Join-Path $docsPath "TEST_PLAN.md"
$verificationPath = Join-Path $docsPath "SPEC_VERIFICATION.md"

$acText = Get-Content -LiteralPath $acPath -Raw
$traceText = Get-Content -LiteralPath $tracePath -Raw
$testText = Get-Content -LiteralPath $testPath -Raw
$verificationText = Get-Content -LiteralPath $verificationPath -Raw

$acIds = [regex]::Matches($acText, "AC-\d{3}") |
    ForEach-Object { $_.Value } |
    Sort-Object -Unique

if ($acIds.Count -eq 0) {
    Fail "No acceptance criteria IDs found in $ChangeDocsPath/ACCEPTANCE_CRITERIA.md"
}

foreach ($ac in $acIds) {
    if ($traceText -notmatch [regex]::Escape($ac)) {
        Fail "$ac is missing from TRACEABILITY_MATRIX.md"
    }
    if ($testText -notmatch [regex]::Escape($ac)) {
        Fail "$ac is missing from TEST_PLAN.md"
    }
}

PassLine "Every AC appears in traceability and test plan"

if ($verificationText -notmatch "SPEC gate status:\s*Pass") {
    Fail "SPEC_VERIFICATION.md must contain 'SPEC gate status: Pass'"
}

PassLine "SPEC gate is marked Pass"

if ($RequireFinal) {
    $summaryPath = Join-Path $docsPath "FINAL_SDLC_SUMMARY.md"
    if (-not (Test-Path -LiteralPath $summaryPath)) {
        Fail "FINAL_SDLC_SUMMARY.md is required for final gate"
    }

    $summaryText = Get-Content -LiteralPath $summaryPath -Raw
    foreach ($ac in $acIds) {
        if ($summaryText -notmatch [regex]::Escape($ac)) {
            Fail "$ac is missing from FINAL_SDLC_SUMMARY.md"
        }
    }

    if ($traceText -match "\|\s*Planned\s*\|") {
        Fail "TRACEABILITY_MATRIX.md still contains Planned verification rows"
    }

    if ($summaryText -notmatch "Final Gate Result\s*\r?\n\s*Pass") {
        Fail "FINAL_SDLC_SUMMARY.md must include a passing Final Gate Result"
    }

    PassLine "Final summary covers every AC and traceability has no planned rows"
}

Write-Host "SDD GATE PASS: $ChangeDocsPath" -ForegroundColor Green
