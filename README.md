## About EZCA
[EZCA is a cloud based Certificate Authority](https://www.keytos.io/AZURE-PKI.html) that allows organizations to manage their internal certificates either with an existing [ADCS CA](https://blog.keytos.io/2022/03/31/ADCS.html) or with a cloud based CA managed by EZCA. While EZCA offers multiple automated certificate issuance tools such as [Private CA ACME support](https://blog.keytos.io/2022/12/01/ACMECA.html), sometimes a custom tool is needed. This repo hosts the code for the EZCAClient Nuget package that allows organizations to write client side automation for EZCA an Azure based Certificate Authority.

## Getting Started
Fist add the NuGet package to your project by running 
```
dotnet add package EZCAClient --version 1.0.0
```
Then use the sample project in this repository to learn how you use each of the functions. 