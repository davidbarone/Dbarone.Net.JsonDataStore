using System.Text.Json.Nodes;
using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore;

public class Transaction : ITransaction
{
    private object _parent;

    public Transaction(object parent)
    {
        this._parent = parent;
    }

    public Stack<JsonNode> TransactionStack { get; }
    int TransactionLevel => TransactionStack.Count();

    /// <summary>
    /// Starts a nested transaction.
    /// </summary>
    public ITransaction BeginTransaction()
    {
        return new Transaction(this);
    }

    public void Commit()
    {

    }

    public void Rollback()
    {

    }
}