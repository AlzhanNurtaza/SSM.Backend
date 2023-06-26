using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SSM.Backend.Models.Dto;

namespace SSM.Backend.Models
{
    public class Enrollment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<Group> Groups { get; set; }
        public Course Course { get; set; }
        public UserDTO Instructor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Term{ get; set; }
        public string Name { get; set; }
        public int StudyCount { get; set; }
    }
}
