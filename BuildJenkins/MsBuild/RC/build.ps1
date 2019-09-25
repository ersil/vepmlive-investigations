param(
    [string] $version = $(throw "Must specify version. e.g., -version 7.0.0.1"),
    [string] $rootDir,
    [string] $outputDir
)
$ErrorActionPreference = "Stop"

$TempFileLocation = "$env:windir\Temp","$env:TEMP"
$SoftwareDistributionLocation = "$env:windir\SoftwareDistribution\Download"

$TempFile = Get-ChildItem $TempFileLocation -Recurse
$TempFileCount = ($TempFile).count

if($TempFileCount -eq "0") { 
Write-Host "There are no files in the folder $TempFileLocation" -ForegroundColor Green
}
Else {
$TempFile | Remove-Item -Confirm:$false -Recurse -Force -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
Write-Host "Cleared $TempFileCount files in the folder $TempFileLocation" -ForegroundColor Green
}

$SoftwareDistribution = Get-ChildItem $SoftwareDistributionLocation -Recurse
$SoftwareDistributionCount = ($SoftwareDistribution).Count
if($SoftwareDistributionCount -eq "0"){
Write-Host " There are no files in the folder $SoftwareDistributionLocation" -ForegroundColor Green
}
Else
{
$SoftwareDistribution | Remove-Item -Confirm:$false -Recurse -Force -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
Write-Host "Cleared $SoftwareDistributionCount files in the folder $SoftwareDistributionLocation" -ForegroundColor Green
}

# Define directories
$scriptDir = split-path -parent $MyInvocation.MyCommand.Definition
if ($rootDir -eq '') {
	$rootDir = (Get-Item $scriptDir).Parent.Parent.FullName
}
if (!$outputDir) {
    $outputDir = Join-Path $rootDir "output/EPMLive"
}

# Load routines
. $scriptDir/routines.ps1

# Creating empty output directory
Remove-Item -Path $outputDir -Recurse -ErrorAction:SilentlyContinue
New-Item -Path $outputDir -ItemType Directory -Force > $null

Set-Location -Path $rootDir

Write-Host "Write build number"
"[assembly:System.Reflection.AssemblyFileVersion(""$version"")]" > CommonAssemblyInfo.cs
$env:build_number = $version


Write-Host "Marking fake dlls as recent"
Get-ChildItem -Include *.dll -Recurse | Where-Object {$_.FullName -like '*\FakesAssemblies\*.dll'} | ForEach-Object { $_.LastWriteTime = [System.DateTime]::Now }

Write-Host "Execute builds"
Write-Host "Executing tl_buildCode.ps1"
& .\BuildTeamCity\tl_buildCode.ps1 -ConfigurationToBuild Release


Write-Host "Executing tl_buildInstaller.ps1"
& .\BuildTeamCity\tl_buildInstaller.ps1 


$buildRoot = Join-Path $rootDir "BuildTeamCity"
Set-Location -Path $buildRoot

# Execute ps1 for EPM Live (generates config.json and other required wsp or dll files)
Write-Host "Executing tl_buildSilentInstaller.ps1"
& .\tl_buildSilentInstaller.ps1


Write-Host "Copy Silent installer to output dir"
$installerDir = Join-Path $buildRoot "SilentInstaller"
Copy-Item -Path (Join-Path $installerDir "*") -Destination $outputDir -Recurse

Write-Host "Done. Installer is located at $outputDir"


