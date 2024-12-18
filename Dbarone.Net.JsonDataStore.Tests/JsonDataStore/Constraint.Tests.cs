using Dbarone.Net.JsonDataStore;
using Dbarone.Net.JsonDataStore.Tests;

public class ConstraintTests
{
    [Fact]
    public void NotNullConstraint()
    {
        var store = DataStore.Create("", false);

        Assert.Throws<ConstraintException>(() =>
        {
            var t = store.BeginTransaction();

            // Not null constraint on FooBarBaz.Value
            t.AddRequiredConstraint<FooBarBaz>(f => f.Value);

            var coll = t.GetCollection<FooBarBaz>();
            coll.Insert(new FooBarBaz { Value = null });    // null value not allowed!
            store.Commit(); // commit all transations, and force write of data
        });
    }

    [Fact]
    public void UniqueConstraint()
    {
        var store = DataStore.Create("", false);


        Assert.Throws<ConstraintException>(() =>
        {
            var t = store.BeginTransaction();

            // Unique constraint on FooBarBaz.Value
            t.AddUniqueConstraint<FooBarBaz>(f => f.Value);

            var coll = t.GetCollection<FooBarBaz>();
            coll.Insert(new FooBarBaz { Value = "foo" });
            coll.Insert(new FooBarBaz { Value = "foo" });    // duplicate value!
            store.Commit(); // commit all transations, and force write of data
        });
    }

    [Fact]
    public void ReferenceConstraint()
    {
        var store = DataStore.Create("", false);

        // Add product
        var r = store.BeginTransaction();
        var products = r.GetCollection<Product>();
        products.Insert(new Product { ProductCode = "A", ProductName = "Product A" });

        // Add sales table
        var sales = r.GetCollection<Sales>();

        // Add reference constraint
        r.AddReferenceConstraint<Sales, Product>(s => s.ProductCode, p => p.ProductCode);

        // Add 1 sales record which is valid
        sales.Insert(new Sales { SalesDate = DateTime.Now, ProductCode = "A", Quantity = 1, SalesAmount = 10 });

        r.Commit();

        Assert.Throws<ConstraintException>(() =>
        {
            var u = store.BeginTransaction();

            // Add 1 sales record which is not valid
            var sales = u.GetCollection<Sales>();
            sales.Insert(new Sales { SalesDate = DateTime.Now, ProductCode = "B", Quantity = 1, SalesAmount = 10 });

            store.Commit(); // commit all transations, and force write of data
        });
    }

    [Fact]
    public void ReferenceConstraintDictionary()
    {
        var store = DataStore.Create("", false);

        // Add product
        var r = store.BeginTransaction();
        var products = r.GetCollection("products");
        products.Insert(new Dictionary<string, object>() {
            {"ProductCode", "A"},
            {"ProductName", "Test Product"}
        });

        // Add sales table
        var sales = r.GetCollection("sales");

        // Add reference constraint
        r.AddConstraint("sales", "ProductCode", ConstraintType.REFERENCE, "products", "ProductCode");

        // Add 1 sales record which is valid
        sales.Insert(new Dictionary<string, object>()
        {
            {"SalesDate", DateTime.Now },
            {"ProductCode", "A"},
            {"Quantity", 1 },
            {"SalesAmount", 10 }
        });

        r.Commit();

        Assert.Throws<ConstraintException>(() =>
        {
            var u = store.BeginTransaction();

            // Add 1 sales record which is not valid
            var sales = u.GetCollection("sales");

            sales.Insert(new Dictionary<string, object>() {
                {"SalesDate", DateTime.Now },
                {"ProductCode", "B"},   // no 'B' product code
                {"Quantity", 1 },
                {"SalesAmount", 10 }
            });

            store.Commit(); // commit all transations, and force write of data
        });
    }
}