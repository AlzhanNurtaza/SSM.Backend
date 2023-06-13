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
    }
}
