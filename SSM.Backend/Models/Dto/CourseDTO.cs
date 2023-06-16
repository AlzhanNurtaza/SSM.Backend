namespace SSM.Backend.Models.Dto
{
    public class CourseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CreditCount { get; set; }
        public Department Department { get; set; }
        public string Code { get; set; }
    }
}
