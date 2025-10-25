using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.DataAccess;

public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public EFRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(params object[] keys)
    {
        return await _dbSet.FindAsync(keys);
    }

    public async Task<TEntity?> CreateAsync(TEntity entity)
    {
        var newEntity = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return newEntity.Entity;
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        var updatedEntity = _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return updatedEntity.Entity;
    }

    public async Task<bool> DeleteAsync(params object[] keys)
    {
        var entity = await GetByIdAsync(keys);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(params object[] keys)
    {
        var entity = await GetByIdAsync(keys);
        return entity != null;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object[]> keys)
    {
        var results = new List<TEntity>();

        foreach (var key in keys)
        {
            var entity = await GetByIdAsync(key);
            if (entity != null)
                results.Add(entity);
        }

        return results;
    }

    public async Task<IEnumerable<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
    {
        var entityList = entities.ToList();
        await _dbSet.AddRangeAsync(entityList);
        await _context.SaveChangesAsync();
        return entityList;
    }

    public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var entityList = entities.ToList();
        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();
        return entityList;
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        var entityList = entities.ToList();
        if (!entityList.Any())
            return true;

        _dbSet.RemoveRange(entityList);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<object[]> keys)
    {
        var entitiesToDelete = new List<TEntity>();

        foreach (var key in keys)
        {
            var entity = await GetByIdAsync(key);
            if (entity != null)
                entitiesToDelete.Add(entity);
        }

        if (!entitiesToDelete.Any())
            return false;

        _dbSet.RemoveRange(entitiesToDelete);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}