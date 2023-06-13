using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SSM.Backend.Models
{
    public class Classroom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
    }
}
