using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class DBSelfServiceScep
{
    [JsonPropertyName("TenantID")]
    public string TenantID { get; set; } // primary key

    [JsonPropertyName("CaID")]
    public string CaID { get; set; } // cluster key

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; } // cluster key

    [JsonPropertyName("ProfileID")]
    public string ProfileID { get; set; } // cluster key

    [JsonPropertyName("allUsers")]
    public bool AllUsers { get; set; } = true;

    [JsonPropertyName("MultipleDevices")]
    public bool MultipleDevices { get; set; } = false;

    [JsonPropertyName("otherTenants")]
    public bool OtherTenants { get; set; } = false;

    [JsonPropertyName("otherTenantsIds")]
    public string OtherTenantsIds { get; set; } = string.Empty;

    [JsonPropertyName("subjectName")]
    public string SubjectName { get; set; }

    [JsonPropertyName("subjectAltNames")]
    public string SubjectAltNames { get; set; } = string.Empty;

    [JsonPropertyName("durationInDays")]
    public int DurationInDays { get; set; } = 30;

    [JsonPropertyName("Ekus")]
    public string Ekus { get; set; } = string.Empty;

    [JsonPropertyName("KeyUsages")]
    public string KeyUsages { get; set; } = EZCAConstants.DIGITALSIGNATURE;

    [JsonPropertyName("PolicyName")]
    public string PolicyName { get; set; } = string.Empty;

    [JsonPropertyName("EncryptionKeyLocation")]
    public string EncryptionKeyLocation { get; set; } = string.Empty;

    [JsonPropertyName("BehalfOfAgents")]
    public string BehalfOfAgents { get; set; } = string.Empty;
}

public class BehalfOfAgentsDetails
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("ProfileID")]
    public string ProfileID { get; set; }
}
