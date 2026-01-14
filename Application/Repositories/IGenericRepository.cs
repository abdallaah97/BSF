namespace Application.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int Id);
        IQueryable<T> GetAll();
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
    }
}
