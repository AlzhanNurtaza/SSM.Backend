using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SSM.Backend.Models.Dto;
using SSM.Backend.Models;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using SSM.Backend.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace SSM.Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper
            , SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _signInManager = signInManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret")?? string.Empty;
        }

        public async Task<bool> IsUniqueUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task CreateRoleAsync(string roleName)
        {
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }





        public async Task<UserDTO> RegisterAsync(RegistrationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.Email,
                Email = registerationRequestDTO.Email,
                NormalizedEmail = registerationRequestDTO.Email.ToUpper(),
                NormalizedUserName = registerationRequestDTO.Name.ToUpper()
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("Student").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new ApplicationRole { Name= "Student" });
                    }
                    await _userManager.AddToRoleAsync(user, "Student");
                    var userToReturn = _userManager.FindByEmailAsync(registerationRequestDTO.Email);
                    return _mapper.Map<UserDTO>(userToReturn);

                }
            }
            catch (Exception e)
            {

            }

            return new UserDTO();
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            ApplicationUser appUser = await _userManager.FindByEmailAsync(loginRequestDTO.Email) ;
            if (appUser != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(appUser, loginRequestDTO.Password, false, false);
                if (!result.Succeeded)
                {
                    return new LoginResponseDTO()
                    {
                        Token = "",
                        User = null,
                        Success = false
                    };
                }

                //if user was found generate JWT Token
                var roles = await _userManager.GetRolesAsync(appUser);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, appUser.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    Token = tokenHandler.WriteToken(token),
                    User = _mapper.Map<UserDTO>(appUser),
                    Success= true

                };
                return loginResponseDTO;

            }
            return new LoginResponseDTO()
            {
                Token = "",
                User = null,
                Success = false
            };

           
        }



    }
}
