using Azure.Core;
using Azure.Identity;
using EZCAClient.Models;
using EZCAClient.Services;
using Jose;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
    /// <param name="tokenCredential">The <see cref="TokenCredential"/> you would like to use to authenticate.</param>
    /// <returns>An <see cref="AvailableCAModel"/> array.</returns>
    /// <exception cref="HttpRequestException">Error contacting server</exception>
    Task<AvailableCAModel[]?> GetAvailableCAsAsync(TokenCredential? tokenCredential);


}


public class EZCAClient : IEZCAClient
{
    private readonly ILogger? _logger;
    private readonly HttpClientService _httpClient;
    private readonly string _url;

    public EZCAClient(HttpClient httpClient, ILogger? logger = null,
        string baseURL = "https://portal.ezca.io/")
    {
        if (string.IsNullOrWhiteSpace(baseURL))
        {
            throw new ArgumentNullException(nameof(baseURL));
        }
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }
        _httpClient = new(httpClient, logger);
        _url = baseURL.TrimEnd('/');
        _logger = logger;
    }

    public async Task RevokeCertificateAsync(X509Certificate2 cert)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }
        TokenModel token = CreateRSAJWTToken(cert);
        APIResultModel result = await _httpClient.CallGenericAsync(
            _url + "/api/Certificates/RevokeCertificate", null, token.AccessToken,
            HttpMethod.Get);
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
        if(cert == null)
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
            JsonSerializer.Serialize(certReq), token.AccessToken,
            HttpMethod.Get);
        if (result.Success)
        {
            APIResultModel serverResponse =
                JsonSerializer.Deserialize<APIResultModel>(result.Message) ??
                new(false, "error deserializing response: " + result.Message);
            if (serverResponse.Success)
            {
                return serverResponse.Message;
            }
            else
            {
                throw new Exception(serverResponse.Message);
            }
        }
        else
        {
            throw new Exception(result.Message);
        }
    }

    public async Task<AvailableCAModel[]?> GetAvailableCAsAsync(TokenCredential? tokenCredential)
    {
        if (tokenCredential == null)
        {
            Console.WriteLine("No token credential provided, creating default credential");
            tokenCredential = new DefaultAzureCredential(includeInteractiveCredentials: true);
        }
        TokenRequestContext authContext = new(
                new string[] { "https://management.core.windows.net/.default" });
        AccessToken token = await tokenCredential.GetTokenAsync(authContext, default);

        AvailableCAModel[]? availableCAs = null;
        APIResultModel response = await
                _httpClient.CallGenericAsync($"{_url}/api/CA/GetAvailableSSLCAs", null, token.Token,
                    HttpMethod.Get);
        if (response.Success)
        {
            availableCAs = JsonSerializer.Deserialize
                <AvailableCAModel[]>(response.Message);
        }
        else
        {
            throw new HttpRequestException(response.Message);
        }
        return availableCAs;
    }

    private TokenModel CreateRSAJWTToken(X509Certificate2 clientCertificate)
    {
        if(clientCertificate.GetRSAPublicKey() == null)
        {
            throw new ArgumentNullException("Only RSA certificates are supported for certificate based authentication");
        }
        var headers = new Dictionary<string, object>
            {
                { "typ", "JWT" },
                { "x5t", clientCertificate.Thumbprint }
            };
        TokenModel token = new();
        var payload = new Dictionary<string, object>()
            {
                {"aud", $"https://ezca.io"},
                {"jti", Guid.NewGuid().ToString()},
                {"nbf", (ulong)token.NotBefore.ToUnixTimeSeconds()},
                {"exp", (ulong)token.ExpiresOn.ToUnixTimeSeconds()}
            };
        token.AccessToken = JWT.Encode(payload, clientCertificate.GetRSAPrivateKey(),
            JwsAlgorithm.RS256, extraHeaders: headers);
        return token;
    }
}