param(
    [switch]$DryRun = $false
)

# Read version from VERSION file
$versionFile = Join-Path (git rev-parse --show-toplevel) "VERSION"
if (-not (Test-Path $versionFile)) {
    Write-Error "VERSION file not found at $versionFile"
    exit 1
}

$baseVersion = (Get-Content $versionFile -Raw).Trim()
Write-Host "Base version from VERSION file: $baseVersion"

# Get commit count for build number
$commitCount = git rev-list --count HEAD
$fullVersion = "$baseVersion.$commitCount"

# Ensure version follows semantic versioning for assembly version (add .0 if needed)
$parts = $baseVersion -split '\.'

# Pad to 4 parts for assembly version (major.minor.patch.0)
while ($parts.Count -lt 4) {
    $parts += "0"
}
$assemblyVersion = $parts[0..3] -join '.'

# For file version, use full version with commit count
$fileVersion = $assemblyVersion -replace '\.0$', ".$commitCount"

Write-Host "Full version (with build): $fullVersion"
Write-Host "Assembly Version: $assemblyVersion"
Write-Host "File Version: $fileVersion"

# Find all AssemblyInfo.cs files
$assemblyInfoFiles = Get-ChildItem -Recurse -Filter "AssemblyInfo.cs"

foreach ($file in $assemblyInfoFiles) {
    Write-Host "Processing: $($file.FullName)"
    
    $content = Get-Content $file.FullName -Raw
    
    # Update AssemblyVersion
    $content = $content -replace '\[assembly: AssemblyVersion\(".*?"\)\]', "[assembly: AssemblyVersion(`"$assemblyVersion`")]"
    
    # Update AssemblyFileVersion
    $content = $content -replace '\[assembly: AssemblyFileVersion\(".*?"\)\]', "[assembly: AssemblyFileVersion(`"$fileVersion`")]"
    
    if (-not $DryRun) {
        Set-Content $file.FullName -Value $content -NoNewline
        Write-Host "  ✓ Updated"
    } else {
        Write-Host "  [DRY RUN] Would update"
    }
}

# Output version for use in GitHub Actions
Write-Host "Base version: $baseVersion"
Write-Host "Full version: $fullVersion"
Write-Host "Assembly version: $assemblyVersion"

if ($env:GITHUB_OUTPUT) {
    Add-Content -Path $env:GITHUB_OUTPUT -Value "version=$baseVersion"
    Add-Content -Path $env:GITHUB_OUTPUT -Value "full-version=$fullVersion"
    Add-Content -Path $env:GITHUB_OUTPUT -Value "assembly-version=$assemblyVersion"
}

exit 0
