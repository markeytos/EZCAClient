using System.IdentityModel.Tokens.Jwt;
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
    /// <param name="sid">The SID for the domain controller</param>
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
        string dcGuid = "",
        string sid = ""
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

    /// <summary>
    /// Returns a AvailableSelfServiceModel with a list all the Self Service profiles that the authenticated user has access to
    /// </summary>
    /// <returns><see cref="AvailableSelfServiceModel"/>A model containing all the Profiles available in the subscriptions the user is a PKI Administrator</returns>
    Task<AvailableSelfServiceModel?> GetSelfServiceCertAvailableProfilesAsync();

    /// <summary>
    /// Registers a Subject Name as an on behalf of agent that can request certificates on behalf of users in EZCA
    /// </summary>
    /// <param name="scepAgent"></param>
    /// <returns><see cref="APIResultModel"/>Success True if it registered it correctly otherwise false with the Error the server returned</returns>
    Task<APIResultModel> RegisterOnBehalfOfSelfServiceAgentAsync(DBSelfServiceScep scepAgent);

    /// <summary>
    ///
    /// </summary>
    /// <param name="agentCertificate">The certificate for the on behalf of Agent</param>
    /// <param name="csr">the CSR created for the user</param>
    /// <param name="selfServiceProfile">The Self service profile you want to use</param>
    /// <param name="userGuid">the Entra ID user Guid that you want to use</param>
    /// <returns>PEM encoded Certificate for the user</returns>
    Task<string> RequestCertificateOnBehalfOfUserAsync(
        X509Certificate2 agentCertificate,
        string csr,
        DBSelfServiceScep selfServiceProfile,
        string userGuid
    );
    
    /// <summary>
    /// Gets the registered domains for the current user
    /// </summary>
    /// <returns><see cref="DomainInformationModel"/>List DomainInformationModel Containing all the domains</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<DomainInformationModel>> GetRegisteredDomainsAsync();
    
    /// <summary>
    /// Gets the registered certificates for the current user (It Calls GetMyCertificatesPaginatedAsync until it gets all the pages)
    /// </summary>
    /// <returns><see cref="SSLCertInfoV2"/>List SSLCertInfoV2 Containing all the certificates</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<SSLCertInfoV2>> GetMyCertificatesAsync();
    
    /// <summary>
    /// Gets the registered certificates for the current user
    /// </summary>
    /// <returns><see cref="SSLCertInfoV2"/>List SSLCertInfoV2 Containing all the certificates</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<SSLCertInfoV2>> GetMyCertificatesPaginatedAsync(int pageNumber);

    /// <summary>
    /// Gets the audit logs for the current user (if the user is an admin it will get all the logs)
    /// </summary>
    /// <param name="auditRequest">Audit Request with date range and page number</param>
    /// <returns><see cref="SSLCertAuditLogModel"/>List SSLCertAuditLogModel Containing all the certificates logs</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<List<SSLCertAuditLogModel>> GetCertificateAuditLogsAsync(AuditRequestModel auditRequest);
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
        CertificateRequest certificateRequest = new(
            x500DistinguishedName,
            key,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
        string csr = CryptoStaticService.PemEncodeSigningRequest(certificateRequest);
        List<string> subjectAlternateNames = new() { domain };
        CertificateCreateRequestModel request = new(
            ca,
            "CN=" + domain,
            subjectAlternateNames,
            csr,
            certificateValidityDays,
            location
        );
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
        CertificateCreateRequestModel request = new(
            ca,
            subjectName,
            subjectAlternateNames,
            csr,
            certificateValidityDays,
            location
        );
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
        CertificateCreateRequestV2Model request = new(
            ca,
            subjectName,
            subjectAlternateNames,
            csr,
            certificateValidityDays,
            location
        );
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

    public async Task<List<DomainInformationModel>> GetRegisteredDomainsAsync()
    {
        await GetTokenAsync();
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/GetMyDomains",
            string.Empty,
            _token.Token,
            HttpMethod.Get
        );
        if (response.Success)
        {
            response = JsonSerializer.Deserialize<APIResultModel>(
                response.Message
            ) ?? new(false, "Error reading server response");
        }
        if (response.Success)
        {
            List<DomainInformationModel> domains =
                JsonSerializer.Deserialize<List<DomainInformationModel>>(response.Message) ?? new();
            return domains;
        }
        throw new HttpRequestException(response.Message);
    }

    public async Task<APIResultModel> RegisterOnBehalfOfSelfServiceAgentAsync(
        DBSelfServiceScep scepAgent
    )
    {
        if (string.IsNullOrWhiteSpace(scepAgent.BehalfOfAgents))
        {
            throw new ArgumentNullException(nameof(scepAgent.BehalfOfAgents));
        }

        if (string.IsNullOrWhiteSpace(scepAgent.CaID))
        {
            throw new ArgumentNullException(nameof(scepAgent.CaID));
        }
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RegisterNewBehalfOfSelfServiceAgent",
            JsonSerializer.Serialize(scepAgent),
            _token.Token,
            HttpMethod.Post
        );
        return response;
    }

    public async Task<string> RequestCertificateOnBehalfOfUserAsync(
        X509Certificate2 agentCertificate,
        string csr,
        DBSelfServiceScep selfServiceProfile,
        string userGuid
    )
    {
        TokenModel token = CreateRSAJWTToken(agentCertificate);
        APIResultModel result = await _httpClient.CallGenericAsync(
            _url.TrimEnd('/') + "/api/Certificates/RequestSelfServiceCertificateOnBehalfOf",
            JsonSerializer.Serialize(
                new SelfServiceCertBehalfOf(
                    csr,
                    $"EZRADIUS-SelfService {agentCertificate.SubjectName.Name}",
                    selfServiceProfile.CaID,
                    selfServiceProfile.TemplateID,
                    selfServiceProfile.ProfileID,
                    selfServiceProfile.ProfileID,
                    agentCertificate.ExportCertificatePem(),
                    userGuid
                )
            ),
            token.AccessToken,
            HttpMethod.Post
        );
        if (!result.Success)
        {
            throw new HttpRequestException(result.Message);
        }
        return result.Message;
    }
    
    public async Task<List<SSLCertInfoV2>> GetMyCertificatesAsync()
    {
        List<SSLCertInfoV2> allCerts = new();
        int pageNumber = 0;
        while (true)
        {
            List<SSLCertInfoV2> certs = await GetMyCertificatesPaginatedAsync(pageNumber);
            if (certs.Count == 0)
            {
                break;
            }
            allCerts.AddRange(certs);
            pageNumber++;
        }
        return allCerts;
    }

    public async Task<List<SSLCertInfoV2>> GetMyCertificatesPaginatedAsync(int pageNumber)
    {
        if (pageNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        }
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/GetMyCertificatesV2Paginated?pageNumber="
                                                   + pageNumber,
            string.Empty,
            _token.Token,
            HttpMethod.Get
        );
        if (response.Success)
        {
            return JsonSerializer.Deserialize<List<SSLCertInfoV2>>(
                response.Message
            ) ?? new();
        }
        throw new HttpRequestException(response.Message);
    }
    
    public async Task<List<SSLCertAuditLogModel>> GetCertificateAuditLogsAsync(AuditRequestModel auditRequest)
    {
        ArgumentNullException.ThrowIfNull(auditRequest);
        if (auditRequest.DateFrom == null || auditRequest.DateTo == null)
        {
            throw new ArgumentException("DateFrom and DateTo cannot be null");
        }
        if (auditRequest.DateFrom > auditRequest.DateTo)
        {
            throw new ArgumentException("DateFrom cannot be greater than DateTo");
        }
        if (auditRequest.MaxNumberOfRecords < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(auditRequest.MaxNumberOfRecords));
        }
        if (auditRequest.PageNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(auditRequest.PageNumber));
        }
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/Audit/GetCertLogs",
            JsonSerializer.Serialize(auditRequest),
            _token.Token,
            HttpMethod.Post
        );
        if (response.Success)
        {
            return JsonSerializer.Deserialize<List<SSLCertAuditLogModel>>(
                response.Message
            ) ?? new();
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
        CertificateCreateRequestModel request = new(
            ca,
            subjectName,
            new(),
            csr,
            certificateValidityDays,
            EZCAConstants.IMPORTCSR
        );
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
        NewDomainRegistrationRequest request = new(
            ca,
            domain,
            owners,
            certificateAdministrators,
            requestersOnly,
            notificationEmails
        );
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
        string dcGuid = "",
        string sid = ""
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
        CertificateCreateRequestV2Model request = new(
            ca,
            subjectName,
            [new(dnsName)],
            csr,
            certificateValidityDays,
            EZCAConstants.DomainController,
            ekus,
            dcGuid,
            sid
        );
        APIResultModel response = await _httpClient.CallGenericAsync(
            $"{_url}/api/CA/RequestDCCertificateV2",
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
        TokenRequestContext authContext = new(
            new[] { "https://management.core.windows.net/.default" }
        );
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
            { "x5t", clientCertificate.Thumbprint },
        };
        TokenModel token = new();
        var payload = new Dictionary<string, object>()
        {
            { "aud", $"https://ezca.io" },
            { "jti", Guid.NewGuid().ToString() },
            { "nbf", (ulong)token.NotBefore.ToUnixTimeSeconds() },
            { "exp", (ulong)token.ExpiresOn.ToUnixTimeSeconds() },
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
