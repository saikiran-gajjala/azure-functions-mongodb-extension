namespace Hackathon.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// IMongoDBServiceFactory Interface
  /// </summary>
  public interface IMongoDBServiceFactory
  {
    /// <summary>
    /// Create MongoDB Client Wrapper instance from connection string
    /// </summary>
    public BaseClientWrapper CreateMongoDBClient(string connectionString, bool isCosmosDB);
  }
}

