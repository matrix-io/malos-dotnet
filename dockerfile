FROM microsoft/windowsservercore:1709

RUN @"%SystemRoot%/System32/WindowsPowerShell/v1.0/powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%/chocolatey/bin"
    
#****************************************************** VISUAL STUDIO TOOLS ****************************************************** 
# Download the Build Tools bootstrapper.
ADD https://aka.ms/vs/15/release/vs_buildtools.exe C:/TEMP/vs_buildtools.exe

# Install Build Tools excluding workloads and components with known issues.
RUN C:/TEMP/vs_buildtools.exe --quiet --wait --norestart --nocache \
    --installPath C:/BuildTools \
    --add Microsoft.VisualStudio.Workload.ManagedDesktopBuildTools \
    --add Microsoft.VisualStudio.Workload.MSBuildTools \
    --add Microsoft.VisualStudio.Workload.VCTools \
    --add Microsoft.VisualStudio.Workload.UniversalBuildTools \
    --add Microsoft.VisualStudio.Component.VC.140 \
    --add Microsoft.VisualStudio.Component.VC.CLI.Support \
    --add Microsoft.Net.Component.4.5.2.TargetingPack \
    --add Microsoft.Net.Component.4.TargetingPack \
    --add Microsoft.Net.Component.4.5.TargetingPack \
    --add Microsoft.Net.Component.4.6.TargetingPack \
    --add Microsoft.VisualStudio.Component.Windows81SDK \
    --add Microsoft.VisualStudio.Workload.NetCoreBuildTools \
    --add Microsoft.Net.Core.Component.SDK.1x \
    --add Microsoft.Component.VC.Runtime.UCRTSDK \
 || IF "%ERRORLEVEL%"=="3010" EXIT 0

#****************************************************** NUGET ****************************************************** 
# Download nuget.exe
ADD https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe C:/bin/nuget.exe

# Start developer command prompt with any other commands specified.
ENTRYPOINT C:\BuildTools\Common7\Tools\VsDevCmd.bat &&

SHELL ["powershell.exe", "-ExecutionPolicy", "Bypass", "-Command"]

RUN $env:PATH = 'C:/bin;' + $env:PATH; \
[Environment]::SetEnvironmentVariable('PATH', $env:PATH, [EnvironmentVariableTarget]::Machine)