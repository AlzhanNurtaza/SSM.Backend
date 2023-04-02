using MongoDB.Driver;
using System.Linq.Expressions;

namespace SSM.Backend.Repository.IRepository
{
    public interface IRepository
    {
        public interface IRepository<T> where T : class
        {
            Task<List<T>> GetAllAsync(int _start = 0, int _end = 1);
            Task<long> GetCount();
            Task<T> GetAsync(string id);
            Task<T> CreateAsync(T entity);
            Task RemoveAsync(string id);
            Task<T> UpdateAsync(string id, T entity);
        }
    }
}
