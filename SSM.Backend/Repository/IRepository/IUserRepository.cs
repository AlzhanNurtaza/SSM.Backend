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
        Task CreateRoleAsync(string roleName);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    }
}
