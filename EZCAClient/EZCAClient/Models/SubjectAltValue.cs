using System.Text.Json.Serialization;

namespace EZCAClient.Models;

public class SubjectAltValue
{
    public SubjectAltValue() { }

    public SubjectAltValue(string value)
    {
        SubjectAltType = 2;
        ValueSTR = value;
    }

    public SubjectAltValue(string value, int valType)
    {
        SubjectAltType = valType;
        ValueSTR = value;
    }

    [JsonPropertyName("ValueSTR")]
    public string ValueSTR { get; set; } = string.Empty;

    [JsonPropertyName("SubjectAltType")]
    public int SubjectAltType { get; set; } //2 for DNS 7 for IP Address
}
