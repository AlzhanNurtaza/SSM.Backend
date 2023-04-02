using SSM.Backend.Models.Dto;

namespace SSM.Backend.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUserAsync(string username);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> RegisterAsync(RegistrationRequestDTO registerationRequestDTO);
        Task CreateRoleAsync(string roleName);
    }
}
