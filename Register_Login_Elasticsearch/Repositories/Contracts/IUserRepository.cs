using Microsoft.IdentityModel.Abstractions;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.SeriLog;
using Serilog.Events;

namespace Register_Login_Elasticsearch.Repositories.Contracts
{
    public interface IUserRepository<T> 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetByIdAsync(string _id);
        Task<T> CreateAsync(T Entity);
        Task<T> LoginAsync(UserLoginDto userLoginDto);
        List<LogDocument> GetUserLogs(string userId);
        Task<T> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteByIdAsync(string id);
        Task<bool> DeleteAllAsync();

    }
}
