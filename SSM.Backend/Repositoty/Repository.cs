using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SSM.Backend.Models;
using System.Linq.Expressions;
using System.Reflection;
using static SSM.Backend.Repositoty.IRepository.IRepository;

namespace SSM.Backend.Repositoty
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection; 
        public Repository(IMongoDatabase db)
        {
            var name = typeof(T).Name;
            _collection = db.GetCollection<T>(name);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<T> GetAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            T result = await _collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<T>> GetAllAsync(int _start = 0, int _end = 1)
        {
            if(_end > 100)
            {
                _end = 100;
            }
            var filter = Builders<T>.Filter.Empty;
            return await _collection.Find(filter).Skip(_start).Limit(_end).ToListAsync();
        }

        public async Task RemoveAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<long> GetCount()
        {
            var filter = Builders<T>.Filter.Empty;
            return await _collection.Find(filter).CountDocumentsAsync();
        }

        public async Task<T> UpdateAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var options = new FindOneAndReplaceOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            };
            T result = await _collection.FindOneAndReplaceAsync<T>(filter, entity,options);
            return result;
        }
    }
}
