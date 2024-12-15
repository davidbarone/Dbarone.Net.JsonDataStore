using Dbarone.Net.JsonDataStore;
using Dbarone.Net.JsonDataStore.Tests;

public class ConstraintTests
{
    [Fact]
    public void NotNullConstraint()
    {
        var store = DataStore.Create("", false);

        // Not null constraint on FooBarBaz.Value
        store.AddConstraint<FooBarBaz>(f => f.Value, true, null, null, null);

        Assert.Throws<ConstraintException>(() =>
        {
            var t = store.BeginTransaction();
            var coll = t.GetCollection<FooBarBaz>();
            coll.Insert(new FooBarBaz { Value = null });    // null value not allowed!
            store.Commit(); // commit all transations, and force write of data
        });
    }

    [Fact]
    public void UniqueConstraint()
    {
        var store = DataStore.Create("", false);

        // Not null constraint on FooBarBaz.Value
        store.AddConstraint<FooBarBaz>(f => f.Value, false, true, null, null);

        Assert.Throws<ConstraintException>(() =>
        {
            var t = store.BeginTransaction();
            var coll = t.GetCollection<FooBarBaz>();
            coll.Insert(new FooBarBaz { Value = "foo" });
            coll.Insert(new FooBarBaz { Value = "foo" });    // duplicate value!
            store.Commit(); // commit all transations, and force write of data
        });
    }
}