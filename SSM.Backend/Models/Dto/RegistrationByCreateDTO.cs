using System.ComponentModel.DataAnnotations;

namespace SSM.Backend.Models.Dto
{
    public class RegistrationByCreateDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string IIN { get; set; }
        public Department Department { get; set; }
        public string Image { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; }
    }
}
