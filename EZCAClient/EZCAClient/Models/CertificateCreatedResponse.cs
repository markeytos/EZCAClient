using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZCAClient.Models;

public class CertificateCreatedResponse
{
    public CertificateCreatedResponse() { }

    public CertificateCreatedResponse(
        string certificatePEM,
        string issuingCACertificate,
        string rootCertificate
    )
    {
        CertificatePEM = certificatePEM;
        IssuingCACertificate = issuingCACertificate;
        RootCertificate = rootCertificate;
    }

    public CertificateCreatedResponse(string certificatePEM)
    {
        CertificatePEM = certificatePEM;
    }

    [JsonPropertyName("CertificatePEM")]
    public string CertificatePEM { get; set; } = string.Empty;

    [JsonPropertyName("IssuingCACertificate")]
    public string IssuingCACertificate { get; set; } = string.Empty;

    [JsonPropertyName("RootCertificate")]
    public string RootCertificate { get; set; } = string.Empty;
}
