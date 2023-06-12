using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SSM.Backend.Models;
using System.Linq.Expressions;
using System.Reflection;
using static SSM.Backend.Repository.IRepository.IRepository;

namespace SSM.Backend.Repository
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

        public async Task<List<T>> GetAllAsync(int _start = 0, int _end = 25, string? filterMain="", string? filterAuto = "")
        {
            if(_end > 100)
            {
                _end = 100;
            }
            var filters = Builders<T>.Filter.Empty;
            if (filterMain!= string.Empty)
            {
                string[] array = filterMain.Split('=');
                filters = Builders<T>.Filter.Regex(array[0], new BsonRegularExpression($".*{array[1]}.*", "i"));
            }
            if (filterAuto != string.Empty)
            {
                string[] array = filterAuto.Split('=');
                filters = Builders<T>.Filter.Regex(array[0], new BsonRegularExpression($".*{array[1]}.*", "i"));
            }

            return await _collection.Find(filters).Skip(_start).Limit(_end).ToListAsync();
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

        public async Task<bool> IsUnique(string fieldName, string value, string? id = "")
        {
            var result = true;
            var filter = Builders<T>.Filter.Eq(fieldName, value);
            if(!string.IsNullOrEmpty(id))
            {
                filter = filter & Builders<T>.Filter.Ne("Id", id);
            }
            long count = await _collection.Find(filter).CountAsync();
            if(count > 0)
            {
                result = false;
            }
            return result;
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
