using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Dbarone.Net.JsonDataStore
{
    /// <summary>
    /// Interface for a generic json data store.
    /// </summary>
    public interface IDataStore : IDisposable, ITransaction
    {
        IStorage Storage { get; }

        /// <summary>
        /// Saves the contents of the json file to storage.
        /// </summary>
        /// <param name="jsonData">The content to save.</param>
        void Save(bool loop);

        /// <summary>
        /// Reload json data from storage.
        /// </summary>
        void Reload();

    }
}