using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore;

public class Transaction : ITransaction
{
    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="parentTransaction">The parent transaction (optional).</param>
    public Transaction(ITransaction? parentTransaction)
    {
        this.Parent = parentTransaction;

        // Snapshot current dom.
        if (Parent is not null)
        {
            Dom = this.Parent.Dom.DeepClone();
        }
        else
        {
            Dom = JsonNode.Parse("{}")!;
        }
    }

    // Note we use mutable JsonNode instead of immutable JsonDocument.
    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom?pivots=dotnet-6-0
    public JsonNode Dom { get; set; }

    /// <summary>
    /// Returns the parent transaction if it exists.
    /// </summary>
    public ITransaction? Parent { get; set; }

    /// <summary>
    /// Returns the maximum nesting level currently existing.
    /// </summary>
    public int MaxLevel => this.Leaf.CurrentLevel;

    /// <summary>
    /// Returns the transaction nesting level.
    /// </summary>
    public int CurrentLevel
    {
        get
        {
            int level = 0;
            var parent = this.Parent;
            while (parent != null)
            {
                level++;
                parent = parent.Parent;
            }
            return level;
        }
    }

    /// <summary>
    /// Gets the immediate child transaction if it exists.
    /// </summary>
    public ITransaction? Child { get; set; }

    /// <summary>
    /// Gets the root DataStore object for the current transaction.
    /// </summary>
    public ITransaction Root
    {
        get
        {
            ITransaction transaction = this;
            while (transaction.Parent is not null)
            {
                transaction = transaction.Parent;
            }
            return transaction;
        }
    }

    /// <summary>
    /// Gets the leaf for the current transaction.
    /// </summary>
    public ITransaction Leaf
    {
        get
        {
            ITransaction transaction = this;
            while (transaction!.Child is not null)
            {
                transaction = transaction.Child;
            }
            return transaction;

        }
    }

    /// <summary>
    /// Returns true if the current transaction is the leaf level transaction.
    /// </summary>
    public bool IsLeaf => this.Leaf == this;

    /// <summary>
    /// Returns true if the transaction is active. A transaction is active if it is still connected to a DataStore object at the root.
    /// </summary>
    public bool IsActive
    {
        get
        {
            ITransaction current = this;
            while (current.Parent is not null)
            {
                current = current.Parent;
            }
            return current.GetType() == typeof(DataStore);
        }
    }

    /// <summary>
    /// Starts a nested transaction.
    /// </summary>
    public ITransaction BeginTransaction()
    {
        if (!this.IsLeaf)
        {
            throw new Exception("Transaction can only be created from current leaf transaction.");
        }
        else
        {
            var t = new Transaction(this);
            this.Child = t;
            return t;
        }
    }

    /// <summary>
    /// Commits all transactions under the current transaction.
    /// </summary>
    public void Commit()
    {
        var l = this.Leaf;
        while (l is not null && l != this)
        {
            l.Parent!.Dom = l.Dom.DeepClone();
            l.Parent.Child = null;
            l.Parent = null;
            l = this.Leaf;
        }
        if (this.Parent is not null)
        {
            this.Parent.Child = null;
            this.Parent = null;
        }
    }

    /// <summary>
    /// Returns true if the provided transaction is a descendent of the current transaction.
    /// </summary>
    /// <param name="transaction">The transaction to check.</param>
    /// <returns>Returns true if a descendent of the current transaction.</returns>
    public bool Contains(ITransaction transaction)
    {
        ITransaction current = this;
        while (current!.Child is not null)
        {
            if (current.Equals(transaction))
                return true;
            current = current.Child;
        }
        return false;
    }

    /// <summary>
    /// Rollback of all transactions under the current transaction.
    /// </summary>
    public void Rollback()
    {
        ITransaction l = this.Leaf;
        while (l is not null && l != this)
        {
            // Set parent transation's child to null
            l.Parent!.Child = null;
            l.Parent = null;
            l = this.Leaf;
        }
        if (this.Parent is not null)
        {
            this.Parent.Child = null;
            this.Parent = null;
        }
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

        if (!this.IsLeaf)
        {
            throw new Exception("Must be on leaf transaction to get a collection.");
        }
        else
        {

            var data = new Lazy<List<T>>(() =>
            {
                lock (Dom)
                {
                    if (Dom[name] is null)
                    {
                        return new List<T>();
                    }
                    else
                    {
                        return Dom[name]!.AsArray().Select(e => JsonSerializer.Deserialize<T>(e)).ToList()!;
                    }
                }
            });

            var modificationCallback = (IDocumentCollection<T> coll) =>
            {
                if (IsActive)
                {
                    JsonNode node = JsonSerializer.SerializeToNode(coll.AsList)!;
                    Dom[name] = node;
                }
                else
                {
                    throw new Exception("Cannot modify transaction which is not active. Has the transaction been closed?");
                }
            };

            return new DocumentCollection<T>(name, data, modificationCallback);
        }
    }
}