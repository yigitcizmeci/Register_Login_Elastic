using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Abstractions;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories.Contracts;
using Register_Login_Elasticsearch.Security;
using Register_Login_Elasticsearch.SeriLog;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;


namespace Register_Login_Elasticsearch.Repositories
{
    public class UserRepository : IUserRepository<Users>
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Regis_Login;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite";
        private readonly RepositoryContext _repositoryContext;
        private readonly ElasticClient _client;
        private readonly Verification_Code _verification_Code;
        private readonly IMemoryCache _memoryCache;
        //private readonly ILogger<UserRepository> _logger;
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
                //var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(q => q.UserName == newUser.UserName || q.Email == newUser.Email);
                //if (existUser != null)
                //{
                //    _logger.LogWarning("Username or Email is already exist");
                //    throw new Exception("Username or Email is already exist \n Please check your informations");
                //}

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
                    _logger.LogWarning("Exist UserName or Email. UserId: {UserId}", newUser.DatabaseId);
                    //_logDocument.LogUserActivity(newUser.ElasticId, newUser.DatabaseId.ToString(), "Exist UserName or Email", LogEventLevel.Warning);
                    throw new Exception("Username or Email is already exist in Elasticsearch. Please check your information.");
                }
                var response = await _client.IndexAsync(newUser, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));
                if (!response.IsValid)
                {
                    _logger.LogWarning("Failed to add user Elasticsearch. UserId: {UserId}", newUser.ElasticId);
                    //_logDocument.LogUserActivity(newUser.ElasticId, newUser.DatabaseId.ToString(), "Failed to add user Elasticsearch", LogEventLevel.Warning);
                    throw new Exception("failed to add user. (elastic) Check Elastic container ");
                }

                newUser.ElasticId = response.Id;
                await _verification_Code.CodeGenerator(newUser);
                await _repositoryContext.Users.AddAsync(newUser);
                await _repositoryContext.SaveChangesAsync();
                _logger.LogInformation("Successfully registered. UserId: {UserId}", newUser.DatabaseId);
                //_logDocument.LogUserActivity(newUser.ElasticId, newUser.DatabaseId.ToString(), "Successfully registered");

                return newUser;
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong! Check Container. UserId: {UserId}", newUser.DatabaseId);
                //_logDocument.LogUserActivity(newUser.ElasticId, newUser.DatabaseId.ToString(), "Something went wrong! Check Container", LogEventLevel.Error);
                throw;
            }
        }
        public async Task<Users?> LoginAsync(UserLoginDto userLoginDto)
        {
            var hashedPassword = Hashing.ToSHA256(userLoginDto.Password);
            var existUser = await _repositoryContext.Users.FirstOrDefaultAsync(
                q => q.UserName == userLoginDto.UserName && q.Password == hashedPassword);
            try
            {
                if (existUser == null)
                {
                    _logger.LogWarning("Wrong username or password.  UserId: {UserId}", existUser?.DatabaseId);
                    //_logDocument.LogUserActivity(existUser?.ElasticId, existUser.DatabaseId.ToString(), "Wrong username or password", LogEventLevel.Warning);
                    throw new Exception("Invalid Username or Password");
                }

                var checkCode = _memoryCache.TryGetValue("VerificationCode", out string? VerificationCode);
                if (!checkCode || userLoginDto.Verification_Code != VerificationCode)
                {

                    _logger.LogWarning("Wrong Verification code. UserId: {UserId}", existUser?.DatabaseId);
                    //_logDocument.LogUserActivity(existUser?.ElasticId, existUser.DatabaseId.ToString(), "Wrong Verification code", LogEventLevel.Warning);
                    throw new Exception("İnvalid Verification code.");
                }
                _logger.LogInformation("Successfully logged in. UserId: {UserId}", existUser?.DatabaseId);
                //_logDocument.LogUserActivity(existUser?.ElasticId, existUser.DatabaseId.ToString(), "Successfully logged in", LogEventLevel.Information);
                return existUser;
            }
            catch (Exception)
            {
                _logger.LogError("Error while logging. UserId: {UserId}", existUser?.DatabaseId);
                //_logDocument.LogUserActivity(existUser?.ElasticId, existUser.DatabaseId.ToString(), "Error while logging", LogEventLevel.Error);
                throw;
            }

        }

        public async Task<bool> DeleteAsync(string id)
        {
            var userToDelete = await _client.GetAsync<Users>(id, x => x.Index(indexName));
            if (!userToDelete.IsValid)
            {
                //_logDocument.LogUserActivity(userToDelete.Source.ElasticId, string.Empty,  "User Not found", LogEventLevel.Information);
                throw new Exception("User not found");
            }
            var response = await _client.DeleteAsync<Users>(id, x => x.Index(indexName));
            //_logDocument.LogUserActivity(userToDelete.Source.ElasticId, string.Empty, "User deleted successfully", LogEventLevel.Information);

            return response.IsValid;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var response = await _client.DeleteByQueryAsync<Users>(x => x.Index(indexName).MatchAll());
            if (!response.IsValid) throw new Exception("Failed");
            //_logDocument.LogUserActivity(null, string.Empty, "DELETED ALL USERS");
            return response.IsValid;
        }

        public async Task<List<Users>> GetAllAsync()
        {
            var response = await _client.SearchAsync<Users>(u => u.Index(indexName).Query(q => q.MatchAll()));
            if (!response.IsValid)
            {
                throw new Exception("User not found");
            }
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
            var response = await _client.UpdateAsync<Users, UsersUpdateDto>(usersUpdateDto.ElasticId, x => x.Index(indexName).Doc(usersUpdateDto));

            if (!response.IsValid)
            {
                //_logger.LogError($"Error in Elasticsearch UpdateAsync: {response.ServerError?.ToString()}");
                //_logger.LogError($"Debug Information: {response.DebugInformation}");
            }

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

        public Task<List<Users>> GetAllDbAsync()
        {
            var allUsers = _repositoryContext.Users.ToListAsync();
            if (allUsers == null) throw new Exception("Users not found");

            return allUsers;
        }
        //public void LogUserActivity(LogDocument logDocument)
        //{
        //    Log.Logger.Write(logDocument.logEventLevel, "{ElasticId}: {Message} - {Timestamp}", logDocument.ElasticId, logDocument.Message, DateTime.UtcNow);
        //}
        //public void LogUserActivity(string? ElasticId, string Message, LogEventLevel logLevel = LogEventLevel.Information)
        //{
        //    Log.Logger.Write(logLevel, "{ElasticId}: {Message} - {Timestamp}", ElasticId, Message, DateTime.UtcNow);
        //}
        
        public List<string> GetUserLogs(int userId)
        {
            List<string> userLogs = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT Message FROM Logs WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string logMessage = reader["Message"].ToString();
                            userLogs.Add(logMessage);
                        }
                    }
                }
            }

            return userLogs;
        }
    }
}
