using Microsoft.AspNetCore.Components.Web;

namespace SSM.Backend.Models.Dto
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
