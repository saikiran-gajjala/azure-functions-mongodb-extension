using Moq;
using MongoDB.Driver;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;

namespace Hackathon.Azure.Functions.Extension.MongoDB.UnitTests;

public class CosmosDBDBClientWrapperTests
{
  private readonly BaseClientWrapper clientWrapper;
  private readonly Mock<IMongoClient> mockMongoClient;
  private readonly Mock<IMongoDatabase> mockMongoDatabase;
  private readonly Mock<IMongoCollection<BsonDocument>> mockMongoCollection;
  private const string ConnectionString = "TestConnectionString";
  private const string SampleMatchStage = "{\"$and\":[{\"fullDocument.amount\":100},{\"operationType\":{\"$in\":[\"insert\",\"update\",\"replace\"]}}]}";

  public CosmosDBDBClientWrapperTests()
  {
    this.mockMongoCollection = new Mock<IMongoCollection<BsonDocument>>();
    this.mockMongoDatabase = new Mock<IMongoDatabase>();
    this.mockMongoClient = new Mock<IMongoClient>();
    this.mockMongoDatabase.Setup(x => x.GetCollection<BsonDocument>("TestCollection", null)).Returns(this.mockMongoCollection.Object);
    this.mockMongoClient.Setup(x => x.GetDatabase("TestDatabase", null)).Returns(this.mockMongoDatabase.Object);
    this.clientWrapper = new CosmosDBClientWrapper(this.mockMongoClient.Object, NullLogger.Instance);
  }

  [Fact]
  public void Watch_SingleCollectionInSingleDatabaseWithBasicMatchStageForInsertOperation_VerifyWatchCall()
  {
    // Arrange
    var database = "TestDatabase";
    var collection = "TestCollection";
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoCollection.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB);
    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoCollection.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  [Fact]
  public void Watch_SingleCollectionInSingleDatabaseWithInputMatchStageForUpdateOperation_VerifyWatchCall()
  {
    // Arrange
    var database = "TestDatabase";
    var collection = "TestCollection";
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoCollection.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB, SampleMatchStage);

    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoCollection.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  [Fact]
  public void Watch_AllCollectionsInSingleWithBasicMatchStageForInsertOperation_VerifyWatchCall()
  {
    // Arrange
    var database = "TestDatabase";
    string? collection = null;
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoDatabase.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB);
    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoDatabase.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  [Fact]
  public void Watch_AllCollectionsInSingleDatabaseWithInputMatchStageForUpdateOperation_VerifyWatchCall()
  {
    // Arrange
    var database = "TestDatabase";
    string? collection = null;
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoDatabase.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB, SampleMatchStage);
    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoDatabase.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  [Fact]
  public void Watch_AllDatabasesAllCollectionsWithBasicMatchStageForInsertOperation_VerifyWatchCall()
  {
    // Arrange
    string? database = null;
    string? collection = null;
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoClient.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB);
    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoClient.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  [Fact]
  public void Watch_AllDatabasesAllCollectionsWithInputMatchStageForUpdateOperation_VerifyWatchCall()
  {
    // Arrange
    string? database = null;
    string? collection = null;
    var isCosmosDB = true;
    var changeStreamCursorMock = this.FetchMockCursor();
    this.mockMongoClient.Setup(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>())).Returns(changeStreamCursorMock.Object);
    var attribute = new MongoDBTriggerAttribute(ConnectionString, database, collection, isCosmosDB, SampleMatchStage);
    // Act
    this.clientWrapper.Watch(attribute, x =>
    {
    }, default(CancellationToken));

    // Assert
    this.mockMongoClient.Verify(x => x.Watch(
                                        It.IsAny<PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument>>(),
                                        It.IsAny<ChangeStreamOptions>(),
                                        It.IsAny<CancellationToken>()), Times.Exactly(1));
  }

  private Mock<IChangeStreamCursor<BsonDocument>> FetchMockCursor()
  {
    var changeStreamCursorMock = new Mock<IChangeStreamCursor<BsonDocument>>();
    var backingDocument = this.FetchMockBackingDocument();
    var documents = new List<BsonDocument>() { backingDocument };
    changeStreamCursorMock.Setup(x => x.Current).Returns(documents);
    return changeStreamCursorMock;
  }

  private BsonDocument FetchMockBackingDocument()
  {
    var fullDocument = new BsonDocument();
    fullDocument.Add(new BsonElement("_id", new ObjectId()));
    fullDocument.Add(new BsonElement("userAccountId", 123));
    fullDocument.Add(new BsonElement("receiverAccountId", 456));
    fullDocument.Add(new BsonElement("reason", "Rent"));
    fullDocument.Add(new BsonElement("amount", 10000));
    return new BsonDocument("fullDocument", fullDocument);
  }
}