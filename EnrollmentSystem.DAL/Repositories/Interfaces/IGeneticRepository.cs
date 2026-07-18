// File: EnrollmentSystem.DAL/Repositories/Interfaces/IGenericRepository.cs
using System.Linq.Expressions;

namespace EnrollmentSystem.DAL.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>Escape hatch for advanced/composed queries (Include, projections, paging).</summary>
    IQueryable<T> Query();

    /// <summary>Commits pending changes for the shared DbContext instance.</summary>
    Task<int> SaveAsync();
}