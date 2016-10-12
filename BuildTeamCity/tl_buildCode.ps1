# Build script for EPMLive
# 2016.02.24 - Made changes to work with the build post-removal of circular dependencies

# ### Define user adjustable parameters

param (
    # MSBuild - which configuration to build
    [string]$ConfigurationToBuild = "Release",
    # MSBuild - for which platform to make builds
    [string]$PlatformToBuild = "Any CPU",
    # Tools Version to pass to MSBuild
    [string]$ToolsVersion = "/tv:14.0",
    # user-specific additional command line parameters to pass to MSBuild
    [string]$MsBuildArguments = "/p:visualstudioversion=14.0",
    # should build cleanup be performed before making build
    [string]$CleanBuild = $true
);

$projectsToBePackaged = @("EPMLiveCore", "EPMLiveDashboards","EPMLiveIntegrationService",
                            "EPMLivePS","EPMLiveReporting","EPMLiveSynch",
                            "EPMLiveTimeSheets","EPMLiveWebParts","EPMLiveWorkPlanner","WorkEnginePPM")

$projectsToBeBuildAsEXE = @("EPMLiveTimerService", "EPK_QueueMgr")
$projectsToBeBuildAsDLL = @("PortfolioEngineCore","UplandIntegrations","EPMLiveIntegration", "UserNameChecker", "SPEmulators", ".Tests")

$projectTypeIdTobeReplaced = "C1CDDADD-2546-481F-9697-4EA41081F2FC"
$projectTypeIdTobeReplacedWith = "BB1F664B-9266-4fd6-B973-E1E44974B511"

# Define script directory
$ScriptDir = split-path -parent $MyInvocation.MyCommand.Definition

$env:EnableNuGetPackageRestore = "true"

# ### Includes 

# look-up of dependent libs
. $ScriptDir\RefsLocate.ps1


# ### Logging helpers

function Log-Section($sectionName) {
	Write-Host "============================================================"
	Write-Host "`t $sectionName"
	Write-Host "============================================================"
}

function Log-SubSection($subsectionName) {
	Write-Host "------------------------------------------------------------"
	Write-Host "`t $subsectionName"
	Write-Host "------------------------------------------------------------"
}

function Log-Message($msg) {
	Write-Host $msg
}


$BuildDirectory = "$ScriptDir\..\..\"

# additional parameters to msbuild
if (Test-Path env:\DF_MSBUILD_BUILD_STATS_OPTS) {
	$DfMsBuildArgs = Get-Childitem env:DF_MSBUILD_BUILD_STATS_OPTS | % { $_.Value }
}
# msbuild executable location
# $MSBuildExec = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$MSBuildExec = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
# VSTest executable
$VSTestExec = "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
# Initialize Sources Directory
$SourcesDirectory = "$ScriptDir\..\"

# Directory for outputs
$OutputDirectory = Join-Path $BuildDirectory "output"
# Initialize Binaries Directory
$BinariesDirectory = Join-Path $OutputDirectory "binaries"
# Initialize merged binaries folder
# This directory holds "Single-Folder" build output of all projects
# This is used as a repository to look up dependent DLLs for projects when  
# packaging libs for each project in a separate folder.
# Initialize Libraries Directory
$LibrariesDirectory = "$OutputDirectory\libraries"
# Initialize intermediates directory (PDB)
$IntermediatesDirectory = "$OutputDirectory\intermediate"
# Initialize logs directory
$LogsDirectory = "$BuildDirectory\logs"
if (!(Test-Path -Path $LogsDirectory )){
    New-Item $LogsDirectory -type Directory
}

$projAbsPath = Join-Path $SourcesDirectory "EPMLive.sln"
$projDir = Split-Path $projAbsPath -parent
$projName = [System.IO.Path]::GetFileNameWithoutExtension($projAbsPath) 


### Build preparation steps

# set timezone to UTC - for aline to correctly report on time spent in build tasks
#& tzutil /s "UTC"


Log-Section "Build configuration"
Log-Message "`t Configuration: '$ConfigurationToBuild'"
Log-Message "`t Platform: '$PlatformToBuild'"
Log-Message "`t OutDir: '$BinariesDirectory'"
Log-Message "`t DF MSBuild arguments: '$DfMsBuildArgs'"
Log-Message "`t Additional MSBuild arguments: '$MsBuildArguments'"
Log-Message ""

