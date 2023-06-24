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
using MongoDB.Bson;
using static MongoDB.Driver.WriteConcern;
using System.Linq.Expressions;
using System.Data;

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
        private readonly IMongoCollection<ApplicationUser> _collection;
        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper
            , SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IMailService mail, IMongoDatabase db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _signInManager = signInManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? string.Empty;
            _mail = mail;
            _collection = db.GetCollection<ApplicationUser>("User");
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
                Image = registerationRequestDTO.Image,
                Role = "Student"
            };

            IdentityResult result = null;
            try
            {

                result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
          
                    await _userManager.AddToRoleAsync(user, user.Role);

                    // Add token to verify email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    //var emailConfirmationUrl = $"{confirmationUrl}/?userId={user.Id}&token={encodedToken}&returnUrl={returnUrl}";

                    var editors = _userManager.Users.Where(u => u.Role == "Editor"|| u.Role=="Admin").ToList();
                    foreach(var editor in editors)
                    {
                        var mailData = new MailData(to: new List<string> { editor.Email }, subject: "Подтверждение",
                            body: $"Новый пользователь бы зарегистрирован. Просьба проверить и активировать учетку.</br>" +
                            $"<a href='http://127.0.0.1:5173/users/edit/{user.Id}'>Ссылка</a>"
                            );

                        bool emailResult = await _mail.SendAsync(mailData, new CancellationToken());
                    }



                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public async Task<IdentityResult> RegisterByCreateAsync(RegistrationByCreateDTO registrationByCreateDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationByCreateDTO.Email,
                Email = registrationByCreateDTO.Email,
                NormalizedEmail = registrationByCreateDTO.Email.ToUpper(),
                FirstName = char.ToUpper(registrationByCreateDTO.FirstName[0]) + registrationByCreateDTO.FirstName.Substring(1).ToLower(),
                LastName = char.ToUpper(registrationByCreateDTO.LastName[0]) + registrationByCreateDTO.LastName.Substring(1).ToLower(),
                MiddleName = char.ToUpper(registrationByCreateDTO.MiddleName[0]) + registrationByCreateDTO.MiddleName.Substring(1).ToLower(),
                IIN = registrationByCreateDTO.IIN,
                Department = registrationByCreateDTO.Department,
                Image = registrationByCreateDTO.Image,
                EmailConfirmed = registrationByCreateDTO.EmailConfirmed,
                Role = registrationByCreateDTO.Role
            };

            IdentityResult result = null;
            try
            {

                var randomPass = GenerateRandomPassword(7) + "2023!QA";
                result = await _userManager.CreateAsync(user, randomPass);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, registrationByCreateDTO.Role);
                    // Add token to verify email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var mailData = new MailData(to: new List<string> { user.Email }, subject: "SSM",
                        body: $"Вам создана учетная запись на портале SSM <br/>" +
                        $"Учетная запись: {user.Email} </br>" +
                        $"Пароль: {randomPass}<br/>" +
                        $"При необходимости смените пароль"
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
                //var roles = await _userManager.GetRolesAsync(appUser);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, appUser.UserName.ToString()),
                    new Claim(ClaimTypes.Role, appUser.Role)
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
            //var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var mailData = new MailData(to: new List<string> { user.Email }, subject: "Password Reset",
                body: $"Токен для изменения пароля: {token}"
                );

            bool emailResult = await _mail.SendAsync(mailData, new CancellationToken());
            return token;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result;
        }

        public async Task<List<ApplicationUser>> GetAllAsync(int _start = 0, int _end = 25, string? filterMain = "", string? filterAuto = "", string? role = "")
        {
            if (_end > 100)
            {
                _end = 100;
            }
            var filters = Builders<ApplicationUser>.Filter.Empty;
            if (filterMain != string.Empty)
            {
                string[] array = filterMain.Split('=');
                filters = Builders<ApplicationUser>.Filter.Regex("LastName", new BsonRegularExpression($".*{array[1]}.*", "i"));
            }
            if (filterAuto != string.Empty)
            {
                string[] array = filterAuto.Split('=');
                filters = Builders<ApplicationUser>.Filter.Regex("LastName", new BsonRegularExpression($".*{array[1]}.*", "i"));
            }
            if (role!=string.Empty)
            {
                filters &= Builders<ApplicationUser>.Filter.Regex("Role", new BsonRegularExpression($".*{role}.*", "i"));
            }

            
            var users = await _collection.Find(filters).Skip(_start).Limit(_end).ToListAsync();
            return users;
        }

        public async Task<ApplicationUser> GetUserAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);

        }
        public async Task<IdentityResult> UpdateAsync(string id, ApplicationUser entity)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.LastName = entity.LastName;
            user.FirstName = entity.FirstName;
            user.MiddleName = entity.MiddleName;
            user.IIN = entity.IIN;
            user.Department=entity.Department;
            user.Image= entity.Image;
            user.Role=entity.Role;
            user.EmailConfirmed=entity.EmailConfirmed;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
            await _userManager.AddToRoleAsync(user,entity.Role);
            var result = await _userManager.UpdateAsync(user);
            return result;
        }
        string GenerateRandomPassword(int length)
        {
            const string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            string validChars = lowercaseLetters + uppercaseLetters + digits + specialChars;

            Random random = new Random();
            char[] password = new char[length];

            // Choose a random lowercase letter
            password[0] = lowercaseLetters[random.Next(lowercaseLetters.Length)];

            // Choose a random uppercase letter
            password[1] = uppercaseLetters[random.Next(uppercaseLetters.Length)];

            // Fill the rest of the password with random characters
            for (int i = 2; i < length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            // Shuffle the password to ensure randomness
            for (int i = length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            return new string(password);
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            
        }
    }
}
