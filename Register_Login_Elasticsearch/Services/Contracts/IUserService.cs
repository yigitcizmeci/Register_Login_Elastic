using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.SeriLog;
using Serilog.Events;
using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Services.Contracts
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByIdAsyncElastic(string id);
        Task<List<UserDto>> GetAllUsersDbAsync(); 
        Task<ResponseDto.LoginResult> LoginAsync(UserLoginDto loginDto);
        Task<UserCreateDto> CreateAsync(UserCreateDto userCreateDto);
        Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAllAsyncElastic();
        Task<bool> DeleteAllAsync();
        //void UsersLog(string ElasticId, string Message, LogEventLevel logLevel = LogEventLevel.Information);
        List<string> GetUserLogs(int userId);
    }
}
