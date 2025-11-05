using MongoDB.Bson;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pcf.Administration.DataAccess.Repositories;

public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;
    private readonly MongoDbContext _context;

    public MongoRepository(MongoDbContext context)
    {
        _context = context;
        _collection = _context.GetCollection<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = new BsonDocument("_id", id.ToString());
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
    {
        return await _collection.Find(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.DeleteOneAsync(filter);
    }
}