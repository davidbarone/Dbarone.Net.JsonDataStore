using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

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

    public static Stream ToStream(this string input)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(input);
        writer.Flush();
        stream.Position = 0; // Reset the stream position to the beginning
        return stream;
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