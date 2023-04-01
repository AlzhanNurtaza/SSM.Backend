using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repositoty.IRepository;


namespace SSM.Backend.Repositoty
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
