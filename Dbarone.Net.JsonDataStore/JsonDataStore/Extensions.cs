using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Dbarone.Net.JsonDataStore;

public static class Extensions
{
    public static CryptoStream ToCryptoStream(this Stream stream, string password)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(password);
            byte[] iv = aes.IV;
            stream.Write(iv, 0, iv.Length);

            CryptoStream cryptoStream = new(
                stream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write | CryptoStreamMode.Write);

            return cryptoStream;
        }
    }

    public static string ToJsonString(this JsonDocument jdoc)
    {
        using (var stream = new MemoryStream())
        {
            Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            jdoc.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}