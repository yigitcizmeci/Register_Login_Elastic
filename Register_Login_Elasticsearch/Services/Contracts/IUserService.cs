using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Services.Contracts
{
    public interface IUserService
    {
        Task<ImmutableList<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> UpdateAsync(UsersUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<UserLoginDto> LoginAsync(UserLoginDto loginDto);
        Task<UserDto> CreateAsync(UserCreateDto userCreateDto);
    }
}
