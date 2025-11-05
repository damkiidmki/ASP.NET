namespace Pcf.Administration.DataAccess;

public class MongoDBSettings
{
    public MongoDBSettings()
    {
    }

    public MongoDBSettings(string connection, string databaseName)
    {
        Connection = connection;
        DatabaseName = databaseName;
    }

    public string Connection { get; set; }

    public string DatabaseName { get; set; }
}