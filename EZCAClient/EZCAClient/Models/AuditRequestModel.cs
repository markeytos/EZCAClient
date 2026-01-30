using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class AuditRequestModel
{
    
    public AuditRequestModel()
    {
        DateFrom = DateTime.UtcNow.AddDays(-90);
        DateTo = DateTime.UtcNow.AddDays(1);
    }

    [JsonPropertyName("DateFrom")]
    public DateTime? DateFrom { get; set; }

    [JsonPropertyName("DateTo")]
    public DateTime? DateTo { get; set; }

    [JsonPropertyName("MaxNumberOfRecords")]
    public int MaxNumberOfRecords { get; set; } = 12000;

    [JsonPropertyName("PageNumber")]
    public int PageNumber { get; set; } = 0;
}