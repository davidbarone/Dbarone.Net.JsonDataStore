using System.Text.Json.Nodes;
using Dbarone.Net.JsonDataStore;

public class Transaction
{
    public Transaction(JsonNode dom, IDataStore store)
    {
        this.TransactionStack = new Stack<JsonNode>();
    }

    public Stack<JsonNode> TransactionStack { get; }
    int TransactionLevel => TransactionStack.Count();

    public void Commit()
    {

    }

    public void Rollback()
    {

    }
}