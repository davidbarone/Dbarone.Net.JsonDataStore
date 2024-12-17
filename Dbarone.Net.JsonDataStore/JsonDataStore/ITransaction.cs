using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Defines operations that can occur on a transaction.
/// </summary>
public interface ITransaction
{
    #region Transactions

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

    #endregion

    #region Collections

    IDocumentCollection<Collection> GetCollections();

    /// <summary>
    /// Gets a collection from the current transaction.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="name">Optional collection name. Defaults to the element type name.</param>
    /// <returns>Returns an IDocumentCollection.</returns>
    IDocumentCollection<T> GetCollection<T>(string? name = null) where T : class;
    IDocumentCollection<object> GetCollection(Type elementTypeName, string? collectionName = null);
    IDocumentCollection<Dictionary<string, object>> GetCollection(string collectionName);

    #endregion

    #region Sequences

    IDocumentCollection<Sequence> GetSequences();

    /// <summary>
    /// Gets the next sequence number for a type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>Returns the next sequence number.</returns>
    int Next<T>();

    int Next(string name);

    #endregion

    #region Constraints

    IDocumentCollection<Constraint> GetConstraints();
    void AddRequiredConstraint<T>(Expression<Func<T, object>> attribute);
    void AddUniqueConstraint<T>(Expression<Func<T, object>> attribute);
    void AddReferenceConstraint<T, U>(Expression<Func<T, object>> attribute, Expression<Func<U, object>> references);
    void DropConstraints<T>(Expression<Func<T, object>> attribute);
    void CheckIntegrity(ITransaction transaction);

    #endregion

}