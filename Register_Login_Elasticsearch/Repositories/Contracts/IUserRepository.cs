using Microsoft.IdentityModel.Abstractions;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.SeriLog;
using Serilog.Events;

namespace Register_Login_Elasticsearch.Repositories.Contracts
{
    public interface IUserRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsyncElastic(string id);
        Task<T> CreateAsync(T Entity);
        Task<T?> LoginAsync(UserLoginDto userLoginDto);
        //void LogUserActivity(string ElasticId, string Message, LogEventLevel logLevel = LogEventLevel.Information);
        List<string> GetUserLogs(int userId);
        Task<List<T>> GetAllDbAsync();
        Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteDatabaseAsync();

    }
}