#  clean previous build outputs
If ($CleanBuild -eq $true) {
	Log-Section "Cleaning build outputs..."
	
	if (Test-Path $OutputDirectory) {
	    Remove-Item -Recurse -Force $OutputDirectory
	}
	New-Item -ItemType directory -Force -Path $OutputDirectory

	# Run MSBuild
	 
    
	Log-SubSection "Cleaning '$projName'..."
	    
	& $MSBuildExec "$projAbsPath"  `
	    /t:Clean `
	    /p:SkipInvalidConfigurations=true `
	    /p:Configuration="$ConfigurationToBuild" `
	    /p:Platform="$PlatformToBuild" `
        /m:4 `
        /p:WarningLevel=0 `
        $ToolsVersion `
	    $DfMsBuildArgs `
	    $MsBuildArguments
	if ($LastExitCode -ne 0) {
		throw "Project clean-up failed with exit code: $LastExitCode."
	}
		
	
}

Log-Section "Downloading Nuget . . ."
$nugetPath = $SourcesDirectory + "\nuget.exe"
Invoke-WebRequest -Uri http://nuget.org/nuget.exe -OutFile $nugetPath


Log-Section "Restoring missing packages . . ."
& $nugetPath `
restore `
$projAbsPath

# ### Make build the same way SolutionPackager does

Log-Section "Starting build..."



$loggerArgs = "LogFile=$LogsDirectory\${projName}.log;Verbosity=normal;Encoding=Unicode"
$outDir = Join-Path $BinariesDirectory $projName
$langversion = "Default"
    
Log-SubSection "Building '$projName'..."
    
# Run MSBuild
& $MSBuildExec $projAbsPath `
    /p:PreBuildEvent= `
    /p:PostBuildEvent= `
    /p:Configuration="$ConfigurationToBuild" `
    /p:Platform="$PlatformToBuild" `
	/p:langversion="$langversion" `
    /p:WarningLevel=0 `
    /p:GenerateSerializationAssemblies="Off" `
    /p:ReferencePath="C:\Program Files (x86)\Microsoft SDKs\Project 2013\REDIST" `
    /fl /flp:"$loggerArgs" `
    /m:4 `
    $ToolsVersion `
	$DfMsBuildArgs `
	$MsBuildArguments  
if ($LastExitCode -ne 0) {
    throw "Project build failed with exit code: $LastExitCode."
}

Log-Section "Creating Output Folders . . ."

if (!(Test-Path -Path $LibrariesDirectory)){
    New-Item $LibrariesDirectory -ItemType Directory
}
if (!(Test-Path -Path $IntermediatesDirectory)){
    New-Item $IntermediatesDirectory -ItemType Directory
}
if (!(Test-Path -Path $BinariesDirectory)){
    New-Item $BinariesDirectory -ItemType Directory
}

#Log-Section "Removing Backup directory that is checked into SCM"
#Remove-Item C:\opt\dfinstaller\Source\Backup -recurse

Log-Section "Packaging Projects . . ."
foreach($projectToBePackaged in $projectsToBePackaged){
    
    $projectPath = Get-ChildItem -Path ($SourcesDirectory + "\*") -Include ($projectToBePackaged + ".csproj") -Recurse

    #Log-SubSection "Patching Project Type GUID '$projectToBePackaged'...."
    
    #(Get-Content $projectPath).Replace($projectTypeIdTobeReplaced,$projectTypeIdTobeReplacedWith) | Set-Content $projectPath

    Log-SubSection "Packaging '$projectToBePackaged'..."
	Log-SubSection "projectPath: '$projectPath'...."
    
   & $MSBuildExec $projectPath `
   /t:Package `
   /p:OutputPath="$BinariesDirectory" `
   /p:PreBuildEvent= `
   /p:PostBuildEvent= `
   /p:WarningLevel=0 `
   /p:Configuration="$ConfigurationToBuild" `
   /p:Platform="$PlatformToBuild" `
    /p:langversion="$langversion" `
   /p:GenerateSerializationAssemblies="Off" `
   /p:ReferencePath="C:\Program Files (x86)\Microsoft SDKs\Project 2013\REDIST" `
    /fl /flp:"$loggerArgs" `
    /m:4 `
    $ToolsVersion `
	$DfMsBuildArgs `
 	$MsBuildArguments  
    if ($LastExitCode -ne 0) {
        throw "Project build failed with exit code: $LastExitCode."
    }
}

Log-Section "Building Windows Services Projects . . ."
foreach($projectToBeBuildAsEXE in $projectsToBeBuildAsEXE){
    
    $projectPath = Get-ChildItem -Path ($SourcesDirectory + "\*") -Include ($projectToBeBuildAsEXE + ".csproj") -Recurse

    Log-SubSection "Building '$projectToBeBuildAsEXE'..."
	Log-SubSection "projectPath: '$projectPath'...."
    
   & $MSBuildExec $projectPath `
   /t:build `
   /p:OutputPath="$BinariesDirectory" `
   /p:PreBuildEvent= `
   /p:PostBuildEvent= `
   /p:Configuration="$ConfigurationToBuild" `
   /p:Platform="x64" `
    /p:langversion="$langversion" `
   /p:GenerateSerializationAssemblies="Off" `
   /p:ReferencePath="C:\Program Files (x86)\Microsoft SDKs\Project 2013\REDIST" `
    /fl /flp:"$loggerArgs" `
    /m:4 `
    $ToolsVersion `
	$DfMsBuildArgs `
 	$MsBuildArguments  
    if ($LastExitCode -ne 0) {
        throw "Project build failed with exit code: $LastExitCode."
    }
}

Log-Section "Building DLL Services Projects . . ."
foreach($projectToBeBuildAsDLL in $projectsToBeBuildAsDLL){
    
    $projectPath = Get-ChildItem -Path ($SourcesDirectory + "\*") -Include ($projectToBeBuildAsDLL + ".csproj") -Recurse

    Log-SubSection "Building '$projectToBeBuildAsDLL'..."
	Log-SubSection "projectPath: '$projectPath'...."
    
   & $MSBuildExec $projectPath `
   /t:build `
   /p:OutputPath="$BinariesDirectory" `
   /p:PreBuildEvent= `
   /p:PostBuildEvent= `
   /p:Configuration="$ConfigurationToBuild" `
   /p:Platform="$PlatformToBuild" `
   /p:langversion="$langversion" `
   /p:GenerateSerializationAssemblies="Off" `
   /p:ReferencePath="C:\Program Files (x86)\Microsoft SDKs\Project 2013\REDIST" `
    /fl /flp:"$loggerArgs" `
    /m:4 `
    $ToolsVersion `
	$DfMsBuildArgs `
 	$MsBuildArguments  
    if ($LastExitCode -ne 0) {
        throw "Project build failed with exit code: $LastExitCode."
    }
}

