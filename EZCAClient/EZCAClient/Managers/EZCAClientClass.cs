﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using EZCAClient.Models;
using EZCAClient.Services;
using Jose;
using Microsoft.Extensions.Logging;

namespace EZCAClient.Managers;

public interface IEZCAClient
{
    /// <summary>
    /// Renew Certificate in EZCA
    /// </summary>
    /// <param name="cert">Certificate to Renew </param>
    /// <param name="csr">CSR of new Certificate </param>
    /// <returns>base64 Certificate</returns>
    /// <exception cref="ApplicationException">Error renewing certificate</exception>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<string> RenewCertificateAsync(X509Certificate2 cert, string csr);

    /// <summary>
    /// Revoke Certificate in EZCA
    /// </summary>
    /// <param name="cert">Certificate to Revoke </param>
    /// <exception cref="ApplicationException">Error revoking certificate</exception>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task RevokeCertificateAsync(X509Certificate2 cert);

    /// <summary>
    /// Gets the available CAs from EZCA
    /// </summary>
    /// <returns>An <see cref="AvailableCAModel"/> array.</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<AvailableCAModel[]?> GetAvailableCAsAsync();

    /// <summary>
    /// Creates a new SSL certificate given a domain.
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="domain">the domain for the new certificate</param>
    /// <param name="certificateValidityDays">the duration in days for the new certificate</param>
    /// <param name="location">Text field for where is this certificate is being stored</param>
    /// <returns>The created <see cref="X509Certificate2"/>.</returns>
    /// <exception cref="ApplicationException">Error creating certificate</exception>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<X509Certificate2?> RequestCertificateAsync(
        AvailableCAModel ca,
        string domain,
        int certificateValidityDays,
        string location = "Generated Locally"
    );

    /// <summary>
    /// Creates a new SSL certificate given a valid csr.
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="subjectName">The certificate's subject name</param>
    /// <param name="certificateValidityDays">the duration in days for the new certificate</param>
    /// <param name="csr">the created CSR</param>
    /// <returns>The created <see cref="X509Certificate2"/>.</returns>
    /// <exception cref="ApplicationException">Error creating certificate</exception>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<X509Certificate2?> RequestCertificateAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        int certificateValidityDays
    );

    /// <summary>
    /// Creates a new Domain Controller certificate given a valid CSR (Only Available for SCEP templates).
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="subjectName">The certificate's subject name</param>
    /// <param name="dnsName">The domain controller's DNS name</param>
    /// <param name="certificateValidityDays">the duration in days for the new certificate</param>
    /// <param name="csr">the created CSR</param>
    /// <param name="dcGuid">The domain controller's GUID (only required if SMTP replication is used)</param>
    /// <param name="ekus">The EKUs requested for the Certificate</param>
    /// <returns>The created <see cref="X509Certificate2"/>.</returns>
    /// <exception cref="ApplicationException">Error creating certificate</exception>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<X509Certificate2?> RequestDCCertificateAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        string dnsName,
        int certificateValidityDays,
        List<string> ekus,
        string dcGuid = ""
    );

    /// <summary>
    /// Registers a new domain in EZCA with its appropriate owners
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="domain">Domain name you want to register in EZCA</param>
    /// <param name="owners">Approved Owners for requesting certificate (If left empty current user will be used)</param>
    /// <param name="certificateAdministrators">Approved requesters for requesting certificate (If left empty current user will be used)</param>
    /// <param name="requestersOnly">Approved certificate managers (they can issue or revoke certificates)</param>
    /// <param name="notificationEmails">Extra email list that will get domain and certificate related alerts</param>
    /// <returns><see cref="APIResultModel"/> Indicating if the operation was successful or not.</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<APIResultModel> RegisterDomainAsync(
        AvailableCAModel ca,
        string domain,
        List<AADObjectModel>? owners = null,
        List<AADObjectModel>? certificateAdministrators = null,
        List<AADObjectModel>? requestersOnly = null,
        List<string>? notificationEmails = null
    );

    /// <summary>
    /// Creates a certificate from a CSR and returns the full chain of the certificate
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="csr">Certificate Signing Request for the certificate</param>
    /// <param name="subjectName">certificate subject name</param>
    /// <param name="subjectAlternateNames">list of subject alternate names</param>
    /// <param name="certificateValidityDays">number of days that the certificate is valid for</param>
    /// <param name="location">Text field for where is this certificate is being stored</param>
    /// <returns><see cref="CertificateCreatedResponse"/> Containing the PEM strings of the CA chain as well as the issued certificate</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<CertificateCreatedResponse?> RequestCertificateWithChainAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        List<string> subjectAlternateNames,
        int certificateValidityDays,
        string location = "Generate Locally"
    );

    /// <summary>
    /// Creates a certificate from a CSR and returns the full chain of the certificate
    /// </summary>
    /// <param name="ca">Issuing CA</param>
    /// <param name="csr">Certificate Signing Request for the certificate</param>
    /// <param name="subjectName">certificate subject name</param>
    /// <param name="subjectAlternateNames">list of subject alternate names for subject alt type use 2 for DNS 7 for IP address</param>
    /// <param name="certificateValidityDays">number of days that the certificate is valid for</param>
    /// <param name="location">Text field for where is this certificate is being stored</param>
    /// <returns><see cref="CertificateCreatedResponse"/> Containing the PEM strings of the CA chain as well as the issued certificate</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<CertificateCreatedResponse?> RequestCertificateWithChainV2Async(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        List<SubjectAltValue> subjectAlternateNames,
        int certificateValidityDays,
        string location = "Generate Locally"
    );
}

