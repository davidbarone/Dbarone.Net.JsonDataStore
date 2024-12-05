using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class DocumentCollectionTests : BaseTests
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

    [Fact]
    public void Update()
    {
        var stream = this.GetJsonStream("simple.json");
        var store = DataStore.Open(stream);
        var users = store.GetCollection<User>("users");

        // Update all users country
        users.Update(u => u.Country == "USA", (u) => { u.Country = "UK"; return u; });

        // Asserts
        Assert.Equal(2, users.Count);
        Assert.All(users.AsList, user => Assert.Equal("UK", user.Country));
    }
}