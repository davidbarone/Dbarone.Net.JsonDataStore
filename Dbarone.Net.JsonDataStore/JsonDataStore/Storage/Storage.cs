using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Storage class. Json is always stored as UTF8. 
/// </summary>
public class Storage : IStorage
{
    private Stream _stream;

    public Storage(Stream stream)
    {
        this._stream = stream;
    }

    public JsonDocument ReadDocument()
    {
        _stream.Position = 0;

        using (var reader = new StreamReader(_stream, Encoding.UTF8, true, -1, true))
        {
            var str = reader.ReadToEnd();
            var doc = JsonDocument.Parse(str);
            return doc;
        }
    }

    public JsonNode ReadNode()
    {
        _stream.Position = 0;

        using (var reader = new StreamReader(_stream, Encoding.UTF8, true, -1, true))
        {
            var str = reader.ReadToEnd();
            var node = JsonObject.Parse(str)!;
            return node;
        }
    }

    public void WriteDocument(JsonDocument document)
    {
        var str = document.ToJsonString();
        using (var writer = new StreamWriter(_stream, Encoding.UTF8, -1, true))
        {
            writer.Write(str);
        }
    }

    public void WriteNode(JsonNode node)
    {
        var str = node.ToJsonString();
        using (var writer = new StreamWriter(_stream, Encoding.UTF8, -1, true))
        {
            writer.Write(str);
        }
    }
}