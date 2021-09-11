###### Clean Architecture script by anderson ######
###### uses net5.0 and net standard 2.1 #######
param ($appName="AndersonApp",$netVersion="net5.0",$netStandardVersion="netstandard2.1",$pathSeparator="\")


#### functions
function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value;
    Split-Path $Invocation.MyCommand.Path;
}
### global variables
$global:rootDirSlnBase = Get-ScriptDirectory 
$global:rootDirSln =  Join-Path -Path $global:rootDirSlnBase -ChildPath $appName
$global:rootDir = Join-Path -Path $global:rootDirSln -ChildPath "src"

$Global:corePath = Join-Path -Path $rootDir -ChildPath "core" #$rootDir+"core"
$Global:infraPath = Join-Path -Path $rootDir -ChildPath "infrastructure" # $rootDir+"infrastructure"
$Global:uiPath = Join-Path -Path $rootDir  -ChildPath "presentation"  #$rootDir+"presentation"

#### dotnet Error code captures


function create-solution() {
    Start-Process -FilePath "dotnet.exe" -ArgumentList "new sln" -Wait -NoNewWindow -WorkingDirectory $global:rootDirSln
    sleep 1
}

function create-directories() {
    New-Item -Path $Global:rootDir -ItemType Directory -Verbose -Force -ErrorAction Stop
    New-Item -Path $Global:corePath -ItemType Directory -Verbose -Force -ErrorAction Stop
    sleep 1
    New-Item -Path $Global:infraPath -ItemType Directory -Verbose -Force -ErrorAction Stop
    sleep 1
    New-Item -Path $Global:uiPath -ItemType Directory -Verbose -Force -ErrorAction Stop

}
# 
$global:applicationProjectPath = Join-Path -Path $Global:corePath -ChildPath "$appName.Application" 
$global:domainProjectPath = Join-Path -Path $Global:corePath -ChildPath "$appName.Domain"
function create-core-projects() {
    $ERROR_CODE_DOTNET = (Start-Process -FilePath "dotnet.exe" -ArgumentList "new classlib -f $netStandardVersion -o $domainProjectPath" -Wait -NoNewWindow).ExitTime
    Write-Host "Status Code: $ERROR_CODE_DOTNET" -ForegroundColor Yellow
    sleep 1
     $ERROR_CODE_DOTNET = (Start-Process -FilePath "dotnet.exe" -ArgumentList "new classlib -f $netStandardVersion -o $applicationProjectPath" -Wait -NoNewWindow).ExitCode
     Write-Host "Status Code: $ERROR_CODE_DOTNET" -ForegroundColor Yellow
     sleep 1
     # add reference from Domain to Application
     $global:ERROR_CODE_DOTNET = (Start-Process -FilePath "dotnet.exe" -ArgumentList "add reference ..\$appName.Domain\$appName.Domain.csproj" -Wait -NoNewWindow -WorkingDirectory $global:applicationProjectPath).ExitCode
      Write-Host "Status Code: $ERROR_CODE_DOTNET" -ForegroundColor Yellow
     sleep 1
      
        
}

$global:dataProjectPath = Join-Path -Path $Global:infraPath -ChildPath "$appName.Data" 
$global:sharedProjectPath = Join-Path -Path $Global:infraPath -ChildPath "$appName.Shared" 
function create-infra-projects() {
        Start-Process -FilePath "dotnet.exe" -ArgumentList "new classlib -f $netVersion -o $dataProjectPath" -Wait -NoNewWindow
        sleep 1
        Start-Process -FilePath "dotnet.exe" -ArgumentList "new classlib -f $netVersion -o $sharedProjectPath" -Wait -NoNewWindow

        # make Data project depend upon Domain and Application project
        $kwargs = "add reference  ..\..\core\$appName.Domain\$appName.Domain.csproj ..\..\core\$appName.Application\$appName.Application.csproj"

        Start-Process -FilePath "dotnet.exe" -ArgumentList $kwargs  -Wait -NoNewWindow -WorkingDirectory $global:dataProjectPath

        # make Shared depend on Application
        $kwargs1 = "add reference ..\..\core\$appName.Application\$appName.Application.csproj"

        ## pass kwargs1 not kwargs
        Start-Process -FilePath "dotnet.exe" -ArgumentList $kwargs1  -Wait -NoNewWindow -WorkingDirectory $global:sharedProjectPath
}

$global:webapiProjectPath = Join-Path -Path $Global:uiPath -ChildPath "$appName.WebApi" 
function create-presentation-projects() {
        Start-Process -FilePath "dotnet.exe" -ArgumentList "new webapi -f $netVersion -o $webapiProjectPath" -Wait -NoNewWindow
        sleep 1
        # make WebApi project depend upon: Application, Data, Shared
        $kwargs = "add reference ..\..\core\$appName.Application\$appName.Application.csproj ..\..\infrastructure\$appName.Data\$appName.Data.csproj ..\..\infrastructure\$appName.Shared\$appName.Shared.csproj"

        Start-Process -FilePath "dotnet.exe" -ArgumentList $kwargs  -Wait -NoNewWindow -WorkingDirectory $global:webapiProjectPath
           
}

function consolidate-projects() {
    $argsCoreProjects = "sln add .\src\core\$appName.Domain\$appName.Domain.csproj .\src\core\$appName.Application\$appName.Application.csproj"

    Start-Process -FilePath "dotnet.exe" -ArgumentList $argsCoreProjects -Wait -NoNewWindow -WorkingDirectory $global:rootDirSln

    $argsInfraProjects = "sln add .\src\infrastructure\$appName.Data\$appName.Data.csproj .\src\infrastructure\$appName.Shared\$appName.Shared.csproj" 

    Start-Process -FilePath "dotnet.exe" -ArgumentList $argsInfraProjects -Wait -NoNewWindow -WorkingDirectory $global:rootDirSln

    $argsUIProjects = "sln add .\src\presentation\$appName.WebApi\$appName.WebApi.csproj"

    Start-Process -FilePath "dotnet.exe" -ArgumentList $argsUIProjects -Wait -NoNewWindow -WorkingDirectory $global:rootDirSln

}
function print-tree() {
    tree $global:rootDirSln
}


function Main() {
create-directories
    create-solution
    create-core-projects
    create-infra-projects
    create-presentation-projects
    consolidate-projects
    Write-Host "Rejoice, your project: ($appName) creation completed successfully and no errors!!!!" -ForegroundColor Green
    sleep 1
    Write-Host " ########## The project Will Have The Following Projects ########" -ForegroundColor DarkCyan
    print-tree
    pause
}
Main