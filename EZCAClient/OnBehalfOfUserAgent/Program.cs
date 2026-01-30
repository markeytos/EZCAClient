using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using EZCAClient.Managers;
using EZCAClient.Models;
using EZCAClient.Services;
using OnBehalfOfUserAgent.Services;

//This sample code shows the workflow needed to act as behalf of agent to issue certificates on behalf of the users in the tenant
//Pre-requisites is to have a SCEP CA, Self Service User Profile, and to create the first certificate it must be run by a PKI Administrator
Console.WriteLine("Welcome to the EZCA On Behalf of Client");

//create a new EZCAClient, you can also pass an Azure token credential if you want to use specific credentials
//as well as an ILogger to log exceptions that are caught by our HTTP client
// By default the NuGet package uses https://portal.ezca.io/ as the base URL if you are using another instance
// (such as a private instance or our local offerings such as eu.ezca.io or au.ezca.io) you can pass the base URL
// as a parameter to the EZCAClientClass constructor.
// Example: EZCAClient ezcaClient = new EZCAClientClass(new HttpClient(), logger, "https://eu.ezca.io/");

//The Token credential is only needed if doing the admin bootstrap (first time), otherwise it is not needed since we don't have to call EZCA
TokenCredential credential = new InteractiveBrowserCredential();

IEZCAClient ezcaClient = new EZCAClientClass(new HttpClient(), azureTokenCredential: credential);

try
{
    // Now we are going to get the available CAs
    Console.WriteLine("Getting Available CAs..");
    AvailableSelfServiceModel? availableCAs =
        await ezcaClient.GetSelfServiceCertAvailableProfilesAsync();
    if (availableCAs == null)
    {
        Console.WriteLine("Could not find any available CAs in EZCA");
        return;
    }
    DBSelfServiceScep selectedProfile = InputService.SelectProfile(availableCAs);
    //You probably will want to save the selectedProfile information so you can call it to get the new Certificates as well as renew your bootstrap certificate

    //Get the OnBehalf of Certificate as an Administrator (Note this can only be done with a PKI administrator account)
    //create a 4096 RSA key
    using RSA key = RSA.Create(4096);
    //
    string subjectName = Guid.NewGuid().ToString();
    string profileFriendlyName = $"OnBehalfOf sample";
    X500DistinguishedName x500DistinguishedName = new("CN=" + subjectName);
    CertificateRequest certificateRequest = new(
        x500DistinguishedName,
        key,
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1
    );
    List<string> ekus = ["1.3.6.1.5.5.7.3.2"];
    string csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
    int certificateLifetime = 30;
    X509Certificate2? adminCertificate = await ezcaClient.RequestDCCertificateAsync(
        new(selectedProfile),
        csr,
        subjectName,
        subjectName,
        certificateLifetime,
        ekus
    );
    if (adminCertificate == null)
    {
        Console.WriteLine("Could not create admin certificate");
        return;
    }
    X509Certificate2 adminCertificatePrivateKey = adminCertificate.CopyWithPrivateKey(key); // this is the certificate we will use to authenticate with EZCA, save it in a safe place such an Azure Key Vault
    //Note this certificate will have permission to request a certificate on behalf of the users that have access to that self-service user policy.

    // Register the Certificate as an on behalf of agent  (Note this can only be done with a PKI administrator account)
    List<BehalfOfAgentsDetails> agents =
    [
        new() { ProfileID = subjectName, Name = profileFriendlyName },
    ];
    selectedProfile.BehalfOfAgents = JsonSerializer.Serialize(agents);
    APIResultModel response = await ezcaClient.RegisterOnBehalfOfSelfServiceAgentAsync(
        selectedProfile
    );
    if (response.Success)
    {
        Console.WriteLine("Registered OnBehalfOfSelfServiceAgent");
    }
    else
    {
        Console.WriteLine("Registered OnBehalfOfSelfServiceAgent failed: " + response.Message);
        return;
    }
    // This concludes the bootstrapping of the agent, which is a one time operation per CA.

    // Anything beyond this point will no longer need the PKI administrator token

    // create a certificate for a Sample User
    // create basic CSR for user (the values will be replaced by the profile so it doesn't have to match)
    using RSA userKey = RSA.Create(4096);
    x500DistinguishedName = new("CN=basicCSR");
    certificateRequest = new(
        x500DistinguishedName,
        userKey,
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1
    );
    csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
    // The User Entra ID  GUID
    string userGuid = "2c67a334-e08a-4c59-ae82-7afd8e034c31";

    string userCertificatePEM = await ezcaClient.RequestCertificateOnBehalfOfUserAsync(
        adminCertificatePrivateKey,
        csr,
        selectedProfile,
        userGuid
    );

    X509Certificate2 userCertificate = CryptoStaticService.ImportCertFromPEMString(
        userCertificatePEM
    );
    X509Certificate2 userCertificateWithPrivateKey = userCertificate.CopyWithPrivateKey(userKey);
    Console.WriteLine(
        "Created user certificate with subject name: "
            + userCertificateWithPrivateKey.SubjectName.Name
            + "with expiration date"
            + userCertificateWithPrivateKey.NotAfter
    );
    //Use the userCertificateWithPrivateKey certificate to authenticate as the user

    // Renew your Agent Certificate when it is about to expire
    key = RSA.Create(4096);
    x500DistinguishedName = new("CN=" + subjectName);
    certificateRequest = new(
        x500DistinguishedName,
        key,
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1
    );
    csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
    string newCert = await ezcaClient.RenewCertificateAsync(adminCertificatePrivateKey, csr);
    X509Certificate2 certificate = CryptoStaticService.ImportCertFromPEMString(newCert);
    X509Certificate2 certificateWithPrivateKey = certificate.CopyWithPrivateKey(key); //Save this new certificate as your new on behalf of certificate
    Console.WriteLine(
        "Renewed OnBehalfOfSelfServiceAgent certificate: "
            + certificateWithPrivateKey.SubjectName.Name
            + "with expiration date"
            + certificateWithPrivateKey.NotAfter
    );
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
