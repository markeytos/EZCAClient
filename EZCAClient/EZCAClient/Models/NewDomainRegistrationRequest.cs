using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class NewDomainRegistrationRequest
{
    public NewDomainRegistrationRequest() { }

    public NewDomainRegistrationRequest(
        AvailableCAModel ca,
        string domain,
        List<AADObjectModel> owners,
        List<AADObjectModel> requesters,
        List<AADObjectModel>? requestersOnly,
        List<string>? notificationEmails
    )
    {
        CAID = ca.CAID;
        TemplateID = ca.TemplateID;
        Domain = domain;
        Owners = owners;
        Requesters = requesters;
        RequestersOnly = requestersOnly ?? new();
        NotificationEmails = notificationEmails ?? new();
    }

    [JsonPropertyName("CAID")]
    public string? CAID { get; set; }

    [JsonPropertyName("TemplateID")]
    public string? TemplateID { get; set; }

    [JsonPropertyName("Domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("Owners")]
    public List<AADObjectModel> Owners { get; set; } = new();

    [JsonPropertyName("Requesters")]
    public List<AADObjectModel> Requesters { get; set; } = new();

    [JsonPropertyName("RequestersOnly")]
    public List<AADObjectModel> RequestersOnly { get; set; } = new();

    [JsonPropertyName("NotificationEmails")]
    public List<string> NotificationEmails { get; set; } = new();
}
