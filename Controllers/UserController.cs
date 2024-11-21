using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims;
using MongoDB.Bson;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetMyUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }

            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                return BadRequest("Id de usuário inválido.");
            }

            var user = await _userService.GetUserByIdAsync(objectId.ToString());

            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                Number = user.Number,
                City = user.City,
                ImageUrl = user.ImageUrl,
                Phone = user.Phone,
                IsAdmin = user.IsAdmin 
            };

            return Ok(userDto);
        }

        [HttpGet]
        [Authorize]  
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, User updateUser)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            updateUser.Id = user.Id;
            await _userService.UpdateUserAsync(id, updateUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || user.Password != loginRequest.Password)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            var token = _userService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}
