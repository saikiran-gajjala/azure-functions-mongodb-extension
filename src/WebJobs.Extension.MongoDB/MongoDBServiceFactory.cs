namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBServiceFactory : IMongoDBServiceFactory
  {
    public MongoDBClientWrapper CreateMongoDBClient(string connectionString)
    {
      return new MongoDBClientWrapper(connectionString);
    }
  }
}

