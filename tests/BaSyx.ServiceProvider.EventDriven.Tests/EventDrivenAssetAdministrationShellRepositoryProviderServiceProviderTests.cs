// /*******************************************************************************
// * Copyright (c) 2022 LTSoft - Agentur für Leittechnik-Software GmbH
// * Author: Björn Höper
// *
// * This program and the accompanying materials are made available under the
// * terms of the Eclipse Public License 2.0 which is available at
// * http://www.eclipse.org/legal/epl-2.0
// *
// * SPDX-License-Identifier: EPL-2.0
// *******************************************************************************/

using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using Moq;

namespace BaSyx.ServiceProvider.EventDriven.Tests;

public class EventDrivenAssetAdministrationShellRepositoryProviderServiceProviderTests
{
    [Fact]
    public async Task GetBinding_WhenPersistingDefined_CallsPersister()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        persistingProviderMock.Setup(m => m.GetBinding()).Returns(Enumerable.Empty<AssetAdministrationShell>());
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var binding = eventDrivenProvider.GetBinding();
        
        persistingProviderMock.Verify(m => m.GetBinding(), Times.Once);
    }
    
    [Fact]
    public async Task RetrieveAssetAdministrationShell_WhenPersistingDefined_CallsPersister()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        string aasId = "myAas";
        var returnedResult = new Result<AssetAdministrationShell>(true);
        persistingProviderMock.Setup(m => m.RetrieveAssetAdministrationShell(aasId))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var aas = eventDrivenProvider.RetrieveAssetAdministrationShell(aasId);
        
        persistingProviderMock.Verify(m => m.RetrieveAssetAdministrationShell(aasId), Times.Once);
    }
    
    [Fact]
    public async Task RetrieveAssetAdministrationShells_WhenPersistingDefined_CallsPersister()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var returnedResult = new Result<IElementContainer<IAssetAdministrationShell>>(true);
        persistingProviderMock.Setup(m => m.RetrieveAssetAdministrationShells())
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var aas = eventDrivenProvider.RetrieveAssetAdministrationShells();
        
        persistingProviderMock.Verify(m => m.RetrieveAssetAdministrationShells(), Times.Once);
    }
    
    [Fact]
    public async Task GetAssetAdministrationShellServiceProvider_WhenPersistingDefined_CallsPersister()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aasId = "myAas";
        var serviceProviderMock = new Mock<IAssetAdministrationShellServiceProvider>();
        var result = new Result<IAssetAdministrationShellServiceProvider>(true, serviceProviderMock.Object);
        persistingProviderMock.Setup(m => m.GetAssetAdministrationShellServiceProvider(aasId)).Returns(result);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var binding = eventDrivenProvider.GetAssetAdministrationShellServiceProvider(aasId);
        
        persistingProviderMock.Verify(m => m.GetAssetAdministrationShellServiceProvider(aasId), Times.Once);
    }
    
    [Fact]
    public async Task GetAssetAdministrationShellServiceProviders_WhenPersistingDefined_CallsPersister()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aasId = "myAas";
        var serviceProviderMock = new Mock<IAssetAdministrationShellServiceProvider>();
        var result = new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(true);
        persistingProviderMock.Setup(m => m.GetAssetAdministrationShellServiceProviders()).Returns(result);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var binding = eventDrivenProvider.GetAssetAdministrationShellServiceProviders();
        
        persistingProviderMock.Verify(m => m.GetAssetAdministrationShellServiceProviders(), Times.Once);
    }

    [Fact]
    public async Task CreateAssetAdministrationShell_WhenSuccess_ReturnsEvent()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var returnedResult = new Result<AssetAdministrationShell>(true);
        persistingProviderMock.Setup(m => m.CreateAssetAdministrationShell(It.IsAny<AssetAdministrationShell>()))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var fired = false;
        AssetAdministrationShellEventData capturedEventData = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e =>
        {
            capturedEventData = e;
        });
        var result = eventDrivenProvider.CreateAssetAdministrationShell(aas);
        
        Assert.NotNull(capturedEventData);
        Assert.IsType<AssetAdministrationShellCreatedEventData>(capturedEventData);
        var createdEvent = capturedEventData as AssetAdministrationShellCreatedEventData;
        Assert.Equal(aas, createdEvent.CreatedShell);
        persistingProviderMock.Verify(m => m.CreateAssetAdministrationShell(aas), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task CreateAssetAdministrationShell_WhenNoSuccess_DoesNotFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var returnedResult = new Result<AssetAdministrationShell>(false);
        persistingProviderMock.Setup(m => m.CreateAssetAdministrationShell(It.IsAny<AssetAdministrationShell>()))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var fired = false;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => fired = true);
        var result = eventDrivenProvider.CreateAssetAdministrationShell(aas);
        
        Assert.False(fired);
        persistingProviderMock.Verify(m => m.CreateAssetAdministrationShell(aas), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task UpdateAssetAdministrationShell_WhenNoSuccess_DoesNotFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var updatedAas = new AssetAdministrationShell("myUpdatedAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<AssetAdministrationShell>(false);
        persistingProviderMock.Setup(m => m.UpdateAssetAdministrationShell(aas.IdShort, updatedAas))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        
        var fired = false;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => fired = true);
        var result = eventDrivenProvider.UpdateAssetAdministrationShell(aas.IdShort, updatedAas);
        
        Assert.False(fired);
        persistingProviderMock.Verify(m => m.UpdateAssetAdministrationShell(aas.IdShort, updatedAas), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task UpdateAssetAdministrationShell_WhenSuccess_DoesFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var updatedAas = new AssetAdministrationShell("myUpdatedAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<AssetAdministrationShell>(true);
        persistingProviderMock.Setup(m => m.UpdateAssetAdministrationShell(aas.IdShort, updatedAas))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        
        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e =>
        {
            capturedEvent = e;
        });
        var result = eventDrivenProvider.UpdateAssetAdministrationShell(aas.IdShort, updatedAas);
        
        Assert.NotNull(capturedEvent);
        Assert.IsType<AssetAdministrationShellUpdatedEventData>(capturedEvent);
        var updatedEvent = capturedEvent as AssetAdministrationShellUpdatedEventData;
        Assert.Equal(updatedAas, updatedEvent.UpdatedShell);
        persistingProviderMock.Verify(m => m.UpdateAssetAdministrationShell(aas.IdShort, updatedAas), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task DeleteAssetAdministrationShell_WhenNoSuccess_DoesNotFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<AssetAdministrationShell>(false);
        persistingProviderMock.Setup(m => m.DeleteAssetAdministrationShell(aas.IdShort))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);

        
        var fired = false;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => fired = true);
        var result = eventDrivenProvider.DeleteAssetAdministrationShell(aas.IdShort);
        
        Assert.False(fired);
        persistingProviderMock.Verify(m => m.DeleteAssetAdministrationShell(aas.IdShort), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task DeleteAssetAdministrationShell_WhenSuccess_DoesFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<AssetAdministrationShell>(true);
        persistingProviderMock.Setup(m => m.DeleteAssetAdministrationShell(aas.IdShort))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);


        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => capturedEvent = e);
        var result = eventDrivenProvider.DeleteAssetAdministrationShell(aas.IdShort);
        
        Assert.NotNull(capturedEvent);
        Assert.IsType<AssetAdministrationShellDeletedEventData>(capturedEvent);
        var deletedEvent = capturedEvent as AssetAdministrationShellDeletedEventData;
        Assert.Equal(aas.IdShort, deletedEvent.AasId);
        
        persistingProviderMock.Verify(m => m.DeleteAssetAdministrationShell(aas.IdShort), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task RegisterAssetAdministrationShellServiceProvider_WhenNoSuccess_DoesNotFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<IAssetAdministrationShellServiceProvider>(true);
        var serviceProviderMock = new Mock<IAssetAdministrationShellServiceProvider>();
        var aasDescriptorMock = new Mock<IAssetAdministrationShellDescriptor>();
        persistingProviderMock.Setup(m => m.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object))
            .Returns(new Result<IAssetAdministrationShellDescriptor>(false, aasDescriptorMock.Object));
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);


        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => capturedEvent = e);
        var result = eventDrivenProvider.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object);
        
        Assert.Null(capturedEvent);
        
        
        persistingProviderMock.Verify(m => m.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task RegisterAssetAdministrationShellServiceProvider_WhenSuccess_DoesFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result<IAssetAdministrationShellServiceProvider>(true);
        var serviceProviderMock = new Mock<IAssetAdministrationShellServiceProvider>();
        var aasDescriptorMock = new Mock<IAssetAdministrationShellDescriptor>();
        persistingProviderMock.Setup(m => m.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object))
            .Returns(new Result<IAssetAdministrationShellDescriptor>(true, aasDescriptorMock.Object));
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);


        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => capturedEvent = e);
        var result = eventDrivenProvider.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object);
        
        Assert.NotNull(capturedEvent);
        Assert.IsType<AssetAdministrationShellServiceProviderRegisteredEventData>(capturedEvent);
        var registeredEvent = capturedEvent as AssetAdministrationShellServiceProviderRegisteredEventData;
        Assert.Equal(aas.IdShort, registeredEvent.AasId);
        
        persistingProviderMock.Verify(m => m.RegisterAssetAdministrationShellServiceProvider(aas.IdShort, serviceProviderMock.Object), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task UnegisterAssetAdministrationShellServiceProvider_WhenNoSuccess_DoesNotFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result(false);
        persistingProviderMock.Setup(m => m.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);


        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => capturedEvent = e);
        var result = eventDrivenProvider.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort);
        
        Assert.Null(capturedEvent);
        
        persistingProviderMock.Verify(m => m.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort), Times.AtLeastOnce);
    }
    
    [Fact]
    public async Task UnegisterAssetAdministrationShellServiceProvider_WhenSuccess_DoesFire()
    {
        var persistingProviderMock = new Mock<IAssetAdministrationShellRepositoryServiceProvider>();
        var aas = new AssetAdministrationShell("myAas", new Identifier("https://testing.com/myAas", KeyType.URI));
        var returnedResult = new Result(true);
        persistingProviderMock.Setup(m => m.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort))
            .Returns(returnedResult);
        var loggerMock =
            new Mock<ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<
                IAssetAdministrationShellRepositoryServiceProvider>>>();
        
        var eventDrivenProvider =
            new EventDrivenAssetAdministrationShellRepositoryServiceProvider<IAssetAdministrationShellRepositoryServiceProvider>(loggerMock.Object, persistingProviderMock.Object);


        AssetAdministrationShellEventData capturedEvent = null;
        var subscription = eventDrivenProvider.AasEventObservable.Subscribe(onNext: e => capturedEvent = e);
        var result = eventDrivenProvider.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort);
        
        Assert.NotNull(capturedEvent);
        Assert.IsType<AssetAdministrationShellServiceProviderUnregisteredEventData>(capturedEvent);
        var unregisteredEvent = capturedEvent as AssetAdministrationShellServiceProviderUnregisteredEventData;
        Assert.Equal(aas.IdShort, unregisteredEvent.AasId);
        
        persistingProviderMock.Verify(m => m.UnregisterAssetAdministrationShellServiceProvider(aas.IdShort), Times.AtLeastOnce);
    }
}