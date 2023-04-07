using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;
using SSM.Backend.Repository.IRepository;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace SSM.Backend.Controllers
{
    [Route("api/userAuth")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserAuthController(IUserRepository userRepo, UserManager<ApplicationUser> userManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserNameUnique = await _userRepo.IsUniqueUserAsync(model.Email);
            if (!ifUserNameUnique)
            {
                return BadRequest(new { message = "Current user already exists" });
            }
            var confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/userAuth/ConfirmEmail";
            var result = await _userRepo.RegisterAsync(model, confirmationUrl, model.ReturnUrl);
            if (result == null)
            {
                return BadRequest(new { message = "Cannot register user" });
            }
            return Ok(result);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.LoginAsync(model);
            return Ok(loginResponse);
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
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string returnUrl)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(decodedToken));

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            return BadRequest(result.Errors);
        }

    }
}
