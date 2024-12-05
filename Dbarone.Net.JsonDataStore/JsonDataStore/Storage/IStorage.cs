using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

public interface IStorage
{
    /// <summary>
    /// Reads json into 
    /// </summary>
    /// <returns></returns>
    public JsonDocument ReadDocument();
    public void WriteDocument(JsonDocument document);

    public JsonNode ReadNode();
    public void WriteNode(JsonNode node);

}