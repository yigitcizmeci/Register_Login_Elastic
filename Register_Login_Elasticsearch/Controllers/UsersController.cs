using Microsoft.AspNetCore.Mvc;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;
using Register_Login_Elasticsearch.Services;

namespace Register_Login_Elasticsearch.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _userService.GetByIdAsync(id));
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> CreateUser(UserCreateDto userCreateDto)
        {
            return Ok(await _userService.CreateAsync(userCreateDto));
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginUser(UserLoginDto userLoginDto)
        {
            return Ok(await _userService.LoginAsync(userLoginDto));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUser(UsersUpdateDto usersUpdateDto)
        {
            return Ok(await _userService.UpdateAsync(usersUpdateDto));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return Ok(await _userService.DeleteAsync(id));
        }

    }
}
