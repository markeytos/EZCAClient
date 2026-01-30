using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class SSLCertAuditLogModel
{
    public SSLCertAuditLogModel() { }
    [JsonPropertyName("TenantID")]
    public string TenantID { get; set; } //cluster

    [JsonPropertyName("CAID")]
    public string CAID { get; set; } //cluster

    [JsonPropertyName("DateRequested")]
    public DateTimeOffset DateRequested { get; set; } = DateTime.UtcNow; //cluster

    [JsonPropertyName("Domain")]
    public string Domain { get; set; } = string.Empty; //cluster

    [JsonPropertyName("DomainID")]
    public string DomainID { get; set; } = string.Empty;

    [JsonPropertyName("Thumbprint")]
    public string Thumbprint { get; set; } = string.Empty; //cluster

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; } = string.Empty;

    [JsonPropertyName("CertificatePEM")]
    public string CertificatePEM { get; set; } = string.Empty;

    [JsonPropertyName("SubID")]
    public string SubID { get; set; } = string.Empty;

    [JsonPropertyName("Requester")]
    public string Requester { get; set; } = string.Empty;

    [JsonPropertyName("CAFriendlyName")]
    public string CAFriendlyName { get; set; } = string.Empty;

    [JsonPropertyName("CertLocationType")]
    public string CertLocationType { get; set; } = string.Empty; //local, AKV, Machine

    [JsonPropertyName("CertLocation")]
    public string CertLocation { get; set; } = string.Empty; //AKV URL

    [JsonPropertyName("CertNameAtLocation")]
    public string CertNameAtLocation { get; set; } = string.Empty; //Name of Certificate in AKV

    [JsonPropertyName("Action")]
    public string Action { get; set; } = string.Empty;
}