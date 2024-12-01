using System.Text.Json;

namespace Dbarone.Net.JsonDataStore;

public interface IStorage
{
    public JsonDocument Read();
    public void Write(JsonDocument document);
}