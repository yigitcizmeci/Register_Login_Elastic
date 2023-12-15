using Microsoft.EntityFrameworkCore;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories.Contracts;
using Register_Login_Elasticsearch.Security;
using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Repositories
{
    public class UserRepository : IUserRepository<Users>
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly ElasticClient _client;
        private readonly TokenHandler _tokenHandler;
        private const string indexName = "users";

        public UserRepository(RepositoryContext repositoryContext, TokenHandler tokenHandler, ElasticClient client)
        {
            _repositoryContext = repositoryContext;
            _tokenHandler = tokenHandler;
            _client = client;
        }

        public async Task<Users> CreateAsync(Users newUser)
        {
            newUser.Password = Hashing.ToSHA256(newUser.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.UserName == newUser.UserName || q.Email == newUser.Email);
            if (existUser != null) throw new Exception("Username or Email is already exist");

            var response = await _client.IndexAsync(newUser, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            if (!response.IsValid) throw new InvalidOperationException("failed to add user. (elastic)");
                
            await _repositoryContext.AddAsync(newUser);
            await _repositoryContext.SaveChangesAsync();

            return newUser;

        }
        public async Task<ResponseDto.LoginResult> LoginAsync(UserLoginDto userLoginDto)
        {
            var hashedPassword = Hashing.ToSHA256(userLoginDto.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(
                q => q.UserName == userLoginDto.UserName && q.Password == hashedPassword);

            if (existUser == null) throw new Exception("İnvalid username or password");
            else
            {
                string token = _tokenHandler.CreateToken(existUser);
                var loginResult = new ResponseDto.LoginResult(existUser, token.ToString(), "Login Successfully");
                return loginResult;
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync<Users>(id, x => x.Index(indexName));
            if (!response.IsValid) throw new Exception("Usern not found");

            return response.IsValid;
        }

        public async Task<ImmutableList<Users>> GetAllAsync()
        {
            var response = await _client.SearchAsync<Users>(u => u.Index(indexName).Query(q => q.MatchAll()));
            if (!response.IsValid) throw new Exception("User not found");
            return response.Documents.ToImmutableList();
        }

        public async Task<Users> GetByIdAsync(int id)
        {
            var response = await _client.GetAsync<Users>(id, x => x.Index(indexName));
            if (!response.IsValid) throw new Exception("User not found");
            return response.Source;
        }


        public async Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto)
        {
            var response = await _client.UpdateAsync<Users, UsersUpdateDto>(usersUpdateDto.id, x => x.Index(indexName).Doc(usersUpdateDto));
            return response.IsValid;
        }

    }
}
