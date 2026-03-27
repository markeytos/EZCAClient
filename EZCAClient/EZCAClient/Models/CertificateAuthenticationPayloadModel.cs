using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class CertificateAuthenticationPayloadModel<T>
{
    public CertificateAuthenticationPayloadModel()
    {
        Certificate = string.Empty;
    }

    public CertificateAuthenticationPayloadModel(string certificate, T payload)
    {
        Certificate = certificate;
        Payload = payload;
    }

    public CertificateAuthenticationPayloadModel(X509Certificate2 certificate, T payload)
    {
        Certificate = certificate.ExportCertificatePem();
        Payload = payload;
    }

    [JsonPropertyName("Certificate")]
    public string Certificate { get; set; }

    [JsonPropertyName("Payload")]
    public T? Payload { get; set; }
}