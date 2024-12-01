using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class DataStoreTests : BaseTests
{

    [Fact]
    public void Open()
    {
        var stream = this.GetJsonStream("tiny.json");
        var store = DataStore.Open(stream);
        var str = store.Document.ToJsonString();
        var expected = @"{""fooBarBaz"":[{""value"":""foo""}]}";
        Assert.Equal(expected, str);
    }

    [Fact]
    public void Create()
    {
        var store = DataStore.Create();
        var str = store.Document.ToJsonString();
        var expected = @"{}";
        Assert.Equal(expected, str);
    }

}