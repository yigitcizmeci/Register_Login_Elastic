using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Repositories.Contracts
{
    public interface IUserRepository<T>
    {
        Task<ImmutableList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T Entity);
        Task<T> LoginAsync(string eMail, string userName, string password);
        Task<T> UpdateAsync(T Entity);
        Task<T> DeleteAsync(int id);

    }
}
