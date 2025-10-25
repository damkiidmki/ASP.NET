using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(params object[] keys);

    Task<TEntity?> CreateAsync(TEntity entity);

    Task<TEntity?> UpdateAsync(TEntity entity);

    Task<bool> DeleteAsync(params object[] keys);

    Task<bool> DeleteAsync(TEntity entity);

    Task<bool> ExistsAsync(params object[] keys);

    IQueryable<TEntity> GetQueryable();

    Task<IEnumerable<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities);

    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);

    Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities);

    Task<bool> DeleteRangeAsync(IEnumerable<object[]> keys);

    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object[]> keys);

    Task<int> GetCountAsync();
}