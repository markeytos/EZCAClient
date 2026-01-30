using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZCAClient.Models;

public class CertificateCreateRequestModel
{
    public CertificateCreateRequestModel(
        AvailableCAModel ca,
        string subjectName,
        List<string> subjectAltNames,
        string csr,
        int lifetime,
        string location
    )
    {
        SubjectName = subjectName;
        SubjectAltNames = subjectAltNames;
        CAID = ca.CAID;
        TemplateID = ca.TemplateID;
        CSR = csr;
        ValidityInDays = lifetime;
        SelectedLocation = location;
    }

    [JsonPropertyName("SubjectName")]
    public string SubjectName { get; set; } = string.Empty;

    [JsonPropertyName("SubjectAltNames")]
    public List<string> SubjectAltNames { get; set; } = new();

    [JsonPropertyName("CAID")]
    public string? CAID { get; set; }

    [JsonPropertyName("TemplateID")]
    public string? TemplateID { get; set; }

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
    public string SelectedLocation { get; set; } = "IoT Device"; // local, AKV, KMS, ETC

    [JsonPropertyName("ResourceID")]
    public string ResourceID { get; set; } = string.Empty;

    [JsonPropertyName("SecretName")]
    public string SecretName { get; set; } = string.Empty;

    [JsonPropertyName("AKVName")]
    public string AKVName { get; set; } = string.Empty;

    [JsonPropertyName("AutoRenew")]
    public bool AutoRenew { get; set; } = false;

    [JsonPropertyName("AutoRenewPercentage")]
    public int AutoRenewPercentage { get; set; } = 80;

    [JsonPropertyName("CertAppID")]
    public string CertAppID { get; set; } = string.Empty; //Azure Application ID In SCEP DC Cert Used for GUID Subject Alternative Name

    [JsonPropertyName("CertificateTags")]
    public string CertificateTags { get; set; } = string.Empty; //Tags
}
