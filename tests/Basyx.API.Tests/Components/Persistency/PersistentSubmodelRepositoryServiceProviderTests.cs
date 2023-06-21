using ArangoDBNetStandard.CollectionApi.Models;
using Basyx.API.Tests.Components.ServiceProvider;
using BaSyx.API.Clients;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Utils.ResultHandling;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Components.Persistency;

public class PersistentSubmodelRepositoryServiceProviderTests : SubmodelRepositoryServiceProviderTestSuite
{
    private Dictionary<string, ISubmodel> _storageEntries;

    public PersistentSubmodelRepositoryServiceProviderTests() : base()
    {
        _storageEntries = new Dictionary<string, ISubmodel>();
    }

    protected override ISubmodelRepositoryServiceProvider GetSubmodelRepositoryServiceProvider(ISubmodelServiceProviderFactory submodelServiceProviderFactory)
    {
        var storageClientMock = new Mock<StorageClient<ISubmodel>>();
        initializeStorageClientMock(ref storageClientMock);
        return new PersistentSubmodelRepositoryServiceProvider()
        {
            StorageClient = storageClientMock.Object
        };
    }

    private void initializeStorageClientMock(ref Mock<StorageClient<ISubmodel>> storageClientMock)
    {
        storageClientMock
                    .Setup(storage => storage.CreateOrUpdate(It.IsAny<string>(), It.IsAny<ISubmodel>()))
                    .Callback<ISubmodel>(create => _storageEntries[create.Identification.Id] = create)
                    .Returns((string key) => GetResult(key));

        storageClientMock
            .Setup(storage => storage.Retrieve(It.IsAny<string>()))
            .Returns((string key) => GetResult(key));

        storageClientMock
            .Setup(storage => storage.Delete(It.IsAny<string>()))
            .Callback<string>(deleteKey => _storageEntries.Remove(deleteKey))
            .Returns(new Result(true));
    }

    private IResult<ISubmodel> GetResult(string key)
    {
        ISubmodel submodel; 
        if(!_storageEntries.TryGetValue(key, out submodel))
        {
            return new Result<ISubmodel>(new Exception($"Submodel with key '{key}' does not Exist"));
        }
        return new Result<ISubmodel>(true, submodel);
    }
}
