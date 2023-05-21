namespace SSM.Backend.Models.Dto
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string IIN { get; set; }
        public Department Department { get; set; }
        public string Image { get; set; }
    }
}
