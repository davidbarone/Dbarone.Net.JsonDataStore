using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class DocumentCollectionTests : BaseTests
{
    [Fact]
    public void InsertOne()
    {
        var store = DataStore.Create("", false);
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new FooBarBaz { Value = "foo" });
        Assert.Single(coll.AsList);
    }

    [Fact]
    public void InsertMany()
    {
        var store = DataStore.Create("", false);
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new List<FooBarBaz> { { new FooBarBaz { Value = "foo" } }, { new FooBarBaz { Value = "bar" } } });
        Assert.Equal(2, coll.Count);
    }

    [Fact]
    public void Update()
    {
        var str = this.GetJsonStream("simple.json").ToText();
        var store = DataStore.Create(str, false);
        var users = store.GetCollection<User>("users");

        // Update all users country
        users.Update(u => u.Country == "USA", (u) => { u.Country = "UK"; return u; });

        // Asserts
        Assert.Equal(2, users.Count);
        Assert.All(users.AsList, user => Assert.Equal("UK", user.Country));
    }

    [Fact]
    public void Update2()
    {
        var str = this.GetJsonStream("simple.json").ToText();
        var store = DataStore.Create(str, false);
        var users = store.GetCollection<User>("users");

        // Update all users country
        User newUser = new User
        {
            Country = "Country",
            FirstName = "FirstName",
            Surname = "Surname"
        };

        users.Update(u => u.Country == "USA", (u) => { u = newUser; return newUser; });

        // Asserts
        Assert.Equal(2, users.Count);
        Assert.All(users.AsList, user => Assert.Equal("Country", user.Country));
    }


    [Fact]
    public void Delete()
    {
        var str = this.GetJsonStream("simple.json").ToText();
        var store = DataStore.Create(str, false);
        var users = store.GetCollection<User>("users");

        // Update all users country
        Assert.Equal(2, users.Count);

        var deleted = users.Delete(u => u.FirstName == "John");
        Assert.Equal(1, deleted);

        // Asserts
        Assert.Equal(1, users.Count);   // 1 row remains
    }

    [Fact]
    public void Any()
    {
        var str = this.GetJsonStream("simple.json").ToText();
        var store = DataStore.Create(str, false);
        var users = store.GetCollection<User>("users");

        // Update all users country
        Assert.True(users.Any(u => u.FirstName == "John"));
    }
}