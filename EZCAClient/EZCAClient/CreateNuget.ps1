dotnet build -c Release
$cert = Get-ChildItem Cert:\CurrentUser\My |
  Where-Object { $_.Thumbprint -eq "FF359F7EB11C96F7D182BA642800E0625E3AFBEB" } |
  Select-Object Subject, Thumbprint, @{n="SHA256";e={$_.GetCertHashString("SHA256")}}
dotnet nuget sign .\bin\Release\EZCAClient.1.0.7.nupkg --certificate-fingerprint $cert.SHA256 --timestamper http://timestamp.digicert.com
dotnet nuget verify --all .\bin\Release\EZCAClient.1.0.7.nupkg