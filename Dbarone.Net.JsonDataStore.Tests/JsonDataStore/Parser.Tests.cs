using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("123", "unsigned_integer")]
    [InlineData("+", "sign")]
    [InlineData("-", "sign")]
    [InlineData(".123", "exact_numeric_literal")]
    [InlineData("0.123", "exact_numeric_literal")]
    [InlineData("123.456", "exact_numeric_literal")]
    [InlineData("-123", "signed_integer")]
    [InlineData("+123", "signed_integer")]
    [InlineData("1E10", "approximate_numeric_literal")]
    [InlineData("1e10", "approximate_numeric_literal")]
    [InlineData("123e-10", "approximate_numeric_literal")]
    [InlineData("123.456", "unsigned_numeric_literal")]
    [InlineData("123.456e-10", "unsigned_numeric_literal")]
    [InlineData("-123.456", "signed_numeric_literal")]
    [InlineData("-123.456e-10", "signed_numeric_literal")]
    public void TestGrammar(string input, string rootProductionRule)
    {
        var ast = Parser.Parse(input, rootProductionRule);
        var a = ast;
    }

    [Fact]
    public void Test()
    {
        var ast = Parser.Parse("E123E-8", "aa");
        var a = ast;
    }
}