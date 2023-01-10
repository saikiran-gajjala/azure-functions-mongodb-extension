namespace Peerislands.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// IMongoDBServiceFactory Interface
  /// </summary>
  public interface IMongoDBServiceFactory
  {
    /// <summary>
    /// Create MongoDB Client Wrapper instance from connection string
    /// </summary>
    public MongoDBClientWrapper CreateMongoDBClient(string connectionString);
  }
}

