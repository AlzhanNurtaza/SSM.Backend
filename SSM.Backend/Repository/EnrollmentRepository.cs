using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;


namespace SSM.Backend.Repository
{
    public class EnrollmentRepository :  Repository<Enrollment>, IEnrollmentRepository
    {
        private readonly IMongoCollection<Enrollment> _db;
        public EnrollmentRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Enrollment>(typeof(Enrollment).Name);
        }
    }
}
