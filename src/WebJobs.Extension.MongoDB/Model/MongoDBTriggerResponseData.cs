using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDB Trigger event data model. 
  /// </summary>
  public class MongoDBTriggerEventData
  {
    //
    // Summary:
    //     Gets the namespace of the collection.
    //
    // Value:
    //     The namespace of the collection.
    public CollectionNamespace CollectionNamespace { get; internal set; }

    //
    // Summary:
    //     Gets ui field from the oplog entry corresponding to the change event. Only present
    //     when the showExpandedEvents change stream option is enabled and for the following
    //     event types (MongoDB 6.0 and later):
    //     • MongoDB.Driver.ChangeStreamOperationType.Create
    //     • MongoDB.Driver.ChangeStreamOperationType.CreateIndexes
    //     • MongoDB.Driver.ChangeStreamOperationType.Delete
    //     • MongoDB.Driver.ChangeStreamOperationType.Drop
    //     • MongoDB.Driver.ChangeStreamOperationType.DropIndexes
    //     • MongoDB.Driver.ChangeStreamOperationType.Insert
    //     • MongoDB.Driver.ChangeStreamOperationType.Modify
    //     • MongoDB.Driver.ChangeStreamOperationType.RefineCollectionShardKey
    //     • MongoDB.Driver.ChangeStreamOperationType.ReshardCollection
    //     • MongoDB.Driver.ChangeStreamOperationType.ShardCollection
    //     • MongoDB.Driver.ChangeStreamOperationType.Update
    //
    // Value:
    //     The UUID of the collection.
    public Guid? CollectionUuid { get; internal set; }

    //
    // Summary:
    //     Gets the database namespace.
    //
    // Value:
    //     The database namespace.
    public DatabaseNamespace DatabaseNamespace { get; internal set; }

    //
    // Summary:
    //     Gets the document key.
    //
    // Value:
    //     The document key.
    public BsonDocument DocumentKey { get; internal set; }

    //
    // Summary:
    //     Gets the full document.
    //
    // Value:
    //     The full document.
    public BsonDocument FullDocument { get; internal set; }

    //
    // Summary:
    //     Gets the full document before change.
    //
    // Value:
    //     The full document before change.
    public BsonDocument FullDocumentBeforeChange { get; internal set; }

    //
    // Summary:
    //     Gets the type of the operation.
    //
    // Value:
    //     The type of the operation.
    public ChangeStreamOperationType OperationType { get; internal set; }

    //
    // Summary:
    //     Gets the resume token.
    //
    // Value:
    //     The resume token.
    public BsonDocument ResumeToken { get; internal set; }

    //
    // Summary:
    //     Gets the update description.
    //
    // Value:
    //     The update description.
    public ChangeStreamUpdateDescription UpdateDescription { get; internal set; }

    //
    // Summary:
    //     Gets the wall time of the change stream event.
    //
    // Value:
    //     The wall time.
    public DateTime? WallTime { get; internal set; }
  }
}