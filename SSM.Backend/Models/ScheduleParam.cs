

using SSM.Backend.Models.Dto;

namespace SSM.Backend.Models
{
    public class ScheduleParam
    {
        public string? key { get; set; }
        public string? action { get; set; }
        public List<ScheduleCreateDTO>? added { get; set; }
        public List<Schedule>? changed { get; set; } = new List<Schedule>();
        public List<Schedule>? deleted { get; set; } = new List<Schedule>();
        public Schedule? value { get; set; } = null;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        
    }
}
