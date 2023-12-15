using AutoMapper;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Repositories;
using Register_Login_Elasticsearch.Services.Contracts;
using System.Collections.Immutable;

namespace Register_Login_Elasticsearch.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(UserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(UserCreateDto userCreateDto)
        {
            var userToCreate = _mapper.Map<Users>(userCreateDto);
            var createdUser = await _repository.CreateAsync(userToCreate);
            var userDto = _mapper.Map<UserDto>(createdUser);

            return userDto;
        }

        public async Task<ImmutableList<UserDto>> GetAllAsync()
        {
            var allUsers = await _repository.GetAllAsync();
            var userDtos = _mapper.Map<ImmutableList<UserDto>>(allUsers);

            return userDtos;
        }


        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var getById = await _repository.GetByIdAsync(id);
            var userDtos = _mapper.Map<UserDto>(getById);

            return userDtos;
        }

        public Task<UserLoginDto> LoginAsync(UserLoginDto loginDto)
        {
            return Task.FromResult(loginDto);
        }

        public async Task<UserDto?> UpdateAsync(UsersUpdateDto updateDto)
        {
            var updateUser = await _repository.UpdateAsync(updateDto);
            var updatedUser = _mapper.Map<UserDto>(updateUser);

            return updatedUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
