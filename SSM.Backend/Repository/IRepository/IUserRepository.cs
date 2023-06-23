using Microsoft.AspNetCore.Identity;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;

namespace SSM.Backend.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUserAsync(string username);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<IdentityResult> RegisterAsync(RegistrationRequestDTO registerationRequestDTO, string confirmationUrl, string returnUrl);
        Task<IdentityResult> RegisterByCreateAsync(RegistrationByCreateDTO registrationByCreateDTO);
        Task CreateRoleAsync(string roleName);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<List<ApplicationUser>> GetAllAsync(int _start = 0, int _end = 1, string? filterMain = "", string? filterAuto = "");
        Task<ApplicationUser> GetUserAsync(string id);

        Task<IdentityResult> UpdateAsync(string id, ApplicationUser entity);
    }
}
