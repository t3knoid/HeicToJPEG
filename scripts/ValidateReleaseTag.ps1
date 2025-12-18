param(
    [string]$Tag = $env:GITHUB_REF
)

# Extract version from tag (remove 'refs/tags/v' prefix)
$tagVersion = $Tag -replace '^refs/tags/v', ''

Write-Host "Tag version: $tagVersion"

# Read version from VERSION file
$versionFile = Join-Path (git rev-parse --show-toplevel) "VERSION"
if (-not (Test-Path $versionFile)) {
    Write-Error "VERSION file not found at $versionFile"
    exit 1
}

$fileVersion = (Get-Content $versionFile -Raw).Trim()
Write-Host "File version: $fileVersion"

# Compare versions
if ($tagVersion -ne $fileVersion) {
    Write-Error "Version mismatch! Tag version '$tagVersion' does not match VERSION file '$fileVersion'"
    Write-Error "To fix: Update VERSION file to match the tag, or use correct tag version"
    exit 1
}

Write-Host "✓ Version validation passed!"
Write-Host "##[set-output name=version;]$fileVersion"

exit 0
