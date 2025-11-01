using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

public class DataStore : Transaction, IDataStore
{
    IStorage _storage;

    public DataStore(IStorage storage, bool autoSave) : base(null)
    {
        this._storage = storage;
        Reload();

        if (autoSave)
        {
            Task.Run(() => Save(true));
        }
    }

    /// <summary>
    /// Creates an in=memory DataStore.
    /// </summary>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Create(string initialJson, bool autoSave)
    {
        IStorage storage = new Storage(initialJson);
        var ds = new DataStore(storage, autoSave);
        return ds;
    }

    /// <summary>
    /// Creates a DataStore at a specified file path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="password">Optional password for the data store.</param>
    /// <param name="autoSave">Set to true for database to auto-save every 1 second. If not set, you will need to manually save each time.</param>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Create(string path, string password, bool autoSave)
    {
        IStorage storage = new FileStorage(path, FileMode.Create, password);
        storage.Write("{}");
        return Open(path, password, autoSave);
    }

    /// <summary>
    /// Opens an existing DataStore at a specified file path.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="password">Optional password for the data store.</param>
    /// <param name="autoSave">Set to true for database to auto-save every 1 second. If not set, you will need to manually save each time.</param>
    /// <returns>Returns a DataStore object.</returns>
    public static DataStore Open(string path, string password, bool autoSave)
    {
        IStorage storage = new FileStorage(path, FileMode.Open, password);
        return new DataStore(storage, autoSave);
    }

    public void Dispose()
    {
        this.Save(false);
    }

    /// <summary>
    /// Reloads the json document from storage.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Reload()
    {
        this.Dom = _storage.ReadNode();
    }

    public void Save(bool loop = false)
    {
        do
        {
            if (this.IsDirty)
            {
                _storage.WriteNode(this.Dom);
                this.IsDirty = false;
            }
            if (loop)
            {
                Thread.Sleep(1000);
            }
        } while (loop);
    }


    public IStorage Storage => this._storage;

}