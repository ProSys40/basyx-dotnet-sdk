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

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

public class EventDrivenAssetAdministrationShellRepositoryServiceProvider<TPersisting>: IAssetAdministrationShellRepositoryServiceProvider, 
    IAssetAdministrationShellEventPublisher where TPersisting: IAssetAdministrationShellRepositoryServiceProvider
{
    private readonly ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<TPersisting>> _logger;
    private readonly TPersisting _persistingProvider;
    private readonly Subject<AssetAdministrationShellEventData> _aasEventSubject;

    public EventDrivenAssetAdministrationShellRepositoryServiceProvider(ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<TPersisting>> logger,
        TPersisting persistingProvider)
    {
        _logger = logger;
        _persistingProvider = persistingProvider;

        _aasEventSubject = new Subject<AssetAdministrationShellEventData>();
    }
    
    private IEnumerable<IAssetAdministrationShell> AssetAdministrationShells => _persistingProvider.GetBinding();
    
    private IAssetAdministrationShellRepositoryDescriptor? _serviceDescriptor;

    IAssetAdministrationShellRepositoryDescriptor IServiceProvider<IEnumerable<IAssetAdministrationShell>, 
        IAssetAdministrationShellRepositoryDescriptor>.ServiceDescriptor => _serviceDescriptor;

    public void BindTo(IEnumerable<IAssetAdministrationShell> boundElement)
    {
        _persistingProvider.BindTo(boundElement);
    }

    public IEnumerable<IAssetAdministrationShell> GetBinding()
    {
        return _persistingProvider.GetBinding();
    }

    /// <summary>
    /// Observable emitting events for all manipulations on the asset administration shells
    /// contained in the repository
    /// </summary>
    public IObservable<AssetAdministrationShellEventData> AasEventObservable => _aasEventSubject;
    
    public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
    {
        var createdResult = _persistingProvider.CreateAssetAdministrationShell(aas);
        if (!createdResult.Success)
        {
            return createdResult;
        }

        var eventData = new AssetAdministrationShellCreatedEventData(aas.IdShort, aas);
        _aasEventSubject.OnNext(eventData);

        return createdResult;
    }

    public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId)
    {
        return _persistingProvider.RetrieveAssetAdministrationShell(aasId);
    }

    public IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
    {
        return _persistingProvider.RetrieveAssetAdministrationShells();
    }

    public IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas)
    {
        var updateResult = _persistingProvider.UpdateAssetAdministrationShell(aasId, aas);

        if (!updateResult.Success)
        {
            _logger.LogError("Update of AAS with {IdShort} failed", aasId);
            return updateResult;
        }

        _logger.LogDebug("Updated AAS {IdShort}", aasId);
        var eventData = new AssetAdministrationShellUpdatedEventData(aasId, aas);
        _aasEventSubject.OnNext(eventData);
        
        return updateResult;
    }

    public IResult DeleteAssetAdministrationShell(string aasId)
    {
        var deletedResult = _persistingProvider.DeleteAssetAdministrationShell(aasId);
        if (!deletedResult.Success)
        {
            _logger.LogError("Could not delete AAS {IdShort}", aasId);
            return deletedResult;
        }

        _logger.LogDebug("Updated AAS {IdShort}", aasId);
        var deletedEvent = new AssetAdministrationShellDeletedEventData(aasId);
        _aasEventSubject.OnNext(deletedEvent);
        
        return deletedResult;
    }

    public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id,
        IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
    {
        var registrationResult =
            _persistingProvider.RegisterAssetAdministrationShellServiceProvider(id,
                assetAdministrationShellServiceProvider);

        if (!registrationResult.Success)
        {
            _logger.LogError("Failed to register ServiceProvider for {AasId}", id);
            return registrationResult;
        }

        _logger.LogDebug("Registered service provider for {AasId}", id);
        var registrationEventData =
            new AssetAdministrationShellServiceProviderRegisteredEventData(id, assetAdministrationShellServiceProvider);
        _aasEventSubject.OnNext(registrationEventData);
        return registrationResult;
    }

    public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
    {
        var unregistrationResult =
            _persistingProvider.UnregisterAssetAdministrationShellServiceProvider(id);

        if (!unregistrationResult.Success)
        {
            _logger.LogError("Failed to unregister ServiceProvider from {AasId}", id);
            return unregistrationResult;
        }

        _logger.LogDebug("Unregistered service provider for {AasId}", id);
        var unregistrationEventData =
            new AssetAdministrationShellServiceProviderUnregisteredEventData(id);
        _aasEventSubject.OnNext(unregistrationEventData);
        return unregistrationResult;
    }

    public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
    {
        return _persistingProvider.GetAssetAdministrationShellServiceProvider(id);
    }

    public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
    {
        return _persistingProvider.GetAssetAdministrationShellServiceProviders();
    }
}