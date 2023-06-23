/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
* Authors: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using Basyx.API.Tests.Components.ServiceProvider;
using BaSyx.API.Clients;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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
        var storageClientMock = new Mock<StorageClient<ISubmodel>>(MockBehavior.Default, "testStorageName", "testCollectionName");
        initializeStorageClientMock(ref storageClientMock);
        return new PersistentSubmodelRepositoryServiceProvider()
        {
            ServiceProviderFactory = submodelServiceProviderFactory,
            StorageClient = storageClientMock.Object
        };
    }

    private void initializeStorageClientMock(ref Mock<StorageClient<ISubmodel>> storageClientMock)
    {
        storageClientMock
            .Setup(storage => storage.CreateOrUpdate(It.IsAny<string>(), It.IsAny<ISubmodel>()))
            .Callback<string, ISubmodel>((key, entry) => _storageEntries[key] = entry)
            .Returns<string, ISubmodel>((key, _ignore) => GetPseudoPersistentResult(key));

        storageClientMock
            .Setup(storage => storage.Retrieve(It.IsAny<string>()))
            .Returns<string>(key => GetPseudoPersistentResult(key));

        storageClientMock
            .Setup(storage => storage.Delete(It.IsAny<string>()))
            .Callback<string>(deleteKey => _storageEntries.Remove(deleteKey))
            .Returns(new Result(true));
    }

    private IResult<ISubmodel> GetPseudoPersistentResult(string key)
    {
        ISubmodel submodel; 
        if(!_storageEntries.TryGetValue(key, out submodel))
        {
            return new Result<ISubmodel>(new Exception($"Submodel with key '{key}' does not Exist"));
        }
        return new Result<ISubmodel>(true, submodel);
    }
}
