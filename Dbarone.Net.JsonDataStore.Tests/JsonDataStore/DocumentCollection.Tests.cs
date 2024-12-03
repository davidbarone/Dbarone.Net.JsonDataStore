using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class DocumentCollectionTests
{
    [Fact]
    public void InsertOne()
    {
        var store = DataStore.Create();
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new FooBarBaz { Value = "foo" });
        Assert.Single(coll.AsList);
    }

    [Fact]
    public void InsertMany()
    {
        var store = DataStore.Create();
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new List<FooBarBaz> { { new FooBarBaz { Value = "foo" } }, { new FooBarBaz { Value = "bar" } } });
        Assert.Equal(2, coll.Count);
    }
}