using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class CertificateCreateRequestV2Model
{
    public CertificateCreateRequestV2Model() { }

    public CertificateCreateRequestV2Model(
        AvailableCAModel ca,
        string subjectName,
        List<SubjectAltValue> subjectAltNames,
        string csr,
        int lifetime,
        string location
    )
    {
        SubjectName = subjectName;
        SubjectAltNames = subjectAltNames;
        CAID = ca.CAID!;
        TemplateID = ca.TemplateID!;
        CSR = csr;
        ValidityInDays = lifetime;
        SelectedLocation = location;
    }

    public CertificateCreateRequestV2Model(
        AvailableCAModel ca,
        string subjectName,
        List<SubjectAltValue> subjectAltNames,
        string csr,
        int lifetime,
        string location,
        List<string> ekus,
        string certAppID,
        string sid
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ca.CAID);
        ArgumentException.ThrowIfNullOrWhiteSpace(ca.TemplateID);
        ArgumentException.ThrowIfNullOrWhiteSpace(subjectName);
        SubjectName = subjectName;
        SubjectAltNames = subjectAltNames;
        CAID = ca.CAID;
        TemplateID = ca.TemplateID;
        CSR = csr;
        ValidityInDays = lifetime;
        SelectedLocation = location;
        EKUs = ekus.ToArray();
        CertAppID = certAppID;
        Sid = sid;
    }

    [JsonPropertyName("SubjectName")]
    public string SubjectName { get; set; } = string.Empty;

    [JsonPropertyName("SubjectAltNames")]
    public List<SubjectAltValue> SubjectAltNames { get; set; } = new();

    [JsonPropertyName("CAID")]
    public string CAID { get; set; } = string.Empty;

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; } = string.Empty;

    [JsonPropertyName("CSR")]
    public string CSR { get; set; } = string.Empty;

    [JsonPropertyName("ValidityInDays")]
    public int ValidityInDays { get; set; }

    [JsonPropertyName("EKUs")]
    public string[] EKUs { get; set; } =
    [EZCAConstants.ClientAuthenticationEKU, EZCAConstants.ServerAuthenticationEKU];

    [JsonPropertyName("KeyUsages")]
    public string[] KeyUsages { get; set; } = ["Key Encipherment", "Digital Signature"];

    //CertLocation
    [JsonPropertyName("SelectedLocation")]
    public string SelectedLocation { get; set; } = "Generate Locally"; // local, AKV, KMS, ETC

    [JsonPropertyName("ResourceID")]
    public string ResourceID { get; set; } = string.Empty;

    [JsonPropertyName("SecretName")]
    public string SecretName { get; set; } = string.Empty;

    [JsonPropertyName("AKVName")]
    public string AKVName { get; set; } = string.Empty;

    [JsonPropertyName("AutoRenew")]
    public bool AutoRenew { get; set; }

    [JsonPropertyName("AutoRenewPercentage")]
    public int AutoRenewPercentage { get; set; } = 80;

    [JsonPropertyName("CertAppID")]
    public string CertAppID { get; set; } = string.Empty; //Azure Application ID In SCEP DC Cert Used for GUID Subject Alternative Name

    [JsonPropertyName("CertificateTags")]
    public string CertificateTags { get; set; } = string.Empty; //Tags

    [JsonPropertyName("SID")]
    public string Sid { get; set; } = string.Empty;
}
