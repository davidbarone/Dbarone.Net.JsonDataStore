using Dbarone.Net.JsonDataStore;
using Dbarone.Net.JsonDataStore.Tests;

public class ConstraintTests
{
    [Fact]
    public void NotNullTest()
    {
        var store = DataStore.Create("", false);

        // Not null constraint on FooBarBaz.Value
        store.AddConstraint<FooBarBaz>(f => f.Value, true, null, null, null);

        var collection = store.GetCollection<FooBarBaz>();

        Assert.Throws<ConstraintException>(() =>
        {
            var t = store.BeginTransaction();
            var coll = t.GetCollection<FooBarBaz>();
            coll.Insert(new FooBarBaz { Value = null });    // null value not allowed!
            store.Commit(); // commit all transations, and force write of data
        });
    }
}