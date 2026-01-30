using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class DBSelfServiceGuest
{
    [JsonPropertyName("TenantID")]
    public string TenantID { get; set; }

    [JsonPropertyName("CaTenantID")]
    public string CaTenantID { get; set; }

    [JsonPropertyName("CaID")]
    public string CaID { get; set; }

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; }

    [JsonPropertyName("ProfileID")]
    public string ProfileID { get; set; }

    [JsonPropertyName("allUsers")]
    public bool AllUsers { get; set; }

    [JsonPropertyName("PolicyName")]
    public string PolicyName { get; set; }
}
