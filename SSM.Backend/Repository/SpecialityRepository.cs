using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;


namespace SSM.Backend.Repository
{
    public class SpecialityRepository :  Repository<Speciality>, ISpecialityRepository
    {
        private readonly IMongoCollection<Speciality> _db;
        public SpecialityRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Speciality>(typeof(Speciality).Name);
        }
    }
}
