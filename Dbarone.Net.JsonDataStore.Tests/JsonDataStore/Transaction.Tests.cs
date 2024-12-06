namespace Dbarone.Net.JsonDataStore.Tests;
public class TransactionTests : BaseTests
{
    [Fact]
    public void Rollback()
    {
        var stream = this.GetJsonStream("simple.json");
        var store = DataStore.Open(stream);

        // Begin Transaction
        var transaction = store.BeginTransaction();

        var users = transaction.GetCollection<User>("users");

        // Update all users country
        Assert.Equal(2, users.Count);

        // Delete both rows
        var deleted = users.Delete(u => true);
        Assert.Equal(0, users.Count);

        // Rollback - should revert back to 2 rows
        store.Rollback();

        // Try and do anything with the collection - should throw error
        Assert.Throws<Exception>(() => users.Delete(u => true));
    }

    [Fact]
    public void Commit()
    {
        var stream = this.GetJsonStream("simple.json");
        var store = DataStore.Open(stream);

        // Begin Transaction
        var transaction = store.BeginTransaction();

        var users = transaction.GetCollection<User>("users");

        // Update all users country
        Assert.Equal(2, users.Count);

        users.Insert(new User { FirstName = "John", Surname = "Smith", Country = "UK" });
        Assert.Equal(3, users.Count);

        // commit - should preserve 3 rows
        store.Commit();

        // Get collection again - should have 3 rows
        var users2 = store.GetCollection<User>("users");
        Assert.Equal(3, users2.Count);
    }

    [Fact]
    public void NestingLevel()
    {
        var stream = this.GetJsonStream("simple.json");
        var store = DataStore.Open(stream);

        Assert.Equal(0, store.MaxLevel);

        // Begin Transaction
        var transaction1 = store.BeginTransaction();
        Assert.Equal(1, store.MaxLevel);

        // Begin Transaction
        var transaction2 = transaction1.BeginTransaction();
        Assert.Equal(2, store.MaxLevel);

        // Rollback 2
        transaction2.Rollback();
        Assert.Equal(1, store.MaxLevel);

        // Rollback 1
        transaction1.Rollback();
        Assert.Equal(0, store.MaxLevel);
    }
}