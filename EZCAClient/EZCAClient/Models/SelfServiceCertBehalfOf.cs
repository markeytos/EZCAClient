using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class SelfServiceCertBehalfOf
{
    public SelfServiceCertBehalfOf() { }

    public SelfServiceCertBehalfOf(
        string csr,
        string certificateName,
        string caID,
        string caTemplateID,
        string caProfileID,
        string caPolicyName,
        string ezcaCertificatePEM,
        string userGUID
    )
    {
        CSR = csr;
        CertificateName = certificateName;
        CaID = caID;
        CATemplateID = caTemplateID;
        CAProfileID = caProfileID;
        CAPolicyName = caPolicyName;
        EZCACertificatePEM = ezcaCertificatePEM;
        UserGUID = userGUID;
    }

    [JsonPropertyName("CSR")]
    public string CSR { get; set; }

    [JsonPropertyName("CertificateName")]
    public string CertificateName { get; set; }

    [JsonPropertyName("UserGUID")]
    public string UserGUID { get; set; }

    [JsonPropertyName("CaID")]
    public string CaID { get; set; }

    [JsonPropertyName("TemplateID")]
    public string CATemplateID { get; set; }

    [JsonPropertyName("CAProfileID")]
    public string CAProfileID { get; set; }

    [JsonPropertyName("CAPolicyName")]
    public string CAPolicyName { get; set; }

    [JsonPropertyName("EZCACertificatePEM")]
    public string EZCACertificatePEM { get; set; }
}
