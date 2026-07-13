using System.Text.Json.Serialization;

namespace EZCAClient.Models;

/// <summary>
/// Lightweight projection of a CA and its local worker certificates, returned by
/// <c>GET /api/CA/GetCALocalCertificates</c>. Property names mirror EZCA's
/// <c>CADetailInformationModel</c> so the server response deserializes directly.
/// </summary>
public class CADetailsResponse
{
    [JsonPropertyName("CAID")]
    public string? CAID { get; set; }

    [JsonPropertyName("CATier")]
    public string? CATier { get; set; }

    [JsonPropertyName("LocalCAs")]
    public List<LocalCACertModel> LocalCAs { get; set; } = new();
}

public class LocalCACertModel
{
    [JsonPropertyName("WorkerID")]
    public string WorkerID { get; set; } = string.Empty;

    [JsonPropertyName("CACertificate")]
    public string? CACertificate { get; set; }

    [JsonPropertyName("Status")]
    public string Status { get; set; } = string.Empty;
}
