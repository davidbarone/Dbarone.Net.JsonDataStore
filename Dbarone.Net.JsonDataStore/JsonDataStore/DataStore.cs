using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

public class DataStore : IDataStore
{
    IStorage _storage;
    private Stream _stream;
    bool _autosave;

    // Note we use mutable JsonNode instead of immutable JsonDocument.
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom?pivots=dotnet-6-0
    private JsonNode _dom;

    public DataStore(Stream stream, string password, bool autoSave = false)
    {
        if (!string.IsNullOrWhiteSpace(password))
        {
            _stream = stream.ToCryptoStream(password);
        }
        else
        {
            _stream = stream;
        }

        this._storage = new Storage(_stream);

        // Create new document if stream is empty.
        if (stream.Length == 0)
        {
            _dom = JsonNode.Parse("{}")!;
            Save();
        }
        else
        {
            Reload();
        }
    }

    /// <summary>
    /// Creates an in=memory DataStore.
    /// </summary>
    /// <param name="password">Optional password for the data store.</param>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Create(string password = "", bool autoSave = false)
    {
        MemoryStream ms = new MemoryStream();
        return new DataStore(ms, password);
    }

    /// <summary>
    /// Creates a DataStore at a specified file path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="password">Optional password for the data store.</param>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Create(string path, string password = "", bool autoSave = false)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        return new DataStore(fs, password);
    }

    /// <summary>
    /// Opens an existing DataStore at a specified file path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="password">Optional password for the data store.</param>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Open(string path, string password = "", bool autoSave = false)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        return new DataStore(fs, password);
    }

    public static DataStore Open(Stream stream, string password = "", bool autoSave = false)
    {
        return new DataStore(stream, password);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a collection. If no collection exists, a new one is automatically created.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="name">Optional name for the collection. If not specified, the element type name is used as the collection name.</param>
    /// <returns>Returns a collection.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IDocumentCollection<T> GetCollection<T>(string? name = null) where T : class
    {
        name = name ?? typeof(T).Name;

        var data = new Lazy<List<T>>(() =>
        {
            lock (this._dom)
            {
                if (_dom[name] is null)
                {
                    return new List<T>();
                }
                else
                {
                    return _dom[name].AsArray().Select(e => JsonSerializer.Deserialize<T>(e)).ToList()!;
                }
            }
        });

        var modificationCallback = (IDocumentCollection<T> coll) =>
        {
            JsonNode node = JsonSerializer.SerializeToNode(coll.AsList)!;
            _dom[name] = node;
        };

        return new DocumentCollection<T>(name, data, modificationCallback);
    }

    /// <summary>
    /// Reloads the json document from storage.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Reload()
    {
        this._dom = _storage.ReadNode();
    }

    public void Save()
    {
        _storage.WriteNode(this._dom);
    }

    public JsonNode Document => this._dom;
    public IStorage Storage => this._storage;
}