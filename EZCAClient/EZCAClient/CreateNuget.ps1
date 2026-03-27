dotnet build -c Release
$cert = Get-ChildItem Cert:\CurrentUser\My |
  Where-Object { $_.Thumbprint -eq "FF359F7EB11C96F7D182BA642800E0625E3AFBEB" } |
  Select-Object Subject, Thumbprint, @{n="SHA256";e={$_.GetCertHashString("SHA256")}}
$package = Get-ChildItem .\bin\Release\EZCAClient*.nupkg | Sort-Object LastWriteTime -Descending | Select-Object -First 1
dotnet nuget sign $package.FullName --certificate-fingerprint $cert.SHA256 --timestamper http://timestamp.digicert.com
dotnet nuget verify --all $package.FullName