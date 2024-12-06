using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Defines operations that can occur on a transaction.
/// </summary>
public interface ITransaction
{
    void Commit();
    void Rollback();
    ITransaction BeginTransaction();
    JsonNode Dom { get; set; }
    ITransaction? Parent { get; set; }
    ITransaction? Child { get; set; }
    ITransaction Root { get; }
    ITransaction Leaf { get; }
    int CurrentLevel { get; }
    bool Contains(ITransaction transaction);
    bool IsActive { get; }
    bool IsLeaf { get; }
    bool IsDirty { get; set; }

    /// <summary>
    /// Gets a collection from the current transaction.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="name">Optional collection name. Defaults to the element type name.</param>
    /// <returns>Returns an IDocumentCollection.</returns>
    IDocumentCollection<T> GetCollection<T>(string? name = null) where T : class;
}