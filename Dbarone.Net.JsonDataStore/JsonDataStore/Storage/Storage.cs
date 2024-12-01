using System.Text;
using System.Text.Json;

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

    public JsonDocument Read()
    {
        _stream.Position = 0;

        using (var reader = new StreamReader(_stream, Encoding.UTF8, true, -1, true))
        {
            var str = reader.ReadToEnd();
            var doc = JsonDocument.Parse(str);
            return doc;
        }
    }

    public void Write(JsonDocument document)
    {
        var str = document.ToJsonString();
        using (var writer = new StreamWriter(_stream, Encoding.UTF8, -1, true))
        {
            writer.Write(str);
        }
    }
}