using MongoDB.Bson;
using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;

namespace SSM.Backend.Repository
{
    public class ScheduleRepository :  Repository<Schedule>, IScheduleRepository
    {
        private readonly IMongoCollection<Schedule> _db;
        public ScheduleRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Schedule>(typeof(Schedule).Name);
        }

       public async Task<List<Schedule>> GetAllScheduleAsync(DateTime? startDate, DateTime? endDate)
        {
            var filter = Builders<Schedule>.Filter.Empty;
            if (startDate != null || endDate != null)
            {
                filter = Builders<Schedule>.Filter.And(
                Builders<Schedule>.Filter.Gte("StartTime", startDate),
                Builders<Schedule>.Filter.Lte("StartTime", endDate)
            );
            }
            
            return await _db.Find(filter).ToListAsync();
        }
    }
}
