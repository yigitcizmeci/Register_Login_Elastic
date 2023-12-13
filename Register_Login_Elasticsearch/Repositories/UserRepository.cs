using Microsoft.EntityFrameworkCore;
using Nest;
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
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.UserName == newUser.UserName || q.Email == newUser.Email);
            if (existUser != null) throw new Exception("Username or Email is already exist");
            var response = await _client.IndexAsync(newUser, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
            if (!response.IsValid) throw new InvalidOperationException("failed to add user. (elastic)");
                
            await _repositoryContext.AddAsync(newUser);
            await _repositoryContext.SaveChangesAsync();

            return newUser;

        }
        public Task<Users> LoginAsync(string eMail, string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<Users> DeleteAsync(int id)
        {
            var response = await _client.DeleteAsync<Users>(id, x => x.Index(indexName));
            throw new NotImplementedException();
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


        public Task<Users> UpdateAsync(Users Entity)
        {
            throw new NotImplementedException();
        }

    }
}
