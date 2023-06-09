using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    }
}
