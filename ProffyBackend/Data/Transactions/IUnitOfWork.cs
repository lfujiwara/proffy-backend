namespace ProffyBackend.Data.Transactions
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();
    }
}