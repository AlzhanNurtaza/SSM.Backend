namespace SSM.Backend.Models.Dto
{
    public class CourseCreateDTO
    {
        public string Name { get; set; }
        public int CreditCount { get; set; }
        public Department Department { get; set; }
        public string Code { get; set; }
    }
}
