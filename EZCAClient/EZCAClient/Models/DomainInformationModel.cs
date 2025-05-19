using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class DomainInformationModel
{
    public DomainInformationModel() { }
    

    [JsonPropertyName("CAID")]
    public string CAID { get; set; }

    [JsonPropertyName("TemplateID")]
    public string TemplateID { get; set; }

    [JsonPropertyName("DomainID")]
    public string DomainID { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("Domain")]
    public string Domain { get; set; }

    [JsonPropertyName("CAFriendlyName")]
    public string CAFriendlyName { get; set; }

    [JsonPropertyName("IsOwner")]
    public bool IsOwner { get; set; }

    [JsonPropertyName("IsRequester")]
    public bool IsRequester { get; set; }

    [JsonPropertyName("Owners")]
    public List<AADObjectModel> DomainOwners { get; set; } = new();

    [JsonPropertyName("Requesters")]
    public List<AADObjectModel> Requesters { get; set; } = new();

    [JsonPropertyName("RequestersOnly")]
    public List<AADObjectModel> RequestersOnly { get; set; } = new();

    [JsonPropertyName("NotificationEmails")]
    public List<string> NotificationEmails { get; set; } = new();
}