using Microsoft.EntityFrameworkCore;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories.Contracts;
using Register_Login_Elasticsearch.Security;
using Register_Login_Elasticsearch.Services.Contracts;
using System.Diagnostics;

namespace Register_Login_Elasticsearch.Repositories
{
    public class UserRepository : IUserRepository<Users>
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly ElasticClient _client;
        private readonly Verification_Code _verification_Code;
        private const string indexName = "users";

        public UserRepository(RepositoryContext repositoryContext, ElasticClient client, Verification_Code verification_Code)
        {
            _repositoryContext = repositoryContext;
            _client = client;
            _verification_Code = verification_Code;
        }

        public async Task<Users> CreateAsync(Users newUser)
        {
            newUser.Password = Hashing.ToSHA256(newUser.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.UserName == newUser.UserName || q.Email == newUser.Email);
            if (existUser != null) throw new Exception("Username or Email is already exist");

            var response = await _client.IndexAsync(newUser, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            if (!response.IsValid) throw new InvalidOperationException("failed to add user. (elastic)");
            newUser.ElasticId = response.Id;

            await _verification_Code.CodeGenerator();

            await _repositoryContext.Users.AddAsync(newUser);
            await _repositoryContext.SaveChangesAsync();
            Debug.WriteLine("***** Registered successfully." +
                "\n Verification code sended *****");
            return newUser;

        }
        public async Task<Users?> LoginAsync(UserLoginDto userLoginDto)
        {
            var hashedPassword = Hashing.ToSHA256(userLoginDto.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(
                q => q.UserName == userLoginDto.UserName && q.Password == hashedPassword);

            return existUser;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Users>(id, x => x.Index(indexName));
            if (!response.IsValid) throw new Exception("User not found");
            await _repositoryContext.SaveChangesAsync();

            return response.IsValid;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var response = await _client.DeleteByQueryAsync<Users>(x => x.Index(indexName).MatchAll());
            if (!response.IsValid) throw new Exception("Failed");
            await _repositoryContext.SaveChangesAsync();

            return response.IsValid;
        }

        public async Task<List<Users>> GetAllAsync()
        {
            var response = await _client.SearchAsync<Users>(u => u.Index(indexName).Query(q => q.MatchAll()));
            if (!response.IsValid) throw new Exception("User not found");
            foreach (var hit in response.Hits) hit.Source.ElasticId = hit.Id;

            return response.Documents.ToList();
        }

        public async Task<Users> GetByIdAsync(int id)
        {
            var response = await _repositoryContext.FindAsync<Users>(id);
            if (response == null) throw new Exception("User not found");

            return response;
        }
        public async Task<Users> GetByIdAsyncElastic(string id)
        {
            var response = await _client.GetAsync<Users>(id, x => x.Index(indexName));
            if (!response.IsValid) throw new Exception("User not found in Elasticsearch");
            response.Source.ElasticId = response.Id;

            return response.Source;
        }

        public async Task<bool> UpdateAsync(UsersUpdateDto usersUpdateDto)
        {
            var response = await _client.UpdateAsync<Users, UsersUpdateDto>(usersUpdateDto.Id, x => x.Index(indexName).Doc(usersUpdateDto));
            await _repositoryContext.SaveChangesAsync();

            return response.IsValid;
        }

        public async Task<bool> DeleteDatabaseAsync()
        {
            var allUsers = await _repositoryContext.Users.ToListAsync();

            if (allUsers != null && allUsers.Any())
            {
                _repositoryContext.Users.RemoveRange(allUsers);
                await _repositoryContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
