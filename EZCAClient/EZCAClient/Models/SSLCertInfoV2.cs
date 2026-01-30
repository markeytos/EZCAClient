using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class SSLCertInfoV2
{
    public SSLCertInfoV2() { }
    [JsonPropertyName("CertificatePEM")]
    public string CertificatePEM { get; set; } //cluster

    [JsonPropertyName("Thumbprint")]
    public string Thumbprint { get; set; } //cluster

    [JsonPropertyName("CAID")]
    public string CAID { get; set; }

    [JsonPropertyName("SubjectName")]
    public string SubjectName { get; set; }

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; }

    [JsonPropertyName("Revoked")]
    public bool Revoked { get; set; }

    [JsonPropertyName("SubjectAlternateNames")]
    public List<SubjectAltValue> SubjectAlternateNames { get; set; } = new();

    [JsonPropertyName("KeyUsages")]
    public List<string> KeyUsages { get; set; } = new();

    [JsonPropertyName("EKUs")]
    public List<string> EKUs { get; set; } = new();

    [JsonPropertyName("DateRequested")]
    public DateTime DateRequested { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("ExpiryDate")]
    public DateTime ExpiryDate { get; set; }

    [JsonPropertyName("CertLocationType")]
    public string CertLocationType { get; set; } = string.Empty; //local, AKV, Machine

    [JsonPropertyName("CertLocation")]
    public string CertLocation { get; set; } = string.Empty; //AKV URL

    [JsonPropertyName("CAFriendlyName")]
    public string CAFriendlyName { get; set; } = string.Empty;

    [JsonPropertyName("CertNameAtLocation")]
    public string CertNameAtLocation { get; set; } = string.Empty; //Name of Certificate in AKV

    [JsonPropertyName("AutoRenew")]
    public bool AutoRenew { get; set; }

    [JsonPropertyName("AutoRenewPercentage")]
    public int AutoRenewPercentage { get; set; }

    [JsonPropertyName("CertLengthDays")]
    public int CertLengthDays { get; set; }

    [JsonPropertyName("CertAppID")]
    public string CertAppID { get; set; } = string.Empty; //Azure Application ID

    [JsonPropertyName("CertificateTags")]
    public string CertificateTags { get; set; } = string.Empty; //Tags

    [JsonPropertyName("CanRevoke")]
    public bool CanRevoke { get; set; }
}