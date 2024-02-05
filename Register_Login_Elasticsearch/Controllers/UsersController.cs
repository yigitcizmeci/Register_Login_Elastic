using Microsoft.AspNetCore.Mvc;
using Nest;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Services;

namespace Register_Login_Elasticsearch.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ElasticClient _client;

        public UsersController(UserService userService, ElasticClient client)
        {
            _userService = userService;
            _client = client;
            _client = _client ?? throw new ArgumentNullException(nameof(_client));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }
        [HttpGet("{_id}")]
        public async Task<IActionResult> GetById(string _id)
        {
            return Ok(await _userService.GetByIdAsync(_id));
        }

        [HttpGet("UserLogs/{userid}")]
        public IActionResult GetUserLogsById(string userid)
        {
            return Ok(_userService.GetUserLogs(userid));
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
        public async Task<IActionResult> DeleteUser(string id)
        {
            return Ok(await _userService.DeleteAsync(id));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllUser()
        {
            return Ok(await _userService.DeleteAllAsync());
        }
        [HttpGet]
        [Route("All_Information")]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _userService.GetAsync());
        }
    }
}
