﻿namespace SSM.Backend.Models.Dto
{
    public class ScheduleDTO
    {
        public string Id { get; set; }
        //public Enrollment Enrollment { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //public Classroom Classroom { get; set; }
        public string Subject { get; set; }
        public bool IsAllDay { get; set; } = false;
        public string? Description { get; set; } = string.Empty;
        public string? RecurrenceRule { get; set; }
    }
}
