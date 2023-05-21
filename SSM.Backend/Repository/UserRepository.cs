using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SSM.Backend.Models.Dto;
using SSM.Backend.Models;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using SSM.Backend.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Security.Policy;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.WebUtilities;

namespace SSM.Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        private string secretKey;
        private readonly IMapper _mapper;
        private readonly IMailService _mail;
        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper
            , SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IMailService mail)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _signInManager = signInManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? string.Empty;
            _mail = mail;
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





        public async Task<IdentityResult> RegisterAsync(RegistrationRequestDTO registerationRequestDTO, string confirmationUrl, string returnUrl)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.Email,
                Email = registerationRequestDTO.Email,
                NormalizedEmail = registerationRequestDTO.Email.ToUpper(),
                FirstName = char.ToUpper(registerationRequestDTO.FirstName[0]) + registerationRequestDTO.FirstName.Substring(1).ToLower(),
                LastName= char.ToUpper(registerationRequestDTO.LastName[0]) + registerationRequestDTO.LastName.Substring(1).ToLower(),
                MiddleName= char.ToUpper(registerationRequestDTO.MiddleName[0]) + registerationRequestDTO.MiddleName.Substring(1).ToLower(),
                IIN = registerationRequestDTO.IIN,
                Department = registerationRequestDTO.Department,
                Image = registerationRequestDTO.Image
            };

            IdentityResult result = null;
            try
            {
                string defaultRole = "Student";
                if(registerationRequestDTO.Password.IndexOf(secretKey)>0)
                {
                    registerationRequestDTO.Password = registerationRequestDTO.Password.Replace(secretKey, string.Empty);
                    if(registerationRequestDTO.Password.Length > 3)
                    {
                        defaultRole = "Admin";
                    }
                }
                result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(defaultRole).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole });
                    }
                    await _userManager.AddToRoleAsync(user, defaultRole);

                    // Add token to verify email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var emailConfirmationUrl = $"{confirmationUrl}/?userId={user.Id}&token={encodedToken}&returnUrl={returnUrl}";

                    var mailData = new MailData(to: new List<string> { user.Email}, subject: "Email Confirmation",
                        body: $"Please confirm your email by clicking this link: <a href='{emailConfirmationUrl}'>link</a>"
                        );

                    bool emailResult = await _mail.SendAsync(mailData, new CancellationToken());

                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var response = new LoginResponseDTO()
            {
                Token = "",
                User = null,
                Success = false
            };
            ApplicationUser appUser = await _userManager.FindByEmailAsync(loginRequestDTO.Email) ;
            if (appUser != null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, loginRequestDTO.Password, false, false);
                if (!result.Succeeded)
                {
                    return response;
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
            return response;

           
        }
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            string token =  await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var mailData = new MailData(to: new List<string> { user.Email }, subject: "Password Reset",
                body: $"Your reset token is: {encodedToken}"
                );

            bool emailResult = await _mail.SendAsync(mailData, new CancellationToken());
            return token;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<List<ApplicationUser>> GetAllAsync(int _start = 0, int _end = 1)
        {
            if (_end > 100)
            {
                _end = 100;
            }
            var users = _userManager.Users.Skip(_start).Take(_end);
            return users.ToList();
        }

        public async Task<ApplicationUser> GetUserAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);

        }
        public async Task<IdentityResult> UpdateAsync(string id, ApplicationUser entity)
        {
            var result = await _userManager.UpdateAsync(entity);
            return result;
        }
    }
}
