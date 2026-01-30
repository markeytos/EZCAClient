using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class AvailableSelfServiceModel
{
    [JsonPropertyName("TenantSelfServiceProfiles")]
    public List<DBSelfServiceScep> TenantSelfServiceProfiles { get; set; } = new();

    [JsonPropertyName("SelfServiceGuestProfiles")]
    public List<SelfServiceGuestDetails> SelfServiceGuestProfiles { get; set; } = new();
}

public class SelfServiceGuestDetails
{
    [JsonPropertyName("OriginalTenantPolicy")]
    public DBSelfServiceScep OriginalTenantPolicy { get; set; }

    [JsonPropertyName("GuestPolicy")]
    public DBSelfServiceGuest GuestPolicy { get; set; }
}
