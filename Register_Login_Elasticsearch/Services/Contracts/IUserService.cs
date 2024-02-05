using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.SeriLog;

namespace Register_Login_Elasticsearch.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<IEnumerable<Users>> GetAsync();
        Task<UserDto> GetByIdAsync(string id);
        Task<ResponseDto.LoginResult> LoginAsync(UserLoginDto loginDto);
        Task<UserCreateDto> CreateAsync(UserCreateDto userCreateDto);
        Task<UsersUpdateDto> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAllAsync();
        List<LogDocument> GetUserLogs(string userId);
    }
}
