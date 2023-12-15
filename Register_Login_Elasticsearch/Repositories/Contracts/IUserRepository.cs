using Register_Login_Elasticsearch.DTOs;
using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Repositories.Contracts
{
    public interface IUserRepository<T>
    {
        Task<ImmutableList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> CreateAsync(T Entity);
        Task<ResponseDto.LoginResult> LoginAsync(UserLoginDto userLoginDto);
        Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto);
        Task<bool> DeleteAsync(int id);

    }
}
