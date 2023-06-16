using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SSM.Backend.Models.Dto;

namespace SSM.Backend.Models
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int GroupNumber { get; set; }
        public int StartYear { get; set; }
        public Speciality Speciality { get; set; }
        public string Name { get; set; }
        public List<UserDTO> Students { get; set; }
    }
}
