using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;

namespace Register_Login_Elasticsearch.Repositories.Contracts
{
    public interface IUserRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsyncElastic(string id);
        Task<T> CreateAsync(T Entity);
        Task<T?> LoginAsync(UserLoginDto userLoginDto);
        Task<List<T>> GetAllDbAsync();
        Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteDatabaseAsync();

    }
}
