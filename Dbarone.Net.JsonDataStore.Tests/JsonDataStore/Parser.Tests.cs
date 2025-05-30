using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("123", "unsigned_integer", true)]
    [InlineData("+", "sign", true)]
    [InlineData("-", "sign", true)]
    [InlineData(".123", "exact_numeric_literal", true)]
    [InlineData("0.123", "exact_numeric_literal", true)]
    [InlineData("123.456", "exact_numeric_literal", true)]
    [InlineData("-123", "signed_integer", true)]
    [InlineData("+123", "signed_integer", true)]
    [InlineData("1E10", "approximate_numeric_literal", true)]
    [InlineData("1e10", "approximate_numeric_literal", true)]
    [InlineData("123e-10", "approximate_numeric_literal", true)]
    public void TestGrammar(string input, string rootProductionRule, bool isValid)
    {
        var ast = Parser.Parse(input, rootProductionRule);
        var a = ast;
    }

    [Fact]
    public void Test()
    {
        var ast = Parser.Parse("123E-8", "approximate_numeric_literal");
        var a = ast;
    }
}