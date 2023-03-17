using BaSyx.API.Clients;

using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace Basyx.API.Tests.Clients;

public abstract class StorageClientTestSuite : IDisposable
{
    private readonly IStorageClient<TestObject> storageClient;

    protected readonly string storageName = "teststorage";
    protected readonly string collectionName = "testcollection";
    private Dictionary<string, TestObject> entries = new();

    public StorageClientTestSuite()
    {
        // Before (SetUp)
        storageClient = GetStorageClient();
    }

    protected abstract IStorageClient<TestObject> GetStorageClient();

    protected void celanUpStorage()
    {
        entries.ToList().ForEach(entry => storageClient.Delete(entry.Key));
        storageClient.DeleteCollection();
        storageClient.DeleteStorage();
    }
 
    public void Dispose()
    {
        // After (CleanUp)
        celanUpStorage();
    }

    [Fact]
    public void GetStorageName()
    {
        string expectedStorageName = storageName;

        string actualStorageName = storageClient.GetStorageName().Entity;

        Assert.Equal(expectedStorageName, actualStorageName);
    }

    [Fact]
    public void GetCollectionName()
    {
        string expectedCollectionName = collectionName;

        string actualCollectionName = storageClient.GetCollection().Entity;

        Assert.Equal(expectedCollectionName, actualCollectionName);
    }

    [Fact]
    public void CreateOrUpdateAndRetrieve()
    {
        var key = "test";
        TestObject testObject = new() { TestValue = "test" };

        entries.Add(key, testObject);
        IResult<TestObject> createdTestObject = storageClient.CreateOrUpdate(key, entries[key]);
        
        TestObject expected = entries[key];
        TestObject actualCreated = createdTestObject.Entity;
        Assert.Equal(expected, actualCreated);

        IResult<object> remoteTestObject = storageClient.Retrieve(key);
        object actualRetrieved = remoteTestObject.Entity;
        Assert.Equal(expected, actualRetrieved);

        testObject.TestValue = "updateTest";
        entries[key] = testObject;
        storageClient.CreateOrUpdate(key, entries[key]);
        IResult<TestObject> remoteUpdatedObject = storageClient.Retrieve(key);
        TestObject updatedExpected = entries[key];
        TestObject updatedActual = remoteUpdatedObject.Entity;
        Assert.Equal(updatedExpected, updatedActual);
    }

    [Fact]
    public void RetrieveAll()
    {
        var key0 = "test0";
        var key1 = "test1";
        TestObject testObject0 = new() { TestValue = "test0" };
        TestObject testObject1 = new() { TestValue = "test1" };
        entries.Add(key0, testObject0);
        entries.Add(key1, testObject1);
        storageClient.CreateOrUpdate(key0, entries[key0]);
        storageClient.CreateOrUpdate(key1, entries[key1]);

        List<TestObject> expecteds = entries.ToList().Select(entry => entry.Value).ToList();
        List<TestObject> actuals = storageClient.RetrieveAll().Entity;

        expecteds.ForEach(entry => Assert.Contains(entry, actuals));
    }

    [Fact]
    public void RetrieveMultiple()
    {
        var key0 = "test0";
        var key1 = "test1";
        TestObject testObject0 = new() { TestValue = "test0" };
        TestObject testObject1 = new() { TestValue = "test1" };
        entries.Add(key0, testObject0);
        entries.Add(key1, testObject1);
        storageClient.CreateOrUpdate(key0, entries[key0]);
        storageClient.CreateOrUpdate(key1, entries[key1]);

        List<TestObject> expecteds = entries.ToList().Select(entry => entry.Value).ToList();
        List<TestObject> actuals = storageClient.RetrieveMultiple(entries.ToList().Select(entry => entry.Key).ToList()).Entity;

        expecteds.ForEach(entry => Assert.Contains(entry, actuals));
    }

    [Fact]
    public void Delete()
    {
        var key = "test";
        TestObject testObject = new() { TestValue = "test" };

        storageClient.CreateOrUpdate(key, testObject);
        IResult<List<TestObject>> beforeDelete = storageClient.RetrieveAll();

        Assert.Single(beforeDelete.Entity);

        IResult deleted = storageClient.Delete(key);
        Assert.True(deleted.Success);

        IResult<List<TestObject>> afterDelete = storageClient.RetrieveAll();
        Assert.Empty(afterDelete.Entity);
    }
}
