dotnet build -c release
dotnet nuget sign .\bin\Release\EZCAClient.1.0.7.nupkg --certificate-fingerprint 6B0113D9DA9EC8745854080A3BED8692FD290662AFDE0016F7A3D960AF87C9A1 --timestamper http://timestamp.digicert.com
dotnet nuget verify --all .\bin\Release\EZCAClient.1.0.7.nupkg