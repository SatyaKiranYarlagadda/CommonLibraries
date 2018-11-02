namespace Common.Foundation.Repositories
{
    public interface IRepositoryAsyncFactory
    {
        IGenericRepositoryAsync<T> GetRepositoryAsync<T>() where T : class;
    }
}
