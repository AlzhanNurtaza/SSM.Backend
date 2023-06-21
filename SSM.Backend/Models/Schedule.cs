using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SSM.Backend.Models
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string EnrollmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ClassroomId { get; set; }
        public string Subject { get; set; }
        public bool IsAllDay { get; set; } = false;
        public string? Description { get; set; } = string.Empty;
        public string? RecurrenceRule { get; set; }
        public string EnrollmentName { get; set; }
        public string GroupsName{ get; set; }
        public int StudentCount { get; set; }
        public string CourseName { get; set; }
        public int SeatCount { get; set; }
        public string InstructorName { get; set; }
    }
}
