using Dbarone.Net.JsonDataStore;

namespace Dbarone.Net.JsonDataStore.Tests;

public class DataStoreTests : BaseTests
{

    [Fact]
    public void Open()
    {
        var json = this.GetJsonStream("tiny.json").ToText();
        var store = DataStore.Create(json, false);
        var str = store.Dom.ToJsonString();
        var expected = @"{""fooBarBaz"":[{""value"":""foo""}]}";
        Assert.Equal(expected, str);
    }

    [Fact]
    public void Create()
    {
        var store = DataStore.Create("", false);
        var str = store.Dom.ToJsonString();
        var expected = @"{}";
        Assert.Equal(expected, str);
    }

    [Fact]
    public void GetCollection()
    {
        var store = DataStore.Create("", false);
        var coll = store.GetCollection<FooBarBaz>();

        Assert.IsType<List<FooBarBaz>>(coll.AsList);
        Assert.NotNull(coll.AsList);
        Assert.Equal(0, coll.Count);
    }

    [Fact]
    public void ModificationAction()
    {
        // Tests that any modifications to collections are saved back to store
        var store = DataStore.Create("", false);
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new FooBarBaz { Value = "foo" });
        Assert.Single(coll.AsList);

        // Get another copy of collection - should have same item in it.
        var coll2 = store.GetCollection<FooBarBaz>();
        Assert.Single(coll2.AsList);
    }

    [Fact]
    public void AutoSave()
    {
        var fileName = $"{GetCallerName()}.json";
        using (var store = DataStore.Create(fileName, "", true))
        {
            var transaction = store.BeginTransaction();
            var coll1 = transaction.GetCollection<FooBarBaz>();
            Assert.Equal(0, coll1.Count);
            coll1.Insert(new FooBarBaz { Value = "foo" });
            Assert.Equal(1, coll1.Count);
            transaction.Commit();
        }
        var store2 = DataStore.Open(fileName, "", false);
        var coll2 = store2.GetCollection<FooBarBaz>();
        Assert.Equal(1, coll2.Count);
    }

    [Fact]
    public void Encrypt()
    {
        var plainFileName = $"{GetCallerName()}_plain.json";
        using (var store = DataStore.Create(plainFileName, "", true))
        {
            var transaction = store.BeginTransaction();
            var coll1 = transaction.GetCollection<FooBarBaz>();
            Assert.Equal(0, coll1.Count);
            coll1.Insert(new FooBarBaz { Value = "foo" });
            Assert.Equal(1, coll1.Count);
            transaction.Commit();
        }

        var encryptFileName = $"{GetCallerName()}_encrypt.json";
        using (var store = DataStore.Create(encryptFileName, "this is a file password", true))
        {
            var transaction = store.BeginTransaction();
            var coll1 = transaction.GetCollection<FooBarBaz>();
            Assert.Equal(0, coll1.Count);
            coll1.Insert(new FooBarBaz { Value = "foo" });
            Assert.Equal(1, coll1.Count);
            transaction.Commit();
        }

        // Try to open the encrypted file without password - should throw exception
        Assert.ThrowsAny<Exception>(() =>
        {
            using (var store = DataStore.Open(encryptFileName, "", true))
            {
                // This should fail as file is encrypted...
            }
        });
    }

    [Fact]
    public void Next()
    {
        var store = DataStore.Create("", false);
        var id = store.Next<User>();
        Assert.Equal(1, id);
        id = store.Next<User>();
        Assert.Equal(2, id);
        id = store.Next<FooBarBaz>();
        Assert.Equal(1, id);
        id = store.Next<User>();
        Assert.Equal(3, id);
    }
}