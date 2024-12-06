using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore;

public class Transaction : ITransaction
{
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
    public ITransaction? Parent { get; set; }
    public int TransactionNestingLevel
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

    public ITransaction? Child { get; set; }

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
    public virtual ITransaction BeginTransaction()
    {
        return new Transaction(this);
    }

    public virtual void Commit()
    {
        this.Parent!.Dom = this.Dom.DeepClone();
        Parent!.Child = null;
        this.Parent = null;
    }

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

    public virtual void Rollback()
    {
        // Set parent transation's child to null
        Parent!.Child = null;
        this.Parent = null;
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

        // always use current leaf transaction
        ITransaction leaf = this.Leaf;

        var data = new Lazy<List<T>>(() =>
        {
            lock (leaf.Dom)
            {
                if (leaf.Dom[name] is null)
                {
                    return new List<T>();
                }
                else
                {
                    return leaf.Dom[name]!.AsArray().Select(e => JsonSerializer.Deserialize<T>(e)).ToList()!;
                }
            }
        });

        var modificationCallback = (IDocumentCollection<T> coll) =>
        {
            if (leaf.IsActive)
            {
                JsonNode node = JsonSerializer.SerializeToNode(coll.AsList)!;
                leaf.Dom[name] = node;
            }
            else
            {
                throw new Exception("Cannot modify transaction which is not active. Has the transaction been closed?");
            }
        };

        return new DocumentCollection<T>(name, data, modificationCallback);
    }
}