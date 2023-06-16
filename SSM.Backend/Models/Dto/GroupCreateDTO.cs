namespace SSM.Backend.Models.Dto
{
    public class GroupCreateDTO
    {
        public int GroupNumber { get; set; }
        public int StartYear { get; set; }
        public Speciality Speciality { get; set; }
        public string Name { get; set; }
        public List<UserDTO> Students { get; set; }

    }
}
