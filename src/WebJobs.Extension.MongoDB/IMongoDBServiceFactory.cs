namespace Azure.Functions.Extension.MongoDB
{
  public interface IMongoDBServiceFactory
  {
    public MongoDBClientWrapper CreateMongoDBClient(string connectionString);
  }
}

