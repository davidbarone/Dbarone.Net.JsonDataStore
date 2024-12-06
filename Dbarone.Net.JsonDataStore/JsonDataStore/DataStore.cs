using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

public class DataStore : Transaction, IDataStore
{
    IStorage _storage;
    private Stream _stream;
    bool _autosave;
    public DataStore(Stream stream, string password, bool autoSave = false) : base(null)
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
            Dom = JsonNode.Parse("{}")!;
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
    /// Reloads the json document from storage.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Reload()
    {
        this.Dom = _storage.ReadNode();
    }

    public void Save()
    {
        _storage.WriteNode(this.Dom);
    }

    public IStorage Storage => this._storage;
}