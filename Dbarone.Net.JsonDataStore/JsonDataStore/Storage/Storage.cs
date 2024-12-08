using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Storage class. Json is always stored as UTF8. 
/// </summary>
public class Storage : IStorage
{
    protected string _jsonStr = "";

    public Storage()
    {
        _jsonStr = "";
    }

    public Storage(string initialJson)
    {
        _jsonStr = string.IsNullOrEmpty(initialJson) ? "{}" : initialJson;
    }

    protected virtual Stream CreateStream(StreamMode mode)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(_jsonStr));
    }

    public JsonDocument ReadDocument()
    {
        var str = Read();
        return JsonDocument.Parse(str);
    }

    public JsonNode ReadNode()
    {
        var str = Read();
        return JsonObject.Parse(str);
    }

    public void WriteDocument(JsonDocument document)
    {
        var _jsonStr = document.ToJsonString();
        Write(_jsonStr);
    }

    public void WriteNode(JsonNode node)
    {
        var _jsonStr = node.ToJsonString();
        Write(_jsonStr);
    }

    public string Read()
    {
        using (var stream = CreateStream(StreamMode.READ))
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, -1, false))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }
    }

    public void Write(string json)
    {
        var _jsonStr = json;
        using (var stream = CreateStream(StreamMode.WRITE))
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, -1, false))
            {
                writer.Write(json);
            }
        }
    }

    public bool IsNew
    {
        get
        {
            return string.IsNullOrEmpty(_jsonStr);
        }
    }
}