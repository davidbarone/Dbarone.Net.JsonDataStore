using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class ParserTests
{
    [Fact]
    public void CreateCollection1()
    {
        var input = "CREATE COLLECTION myCollection (A)";
        var ast = Parser.Parse(input);
        var a = ast;
    }
}