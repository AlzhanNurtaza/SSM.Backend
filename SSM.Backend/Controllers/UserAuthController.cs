using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;
using System.Net;

namespace SSM.Backend.Controllers
{
    [Route("api/userAuth")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UserAuthController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique = await _userRepo.IsUniqueUserAsync(model.Email);
            if (!ifUserNameUnique)
            {
                return BadRequest(new { message = "Current user already exists" });
            }

            var user = await _userRepo.RegisterAsync(model);
            if (user == null)
            {
                return BadRequest(new { message = "Cannot register user" });
            }
            return Ok();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.LoginAsync(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new {message = "Username or password is incorrect" });
            }
            return Ok(new { token = loginResponse.Token });
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoleAsync(string roleName)
        {
            try
            {
                await _userRepo.CreateRoleAsync(roleName);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest( new { message = $"Failed to create role [{roleName}]. {ex.Message}" } );
            }
        }
    }
}
