using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CERTENROLLLib;
using EZCAClient.Managers;
using EZCAClient.Models;
using EZCAClient.Services;
using SampleCode.Services;

Console.WriteLine("Welcome to the EZCAClient Sample");

//create a new EZCAClient, you can also pass an Azure token credential if you want to use specific credentials
//as well as an ILogger to log exceptions that are caught by our HTTP client
// By default the NuGet package uses https://portal.ezca.io/ as the base URL if you are using another instance
// (such as a private instance or our local offerings such as eu.ezca.io or au.ezca.io) you can pass the base URL
// as a parameter to the EZCAClientClass constructor.
// Example: EZCAClient ezcaClient = new EZCAClientClass(new HttpClient(), logger, "https://eu.ezca.io/");
IEZCAClient ezcaClient = new EZCAClientClass(new HttpClient());

try
{
    // Now we are going to get the available CAs
    Console.WriteLine("Getting Available CAs..");
    AvailableCAModel[]? availableCAs = await ezcaClient.GetAvailableCAsAsync();
    if (availableCAs == null || availableCAs.Any() == false)
    {
        Console.WriteLine("Could not find any available CAs in EZCA");
        return;
    }
    AvailableCAModel selectedCA = InputService.SelectCA(availableCAs);
    string domain = InputService.GetDomainName();

    //Now we will register the domain
    //This will use our logged in account, for production we recommend passing the list of Owners and Approvers for the domain.
    //NOTE: Owners Must be groups or human accounts.
    Console.WriteLine($"Registering domain");
    // these emails will get notifications about the domain and certificates
    // they are optional and can be null
    List<string> extraEmails = ["security@keytos.io"];
    APIResultModel registrationResult = await ezcaClient.RegisterDomainAsync(
        selectedCA,
        domain,
        null, //owners, leaving null so it uses current user.
        null, //certificate administrators (who can request and revoke certificates)
        //, leaving null so it uses current user.
        null, //requesters only, these accounts can only request certificates
        extraEmails
    );
    if (!registrationResult.Success)
    {
        Console.WriteLine($"Could not register new device in EZCA {registrationResult.Message}");
        return;
    }

    //Now that the domain has been registered and our account is an approved requester, we can request a certificate.

    //This first certificate request example we will let the EZClient library create the CSR (Certificate Signing Request)
    //for us.
    Console.WriteLine($"Requesting Certificate");
    X509Certificate2? firstCert = await ezcaClient.RequestCertificateAsync(selectedCA, domain, 20);

    //Now I will create a CSR using the Windows Store and use that csr to request a certificate
    //(NOTE: comment this code if running on Mac or Linux)
    List<string> subjectAltNames = new List<string> { domain };
    CX509CertificateRequestPkcs10 certRequest = WindowsCertStoreService.CreateCSR(
        "CN=" + domain,
        subjectAltNames,
        4096
    );
    string csr = certRequest.RawData[EncodingType.XCN_CRYPT_STRING_BASE64REQUESTHEADER];
    Console.WriteLine($"Getting Windows Certificate");
    X509Certificate2? windowsCert = await ezcaClient.RequestCertificateAsync(
        selectedCA,
        csr,
        domain,
        20
    );
    if (windowsCert != null)
    {
        Console.WriteLine($"Installing Windows Certificate");
        WindowsCertStoreService.InstallCertificate(
            CryptoStaticService.ExportToPEM(windowsCert),
            certRequest
        );
    }

    //Now we will use the first certificate to renew it
    if (firstCert != null)
    {
        //create new CSR
        //create a 4096 RSA key
        RSA key = RSA.Create(4096);

        //create Certificate Signing Request
        X500DistinguishedName x500DistinguishedName = new("CN=" + domain);
        CertificateRequest certificateRequest = new(
            x500DistinguishedName,
            key,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
        Console.WriteLine($"Renewing certificate");
        string newCert = await ezcaClient.RenewCertificateAsync(firstCert, csr);
        X509Certificate2 certificate = CryptoStaticService.ImportCertFromPEMString(newCert);
        X509Certificate2 certificateWithPrivateKey = certificate.CopyWithPrivateKey(key);
        Console.WriteLine("Finished renewing certificate");
    }
    //now we will use the same CSR from the windows Certificate to create a certificate that will return the full chain
    CertificateCreatedResponse? certificateCreatedResponse =
        await ezcaClient.RequestCertificateWithChainV2Async(
            selectedCA,
            csr,
            domain,
            [new(domain), new("1.1.1.1", 7)],
            20,
            "testLocation"
        );
    Console.WriteLine("Finished certificate creation sample :)");
    Console.WriteLine("Press any key to exit");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
