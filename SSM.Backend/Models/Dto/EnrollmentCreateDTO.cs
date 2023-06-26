namespace SSM.Backend.Models.Dto
{
    public class EnrollmentCreateDTO
    { 
        public List<Group> Groups { get; set; }
        public Course Course { get; set; }
        public UserDTO Instructor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Term { get; set; }
        public string Name { get; set; }
        public int StudyCount { get; set; }
    }
}
