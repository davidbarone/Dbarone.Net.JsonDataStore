namespace Dbarone.Net.JsonDataStore;

public interface ITransaction
{
    void Commit();

    void Rollback();

    ITransaction BeginTransaction();
}