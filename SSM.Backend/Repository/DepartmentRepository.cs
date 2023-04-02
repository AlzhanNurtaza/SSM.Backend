using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;


namespace SSM.Backend.Repository
{
    public class DepartmentRepository :  Repository<Department>, IDepartmentRepository
    {
        private readonly IMongoCollection<Department> _db;
        public DepartmentRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Department>(typeof(Department).Name);
        }
    }
}
