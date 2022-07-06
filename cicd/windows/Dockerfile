FROM mcr.microsoft.com/dotnet/sdk:6.0.301-windowsservercore-ltsc2022
COPY Install-ChocolateyInContainer.ps1 C:/choco-nupkg/Install-ChocolateyInContainer.ps1
RUN PowerShell.exe "C:\choco-nupkg\Install-ChocolateyInContainer.ps1"
RUN choco install pandoc miktex -y
RUN ls "C:\Program Files\MiKTeX\miktex\bin\x64" && "C:\Program Files\MiKTeX\miktex\bin\x64" | Out-File -FilePath $env:PATH -Encoding utf8 -Append