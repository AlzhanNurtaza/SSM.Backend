using Microsoft.AspNetCore.Identity;
using SSM.Backend.Models.Dto;

namespace SSM.Backend.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUserAsync(string username);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<IdentityResult> RegisterAsync(RegistrationRequestDTO registerationRequestDTO, string confirmationUrl, string returnUrl);
        Task CreateRoleAsync(string roleName);
    }
}
