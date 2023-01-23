namespace Custom.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Context class for MongoDB trigger.
  /// </summary>
  public class MongoDBTriggerContext
  {
    public MongoDBTriggerContext(MongoDBTriggerAttribute triggerAttribute, BaseClientWrapper mongoClient)
    {
      this.TriggerAttribute = triggerAttribute;
      this.MongoClient = mongoClient;
    }

    public MongoDBTriggerAttribute TriggerAttribute { get; private set; }

    public BaseClientWrapper MongoClient { get; private set; }
  }
}