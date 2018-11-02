namespace Common.Foundation.Repositories
{
    public interface IRepositoryFactory
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IReadRepository<T> GetReadRepository<T>() where T : class;
    }

}
