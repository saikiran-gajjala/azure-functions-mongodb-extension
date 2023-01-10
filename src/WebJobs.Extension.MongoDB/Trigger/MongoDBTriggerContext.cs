namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Context class for MongoDB trigger
  /// </summary>
  public class MongoDBTriggerContext
  {
    public MongoDBTriggerContext(MongoDBTriggerAttribute triggerAttribute, MongoDBClientWrapper mongoClient)
    {
      this.TriggerAttribute = triggerAttribute;
      this.MongoClient = mongoClient;
    }

    public MongoDBTriggerAttribute TriggerAttribute { get; private set; }
    public MongoDBClientWrapper MongoClient { get; private set; }
  }
}

