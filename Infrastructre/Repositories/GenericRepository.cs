using Application.Repositories;
using Infrastructre.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructre.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BSFContext _dbContext;
        public GenericRepository(BSFContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task InsertAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }
        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }
        public void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _dbContext.Set<T>().Attach(entity);
            }

            entry.State = EntityState.Modified;
        }
        public void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbContext.Set<T>().Attach(entity);

                entry.State = EntityState.Modified;
            }
        }
        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
    }
}
