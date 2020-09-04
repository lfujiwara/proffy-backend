namespace ProffyBackend.Repositories
{
    public interface ICrudRepository<TEntity>
    {
        public TEntity Create(TEntity value);
        public void Update(TEntity value);
        public TEntity ReadById(int id);
        public TEntity DeleteById(int id);
    }
}