param (
    [string]$Configuration = "Release",
    [string]$VersionSuffix = ""
)

$projectPath = "src\Huskui.Avalonia\Huskui.Avalonia.csproj"
$outputPath = "nupkg"

# Create output directory if it doesn't exist
if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
    Write-Host "Created output directory: $outputPath"
}

# Clean previous builds
Write-Host "Cleaning previous builds..."
dotnet clean $projectPath -c $Configuration

# Build and create NuGet package
Write-Host "Building project and creating NuGet package..."
if ([string]::IsNullOrEmpty($VersionSuffix)) {
    dotnet pack $projectPath -c $Configuration -o $outputPath
} else {
    dotnet pack $projectPath -c $Configuration --version-suffix $VersionSuffix -o $outputPath
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "NuGet package created successfully in the '$outputPath' directory."
    Get-ChildItem $outputPath | Where-Object { $_.Extension -eq ".nupkg" } | ForEach-Object {
        Write-Host "Package: $($_.Name)"
    }
} else {
    Write-Host "Failed to create NuGet package." -ForegroundColor Red
}
