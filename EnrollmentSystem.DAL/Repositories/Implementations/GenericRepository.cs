// File: EnrollmentSystem.DAL/Repositories/Implementations/GenericRepository.cs
using System.Linq.Expressions;
using EnrollmentSystem.DAL.Data;
using EnrollmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.DAL.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly EnrollmentSystemDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(EnrollmentSystemDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);

    public virtual async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        => await _dbSet.AddRangeAsync(entities);

    public virtual void Update(T entity)
        => _dbSet.Update(entity);

    public virtual void Remove(T entity)
        => _dbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<T> entities)
        => _dbSet.RemoveRange(entities);

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate is null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);

    public virtual IQueryable<T> Query()
        => _dbSet.AsQueryable();

    public virtual async Task<int> SaveAsync()
        => await _context.SaveChangesAsync();
}