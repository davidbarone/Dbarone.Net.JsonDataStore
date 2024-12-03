using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dbarone.Net.JsonDataStore
{
    /// <summary>
    /// Interface for a generic json data store.
    /// </summary>
    public interface IDataStore : IDisposable
    {
        JsonDocument Document { get; }
        IStorage Storage { get; }

        /// <summary>
        /// Gets a collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="name">Optional collection name. Defaults to the element type name.</param>
        /// <returns>Returns an IDocumentCollection.</returns>
        IDocumentCollection<T> GetCollection<T>(string? name = null) where T : class;

        /// <summary>
        /// Saves the contents of the json file to storage.
        /// </summary>
        /// <param name="jsonData">The content to save.</param>
        void Save();

        /// <summary>
        /// Reload json data from storage.
        /// </summary>
        void Reload();

        /// <summary>
        /// Calls the IStorage.Write() method to 
        /// </summary>
        void Checkpoint();
    }
}