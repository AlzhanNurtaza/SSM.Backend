using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SSM.Backend.Models.Dto;

namespace SSM.Backend.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int CreditCount { get; set; }
        public Department Department { get; set; }
        public string Code { get; set; }
    }
}
