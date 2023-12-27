using AutoMapper;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories;
using Register_Login_Elasticsearch.Security;
using Register_Login_Elasticsearch.Services.Contracts;

namespace Register_Login_Elasticsearch.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        private readonly IMapper _mapper;
        private readonly TokenHandler _tokenHandler;

        public UserService(UserRepository repository, IMapper mapper, TokenHandler tokenHandler)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenHandler = tokenHandler;
        }

        public async Task<UserCreateDto> CreateAsync(UserCreateDto userCreateDto)
        {
            var userToCreate = _mapper.Map<Users>(userCreateDto);
            var createdUser = await _repository.CreateAsync(userToCreate);
            var userDto = _mapper.Map<UserCreateDto>(createdUser);

            return userDto;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var allUsers = await _repository.GetAllAsync();
            var response = _mapper.Map<List<UserDto>>(allUsers);

            return response;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var getById = await _repository.GetByIdAsync(id);
            var userDtos = _mapper.Map<UserDto>(getById);

            return userDtos;
        }

        public async Task<UserDto?> GetByIdAsyncElastic(string id)
        {
            var getById = await _repository.GetByIdAsyncElastic(id);
            var userDtos = _mapper.Map<UserDto?>(getById);

            return userDtos;
        }

        public async Task<ResponseDto.LoginResult> LoginAsync(UserLoginDto loginDto)
        {
            var existUser = await _repository.LoginAsync(loginDto);

            if (existUser == null) throw new Exception("Invalid username or password");
        
                string token = _tokenHandler.CreateToken(existUser);
                var loginResult = new ResponseDto.LoginResult(existUser, token.ToString(), "Login Successfully");
                return _mapper.Map<ResponseDto.LoginResult>(loginResult);
        
        }

        public async Task<bool> UpdateAsync(UsersUpdateDto updateDto)
        {
            var isUpdateSuccessful = await _repository.UpdateAsync(updateDto);
            if (!isUpdateSuccessful) throw new Exception("Error while updating");

            return true;
        }
        public async Task<List<UserDto>> GetAllUsersDbAsync()
        {
            var response = await _repository.GetAllDbAsync();
            var userDto = _mapper.Map<List<UserDto>>(response);
            return userDto;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteAllAsyncElastic()
        {
            return await _repository.DeleteAllAsync();
        }

        public async Task<bool> DeleteAllAsync()
        {
            return await _repository.DeleteDatabaseAsync();
        }
        
    }
}
