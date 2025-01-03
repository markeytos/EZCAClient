dotnet build -c release
dotnet nuget sign .\bin\Release\EZCAClient.1.0.6.nupkg --certificate-fingerprint fee79f6d8eceb66618e845e82bcb56270accdeed2aeb725d56049ac42efb3d87 --timestamper http://timestamp.digicert.com
dotnet nuget verify --all .\bin\Release\EZCAClient.1.0.6.nupkg