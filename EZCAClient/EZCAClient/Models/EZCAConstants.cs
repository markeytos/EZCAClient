using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZCAClient.Models;

public static class EZCAConstants
{
    public const string AnyEKU = "2.5.29.37.0";
    public const string ClientAuthenticationEKU = "1.3.6.1.5.5.7.3.2";
    public const string ServerAuthenticationEKU = "1.3.6.1.5.5.7.3.1";
    public const string SmartCardLogonEKU = "1.3.6.1.4.1.311.20.2.2";
    public const string KDCAuthForDCsEKU = "1.3.6.1.5.2.3.5";
    public static List<string> DomainControllerDefaultEKUs = new()
        {
            ClientAuthenticationEKU,
            ServerAuthenticationEKU,
            SmartCardLogonEKU,
            KDCAuthForDCsEKU
        };
    //Certificate Locations
    public const string IOT = "IoT Device";
    public const string IOTEDGE = "IoT Edge";
    public const string IMPORTCSR = "Import CSR";
    public const string ACME = "ACME";
    public const string SCEP = "SCEP";
    public const string DomainController = "Domain Controller";
}