$ErrorActionPreference = "Stop"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$env:DOTNET_NOLOGO = "1"

$ProjectDir = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $ProjectDir "build\publish"
$OutputExe = Join-Path $ProjectDir "build\LiteTick.exe"
$VersionFile = Join-Path $ProjectDir "Support\Version.props"

[xml]$VersionDocument = Get-Content $VersionFile
$VersionGroup = $VersionDocument.Project.PropertyGroup
$VersionGroup.BuildNumber = ([int]$VersionGroup.BuildNumber + 1).ToString()
$VersionDocument.Save($VersionFile)

dotnet publish (Join-Path $ProjectDir "LiteTick.Windows.csproj") `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output $PublishDir

$PublishedFiles = @(Get-ChildItem -File $PublishDir)
if ($PublishedFiles.Count -ne 1 -or $PublishedFiles[0].Name -ne "LiteTick.exe") {
    throw "Expected exactly one published file named LiteTick.exe"
}

Copy-Item $PublishedFiles[0].FullName $OutputExe -Force
dotnet run --project (Join-Path $ProjectDir "tests\CoreSmoke\CoreSmoke.csproj") --configuration Release
dotnet run --project (Join-Path $ProjectDir "tests\PeVerify\PeVerify.csproj") --configuration Release -- `
    $OutputExe $VersionGroup.MarketingVersion $VersionGroup.BuildNumber $VersionGroup.ReleaseChannel modern
Write-Host $OutputExe
