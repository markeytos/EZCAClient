using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class AvailableCAModel
{
    public AvailableCAModel() { }

    public AvailableCAModel(DBSelfServiceScep scepProfile)
    {
        CAID = scepProfile.CaID;
        CAFriendlyName = scepProfile.CaID;
        TemplateID = scepProfile.TemplateID;
    }

    [JsonPropertyName("CAID")]
    public string? CAID { get; set; }

    [JsonPropertyName("CAFriendlyName")]
    public string? CAFriendlyName { get; set; }

    [JsonPropertyName("CATemplateType")]
    public string? CATemplateType { get; set; }

    [JsonPropertyName("TemplateID")]
    public string? TemplateID { get; set; }

    [JsonPropertyName("KeyUsage")]
    public string? KeyUsage { get; set; }

    [JsonPropertyName("MaxCertLifeDays")]
    public int MaxCertLifeDays { get; set; }

    [JsonPropertyName("DomainRestrictions")]
    public string? DomainRestrictions { get; set; } //this is if all domains are allowed or not

    [JsonPropertyName("CAKeyType")]
    public string? CAKeyType { get; set; }

    [JsonPropertyName("CAHashing")]
    public string? CAHashing { get; set; }
}
