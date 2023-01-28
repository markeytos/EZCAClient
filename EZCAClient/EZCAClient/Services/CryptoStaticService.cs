using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace EZCAClient.Services;

public static class CryptoStaticService
{
    public static X509Certificate2 ImportCertFromPEMString(string pemCert)
    {
        pemCert = Regex.Replace(pemCert, @"-----[a-z A-Z]+-----", "").Trim();
        return new X509Certificate2(Convert.FromBase64String(pemCert));
    }
    
    public static string PemEncodeSigningRequest(CertificateRequest request)
    {
        byte[] pkcs10 = request.CreateSigningRequest();
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");
        builder.AppendLine(Convert.ToBase64String(pkcs10, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE REQUEST-----");
        return builder.ToString();
    }

    public static string ExportToPEM(X509Certificate2 cert)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("-----BEGIN CERTIFICATE-----");
        builder.AppendLine(Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE-----");
        return builder.ToString();
    }
}