using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;


namespace SSM.Backend.Repository
{
    public class CourseRepository :  Repository<Course>, ICourseRepository
    {
        private readonly IMongoCollection<Department> _db;
        public CourseRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Department>(typeof(Course).Name);
        }
    }
}