public class EZCAClientClass : IEZCAClient
{
    private readonly HttpClientService _httpClient;
    private readonly string _url;
    private AccessToken _token;
    private readonly TokenCredential? _azureTokenCredential;

    public EZCAClientClass(
        HttpClient httpClient,
        ILogger? logger = null,
        string baseUrl = "https://portal.ezca.io/",
        TokenCredential? azureTokenCredential = null
    )
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        if (azureTokenCredential == null)
        {
            _azureTokenCredential = new DefaultAzureCredential();
        }
        else
        {
            _azureTokenCredential = azureTokenCredential;
        }
        _httpClient = new(httpClient, logger);
        _url = baseUrl.TrimEnd('/').Replace("http://", "https://");
    }

    public async Task RevokeCertificateAsync(X509Certificate2 cert)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }
        TokenModel token = CreateRSAJWTToken(cert);
        APIResultModel result = await _httpClient.CallGenericAsync(
            _url + "/api/Certificates/RevokeCertificate",
            null,
            token.AccessToken,
            HttpMethod.Get
        );
        if (result.Success)
        {
            APIResultModel serverResponse =
                JsonSerializer.Deserialize<APIResultModel>(result.Message)
                ?? new(false, "error deserializing response: " + result.Message);
            if (serverResponse.Success)
            {
                Console.WriteLine(serverResponse.Message);
            }
            else
            {
                throw new ApplicationException(serverResponse.Message);
            }
        }
        else
        {
            throw new HttpRequestException(result.Message);
        }
    }

    public async Task<string> RenewCertificateAsync(X509Certificate2 cert, string csr)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }
        if (string.IsNullOrWhiteSpace(csr))
        {
            throw new ArgumentNullException(nameof(csr));
        }
        CertRenewReqModel certReq = new(csr, (cert.NotAfter - cert.NotBefore).Days);
        TokenModel token = CreateRSAJWTToken(cert);
        APIResultModel result = await _httpClient.CallGenericAsync(
            _url + "/api/Certificates/RenewCertificate",
            JsonSerializer.Serialize(certReq),
            token.AccessToken,
            HttpMethod.Post
        );
        if (result.Success)
        {
            APIResultModel serverResponse =
                JsonSerializer.Deserialize<APIResultModel>(result.Message)
                ?? new(false, "error deserializing response: " + result.Message);
            if (serverResponse.Success)
            {
                return serverResponse.Message;
            }
            throw new Exception(serverResponse.Message);
        }
        throw new Exception(result.Message);
    }

    public async Task<AvailableCAModel[]?> GetAvailableCAsAsync()
    {
        await GetTokenAsync();
        AvailableCAModel[]? availableCAs;
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/GetAvailableSSLCAs",
            null,
            _token.Token,
            HttpMethod.Get
        );
        if (response.Success)
        {
            availableCAs = JsonSerializer.Deserialize<AvailableCAModel[]>(response.Message);
        }
        else
        {
            throw new HttpRequestException(response.Message);
        }
        return availableCAs;
    }

    public async Task<X509Certificate2?> RequestCertificateAsync(
        AvailableCAModel ca,
        string domain,
        int certificateValidityDays,
        string location = "Generate Locally"
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentNullException(nameof(domain));
        }
        await GetTokenAsync();
        //create a 4096 RSA key
        RSA key = RSA.Create(4096);

        //create Certificate Signing Request
        X500DistinguishedName x500DistinguishedName = new("CN=" + domain);
        CertificateRequest certificateRequest =
            new(x500DistinguishedName, key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        string csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
        List<string> subjectAlternateNames = new() { domain };
        CertificateCreateRequestModel request =
            new(ca, "CN=" + domain, subjectAlternateNames, csr, certificateValidityDays, location);
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestSSLCertificate",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            APIResultModel result =
                JsonSerializer.Deserialize<APIResultModel>(response.Message)
                ?? new(false, "Error reading server response");
            if (result.Success)
            {
                X509Certificate2 certificate = CryptoStaticService.ImportCertFromPEMString(
                    result.Message
                );
                return certificate.CopyWithPrivateKey(key);
            }
            throw new ApplicationException(result.Message);
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<CertificateCreatedResponse?> RequestCertificateWithChainAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        List<string> subjectAlternateNames,
        int certificateValidityDays,
        string location = "Generate Locally"
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(csr))
        {
            throw new ArgumentNullException(nameof(csr));
        }
        await GetTokenAsync();
        CertificateCreateRequestModel request =
            new(ca, subjectName, subjectAlternateNames, csr, certificateValidityDays, location);
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestFullSSLCertificate",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            return JsonSerializer.Deserialize<CertificateCreatedResponse>(response.Message);
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<CertificateCreatedResponse?> RequestCertificateWithChainV2Async(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        List<SubjectAltValue> subjectAlternateNames,
        int certificateValidityDays,
        string location = "Generate Locally"
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(csr))
        {
            throw new ArgumentNullException(nameof(csr));
        }
        await GetTokenAsync();
        CertificateCreateRequestV2Model request =
            new(ca, subjectName, subjectAlternateNames, csr, certificateValidityDays, location);
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestSSLCertificateV2",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            return JsonSerializer.Deserialize<CertificateCreatedResponse>(response.Message);
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<X509Certificate2?> RequestCertificateAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        int certificateValidityDays
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(csr))
        {
            throw new ArgumentNullException(nameof(csr));
        }
        if (string.IsNullOrWhiteSpace(subjectName))
        {
            throw new ArgumentNullException(nameof(subjectName));
        }

        if (!subjectName.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
        {
            subjectName = "CN=" + subjectName;
        }
        await GetTokenAsync();
        CertificateCreateRequestModel request =
            new(ca, subjectName, new(), csr, certificateValidityDays, EZCAConstants.IMPORTCSR);
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestSSLCertificate",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            APIResultModel result =
                JsonSerializer.Deserialize<APIResultModel>(response.Message)
                ?? new(false, "Error reading server response");
            if (result.Success)
            {
                X509Certificate2 certificate = CryptoStaticService.ImportCertFromPEMString(
                    result.Message
                );
                return certificate;
            }
            throw new ApplicationException(result.Message);
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<APIResultModel> RegisterDomainAsync(
        AvailableCAModel ca,
        string domain,
        List<AADObjectModel>? owners = null,
        List<AADObjectModel>? certificateAdministrators = null,
        List<AADObjectModel>? requestersOnly = null,
        List<string>? notificationEmails = null
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentNullException(nameof(domain));
        }
        await GetTokenAsync();
        if (owners == null || certificateAdministrators == null)
        {
            //get requester information
            AADObjectModel requester = await UserFromTokenAsync();
            owners ??= new() { requester };
            certificateAdministrators ??= new() { requester };
        }
        NewDomainRegistrationRequest request =
            new(ca, domain, owners, certificateAdministrators, requestersOnly, notificationEmails);
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RegisterNewDomain",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            APIResultModel result =
                JsonSerializer.Deserialize<APIResultModel>(response.Message)
                ?? new(false, "Error reading server response");
            return result;
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<X509Certificate2?> RequestDCCertificateAsync(
        AvailableCAModel ca,
        string csr,
        string subjectName,
        string dnsName,
        int certificateValidityDays,
        List<string> ekus,
        string dcGuid = ""
    )
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(csr))
        {
            throw new ArgumentNullException(nameof(csr));
        }
        if (string.IsNullOrWhiteSpace(subjectName))
        {
            throw new ArgumentNullException(nameof(subjectName));
        }
        if (string.IsNullOrWhiteSpace(dnsName))
        {
            throw new ArgumentNullException(nameof(dnsName));
        }
        if (certificateValidityDays < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(certificateValidityDays));
        }

        if (ekus is null || !ekus.Any())
        {
            throw new ArgumentNullException(nameof(ekus));
        }

        if (
            !subjectName.StartsWith("CN=", StringComparison.OrdinalIgnoreCase)
            && !subjectName.StartsWith("CN =", StringComparison.OrdinalIgnoreCase)
        )
        {
            subjectName = "CN=" + subjectName;
        }
        await GetTokenAsync();
        CertificateCreateRequestModel request =
            new(
                ca,
                subjectName,
                [dnsName],
                csr,
                certificateValidityDays,
                EZCAConstants.DomainController,
                ekus,
                dcGuid
            );
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestDCCertificate",
            JsonSerializer.Serialize(request),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            APIResultModel result =
                JsonSerializer.Deserialize<APIResultModel>(response.Message)
                ?? new(false, "Error reading server response");
            if (result.Success)
            {
                X509Certificate2 certificate = CryptoStaticService.ImportCertFromPEMString(
                    result.Message
                );
                return certificate;
            }
            throw new ApplicationException(result.Message);
        }
        throw new HttpRequestException(response.Message);
    }

    private async Task<AADObjectModel> UserFromTokenAsync()
    {
        await GetTokenAsync();
        JwtSecurityTokenHandler handler = new();
        var jsonToken = handler.ReadToken(_token.Token);
        JwtSecurityToken tokenS = (JwtSecurityToken)jsonToken;
        string objectId = (string)tokenS.Payload.FirstOrDefault(i => i.Key == "oid").Value;
        string upn = (string)tokenS.Payload.FirstOrDefault(i => i.Key == "upn").Value;
        return new(objectId, upn);
    }

    private async Task GetTokenAsync()
    {
        TokenRequestContext authContext =
            new(new[] { "https://management.core.windows.net/.default" });
        if (_azureTokenCredential == null)
        {
            throw new ArgumentNullException(nameof(_azureTokenCredential));
        }
        _token = await _azureTokenCredential.GetTokenAsync(authContext, default);
        if (string.IsNullOrWhiteSpace(_token.Token))
        {
            throw new AuthenticationFailedException("Error getting token");
        }
    }

    private static TokenModel CreateRSAJWTToken(X509Certificate2 clientCertificate)
    {
        if (clientCertificate.GetRSAPublicKey() == null)
        {
            throw new ArgumentException(
                "Only RSA certificates are supported for certificate based authentication"
            );
        }
        var headers = new Dictionary<string, object>
        {
            { "typ", "JWT" },
            { "x5t", clientCertificate.Thumbprint }
        };
        TokenModel token = new();
        var payload = new Dictionary<string, object>()
        {
            { "aud", $"https://ezca.io" },
            { "jti", Guid.NewGuid().ToString() },
            { "nbf", (ulong)token.NotBefore.ToUnixTimeSeconds() },
            { "exp", (ulong)token.ExpiresOn.ToUnixTimeSeconds() }
        };
        token.AccessToken = JWT.Encode(
            payload,
            clientCertificate.GetRSAPrivateKey(),
            JwsAlgorithm.RS256,
            extraHeaders: headers
        );
        return token;
    }
}
