using BaSyx.API.Clients;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Utils.ResultHandling;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Clients;

public abstract class StorageClientTestSuite : IDisposable
{
    private readonly IStorageClient<object> storageClient;

    protected readonly string storageName = "teststorage";
    protected readonly string collectionName = "testcollection";
    private Dictionary<string, object> entries = new();

    public StorageClientTestSuite()
    {
        // Before (SetUp)
        storageClient = GetStorageClient();
    }

    protected abstract IStorageClient<Object> GetStorageClient();

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
    public void GetCollection()
    {
        string expectedCollectionName = collectionName;

        string actualCollectionName = storageClient.GetCollection().Entity;

        Assert.Equal(expectedCollectionName, actualCollectionName);
    }

    [Fact]
    public void CreateOrUpdateAndRetrieve()
    {
        var key = "test";
        var testObject = new { test = "test" };
        
        entries.Add(key, testObject);
        storageClient.CreateOrUpdate(key, entries[key]);
        IResult<object> remoteTestObject = storageClient.Retrieve(key);

        Assert.Equal(entries[key], remoteTestObject.Entity);
        
        var updateTestObject = new { test = "update" };
        entries[key] = updateTestObject;
        storageClient.CreateOrUpdate(key, entries[key]);
        IResult<object> remoteUpdatedObject = storageClient.Retrieve(key);

        Assert.Equal(entries[key], remoteUpdatedObject.Entity);
    }

    [Fact]
    public void RetrieveAll()
    {
        var key0 = "test0";
        var key1 = "test1";
        var testObject0 = new { test = "test0" };
        var testObject1 = new { test = "test1" };
        entries.Add(key0, testObject0);
        entries.Add(key1, testObject1);
        storageClient.CreateOrUpdate(key0, entries[key0]);
        storageClient.CreateOrUpdate(key1, entries[key1]);

        List<Object> remoteEntries = storageClient.RetrieveAll().Entity;
        entries.ToList().ForEach(entry => Assert.Contains(entry.Value, remoteEntries));
    }

    [Fact]
    public void Delete()
    {
        var key = "test";
        var testObject = new { test = "test" };

        storageClient.CreateOrUpdate(key, testObject);
        IResult<List<Object>> beforeDelete = storageClient.RetrieveAll();

        Assert.Single(beforeDelete.Entity);

        IResult deleted = storageClient.Delete(key);
        Assert.True(deleted.Success);

        IResult<List<Object>> afterDelete = storageClient.RetrieveAll();
        Assert.Empty(afterDelete.Entity);
    }
}
