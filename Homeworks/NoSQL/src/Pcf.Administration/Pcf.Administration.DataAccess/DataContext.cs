using MongoDB.Driver;

namespace Pcf.Administration.DataAccess;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, string databaseName)
    {
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName = null)
    {
        collectionName ??= typeof(T).Name.ToLower() + "s";
        return _database.GetCollection<T>(collectionName);
    }
}