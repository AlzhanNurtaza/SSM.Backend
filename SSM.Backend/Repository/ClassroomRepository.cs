using MongoDB.Bson;
using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Repository
{
    public class ClassroomRepository :  Repository<Classroom>, IClassroomRepository
    {
        private readonly IMongoCollection<Classroom> _db;
        public ClassroomRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Classroom>(typeof(Classroom).Name);
        }

        public async Task<List<Classroom>> GetAllClassroomsAsync(int _start = 0, int _end = 100, string? filterFrontEnd = "", string? filterMain = "", string? filterAuto = "")
        {
            if (_end > 100)
            {
                _end = 100;
            }
            var filters = Builders<Classroom>.Filter.Empty;
            if (filterMain != string.Empty)
            {
                string[] array = filterMain.Split('=');
                filters &= Builders<Classroom>.Filter.Regex(array[0], new BsonRegularExpression($".*{array[1]}.*", "i"));
            }
            if (filterAuto != string.Empty)
            {
                string[] array = filterAuto.Split('=');
                filters &= Builders<Classroom>.Filter.Regex(array[0], new BsonRegularExpression($".*{array[1]}.*", "i"));
            }
            if (filterFrontEnd != string.Empty)
            {
                string[] array = filterFrontEnd.Split(' ');
                filters &= Builders<Classroom>.Filter.Regex("Name", new BsonRegularExpression($".*{array[2].Replace("'","")}.*", "i"));
            }

                return await _db.Find(filters).Skip(_start).Limit(_end).ToListAsync();
        }
    }
}
