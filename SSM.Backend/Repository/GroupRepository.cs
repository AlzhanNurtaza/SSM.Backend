using MongoDB.Driver;
using SSM.Backend.Models;
using SSM.Backend.Repository.IRepository;


namespace SSM.Backend.Repository
{
    public class GroupRepository :  Repository<Group>, IGroupRepository
    {
        private readonly IMongoCollection<Group> _db;
        public GroupRepository(IMongoDatabase db) : base(db)
        {
            _db = db.GetCollection<Group>(typeof(Group).Name);
        }
    }
}
