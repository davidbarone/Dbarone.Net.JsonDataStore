using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

public static class Extensions
{
    public static CryptoStream ToCryptoStream(this Stream stream, string password, CryptoStreamMode mode)
    {
        using (Aes aes = Aes.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var keyBytes = SHA256.Create().ComputeHash(passwordBytes).Take(128 / 8).ToArray();
            aes.Key = keyBytes;
            if (mode == CryptoStreamMode.Write)
            {
                // write IV to start of stream
                byte[] iv = aes.IV;
                var a = iv.Length;
                stream.Write(iv, 0, iv.Length);
            }
            else
            {
                // read IV at start of stream
                byte[] iv = new byte[aes.BlockSize / 8];
                stream.Read(iv, 0, aes.BlockSize / 8);
                aes.IV = iv;
            }

            ICryptoTransform? encryptor = null;
            if (mode == CryptoStreamMode.Write)
            {
                encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            }
            else
            {
                encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            }

            CryptoStream cryptoStream = new(
                stream,
                encryptor,
                mode,
                false);

            return cryptoStream;
        }
    }

    public static Stream ToStream(this string input)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(input);
        writer.Flush();
        stream.Position = 0; // Reset the stream position to the beginning
        return stream;
    }

    public static string ToText(this Stream stream)
    {
        StreamReader reader = new StreamReader(stream);
        string text = reader.ReadToEnd();
        return text;
    }

    public static JsonNode ToJsonObject(this string json)
    {
        var stream = json.ToStream();
        var jsonObj = JsonObject.Parse(stream)!;
        return jsonObj;
    }

    public static string ToJsonString(this JsonDocument jdoc)
    {
        using (var stream = new MemoryStream())
        {
            Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });
            jdoc.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    public static JsonNode? ToJsonNode(this JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Array => JsonArray.Create(element),
            JsonValueKind.Object => JsonObject.Create(element),
            _ => JsonValue.Create(element)
        };
    }
}