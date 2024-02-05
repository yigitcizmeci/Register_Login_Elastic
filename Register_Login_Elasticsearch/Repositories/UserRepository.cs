using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories.Contracts;
using Register_Login_Elasticsearch.Security;
using Register_Login_Elasticsearch.SeriLog;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;


namespace Register_Login_Elasticsearch.Repositories
{
    public class UserRepository : IUserRepository<Users>
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Regis_Login;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite";
        private readonly RepositoryContext _repositoryContext;
        private readonly ElasticClient _client;
        private readonly Verification_Code _verification_Code;
        private readonly IMemoryCache _memoryCache;
        private const string indexName = "users";
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(RepositoryContext repositoryContext, ElasticClient client, Verification_Code verification_Code, IMemoryCache memoryCache,
            ILogger<UserRepository> logger)
        {
            _repositoryContext = repositoryContext;
            _client = client;
            _verification_Code = verification_Code;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<Users> CreateAsync(Users newUser)
        {
            try
            {

                newUser.Password = Hashing.ToSHA256(newUser.Password);
                var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.UserName == newUser.UserName && q.Email == newUser.Email);
                if (existUser != null)
                {
                    _logger.LogWarning("Entered an exist Username or E-Mail");
                    throw new Exception("Username or E-Mail already exist.");
                }

                var isExist = await _client.SearchAsync<Users>(s => s
                  .Index(indexName)
                  .Query(q => q
                      .Bool(b => b
                          .Must(mu => mu
                              .Term(t => t.Field(f => f.Email).Value(newUser.Email.ToLower()))
                          )
                          .Must(mu => mu
                              .Term(t => t.Field(f => f.UserName).Value(newUser.UserName.ToLower()))
                          )
                      )
                  )
              );
                if (isExist.Documents.Any())
                {
                    _logger.LogWarning("Exist UserName or Email. (ES) UserId: {UserId}", newUser.Id);
                    throw new Exception("Username or Email is already exist in Elasticsearch. Please check your information.");
                }
                var response = await _client.IndexAsync(newUser, x => x.Index(indexName).Id(Guid.NewGuid()));
                if (!response.IsValid)
                {
                    _logger.LogWarning("Failed to add user Elasticsearch. UserId: {UserId}", newUser.Id);
                    throw new Exception("failed to add user. (elastic) Check Elastic container ");
                }
                newUser.Id = response.Id;
                await _verification_Code.CodeGenerator(newUser);
                await _repositoryContext.Users.AddAsync(newUser);
                await _repositoryContext.SaveChangesAsync();
                _logger.LogInformation("Successfully registered. UserId: {UserId}", newUser.Id);

                return newUser;
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong! Check Container. UserId: {UserId}", newUser.Id);
                throw;
            }
        }
        public async Task<Users> LoginAsync(UserLoginDto userLoginDto)
        {
            var hashedPassword = Hashing.ToSHA256(userLoginDto.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(
                q => q.UserName == userLoginDto.UserName && q.Password == hashedPassword);
            try
            {
                if (existUser == null)
                {
                    _logger.LogWarning("Entered Wrong UserName or Password");
                    throw new Exception("Wrong Username or Password");
                }

                var checkCode = _memoryCache.TryGetValue("VerificationCode", out string VerificationCode);
                if (!checkCode || userLoginDto.Verification_Code != VerificationCode)
                {
                    _logger.LogWarning("Wrong Verification code. UserId: {UserId}", existUser?.Id);
                    throw new Exception("İnvalid Verification code.");
                }
                _logger.LogInformation("Successfully logged in. UserId: {UserId}", existUser?.Id);
                return existUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging.");
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string _id)
        {
            var userToDeleteDb = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.Id == _id);
            var userToDeleteEs = await _client.GetAsync<Users>(_id, x => x.Index(indexName));
            if (userToDeleteDb == null && !userToDeleteEs.IsValid)
            {
                _logger.LogWarning("User not found. UserId: {UserId}", _id);
                throw new Exception($"User not found. Id: {_id}");
            }
            _repositoryContext.Users.Remove(userToDeleteDb);
            await Task.WhenAll(
            _client.DeleteAsync<Users>(_id, x => x.Index(indexName)),
            _repositoryContext.SaveChangesAsync()
                );
            _logger.LogInformation("User deleted successfully. UserId: {UserId}", _id);
            return true;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var allUsersDb = await _repositoryContext.Users.ToListAsync();
            var allUsersEs = await _client.SearchAsync<Users>(u => u.Index(indexName).Query(q => q.MatchAll()));
            if (allUsersDb == null && allUsersEs == null)
            {
                _logger.LogInformation("There are no any users.");
                throw new Exception("Users are missing");
            }
            _repositoryContext.Users.RemoveRange(allUsersDb);
            await Task.WhenAll(
                _client.DeleteByQueryAsync<Users>(x => x.Index(indexName).MatchAll()),
                _repositoryContext.SaveChangesAsync()
                );
            _logger.LogInformation("Users are deleted successfully");
            return true;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            var allUsersDb = await _repositoryContext.Users.ToListAsync();
            var allUsersEs = await _client.SearchAsync<Users>(u => u.Index(indexName).Query(q => q.MatchAll()));
            if (allUsersDb.Count == 0 && !allUsersEs.Documents.Any())
            {
                _logger.LogWarning("There are no any users. (GetAll)");
                throw new Exception("Users are missing");
            }
            _logger.LogInformation("Users listed");
            return allUsersDb;
        }


        public async Task<Users> GetByIdAsync(string _id)
        {
            try
            {
                var userDb = await _repositoryContext.Users.FirstOrDefaultAsync(x => x.Id == _id);
                var userEs = await _client.GetAsync<Users>(_id, x => x.Index(indexName));
                if (userDb == null && !userEs.IsValid)
                {
                    _logger.LogWarning("User not found. UserId: {UserId}", _id);
                    throw new Exception($"User not found. Id: {_id}");
                }
                var sd = await _repositoryContext.FindAsync<Users>(_id);
                _logger.LogInformation("User Found. UserId: {UserId}", _id);
                return userDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR occured. (GetById)");
                throw;
            }
        }

        public async Task<Users> UpdateAsync(UsersUpdateDto usersUpdateDto)
        {
            try
            {
                var isUserExistDb = await _repositoryContext.Users.FirstOrDefaultAsync(x => x.Id == usersUpdateDto.Id);
                var isUserExistEs = await _client.GetAsync<Users>(usersUpdateDto.Id, x => x.Index(indexName));

                if (isUserExistDb == null && !isUserExistEs.IsValid)
                {
                    _logger.LogWarning("User not found");
                    throw new Exception("User not exist" + "\n Make sure you are writing exist UserID");
                }

                var updateEs = await _client.UpdateAsync<Users, UsersUpdateDto>(usersUpdateDto.Id, x => x.Index(indexName).Doc(usersUpdateDto));

                if (isUserExistDb != null)
                {
                    isUserExistDb.UserName = usersUpdateDto.UserName;
                    isUserExistDb.Password = Hashing.ToSHA256(usersUpdateDto.Password);
                    _repositoryContext.Entry(isUserExistDb).State = EntityState.Modified;
                }

                await _repositoryContext.SaveChangesAsync();
                _logger.LogInformation("User updated successfully. UserId: {UserId}", usersUpdateDto.Id);

                return isUserExistDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while updating. UserId: {UserId}", usersUpdateDto.Id);
                throw;
            }
        }

        public List<LogDocument> GetUserLogs(string userId)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT Level, TimeStamp, LogEvent, Message, Exception, MessageTemplate, UserId FROM Logs WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<LogDocument> userLogs = new List<LogDocument>();

                        while (reader.Read())
                        {
                            LogDocument logDocument = new LogDocument
                            {
                                Level = reader["Level"].ToString(),
                                TimeStamp = (DateTimeOffset)reader["TimeStamp"],
                                Message = reader["Message"].ToString(),
                                Exception = reader["Exception"].ToString(),
                                MessageTemplate = reader["MessageTemplate"].ToString(),
                                UserId = reader["UserId"].ToString(),
                                logEventDetails = JsonConvert.DeserializeObject<LogEventDetails>(reader["LogEvent"].ToString())
                            };

                            userLogs.Add(logDocument);
                        }
                        return userLogs;
                    }
                }
            }

        }

        public async Task<IEnumerable<Users>> GetAsync()
        {
            var allUsers = await _repositoryContext.Users.ToListAsync();
            return allUsers;
        }
    }
}
