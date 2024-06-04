// /*******************************************************************************
// * Copyright (c) 2023 LTSoft - Agentur für Leittechnik-Software GmbH
// * Author: Björn Höper
// *
// * This program and the accompanying materials are made available under the
// * terms of the Eclipse Public License 2.0 which is available at
// * http://www.eclipse.org/legal/epl-2.0
// *
// * SPDX-License-Identifier: EPL-2.0
// *******************************************************************************/

using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.ServiceProvider.EventDriven.EventCollector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

public class EventDrivenSubmodelServiceProviderFactory<TPersisting>: ISubmodelServiceProviderFactory 
    where TPersisting: ISubmodelServiceProvider
{
    private readonly ILogger<EventDrivenSubmodelServiceProviderFactory<TPersisting>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventCollector<SubmodelEventData>? _eventCollector;


    public EventDrivenSubmodelServiceProviderFactory(ILogger<EventDrivenSubmodelServiceProviderFactory<TPersisting>> logger, 
        IServiceProvider serviceProvider, IEventCollector<SubmodelEventData>? eventCollector = null)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _eventCollector = eventCollector;
    }

    public ISubmodelServiceProvider CreateSubmodelServiceProvider(ISubmodel submodel)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<EventDrivenSubmodelServiceProvider<TPersisting>>>();
        var persisting = _serviceProvider.GetRequiredService<TPersisting>();
        var serviceProvider = new EventDrivenSubmodelServiceProvider<TPersisting>(logger, persisting, submodel);

        if (_eventCollector != null)
        {
            _eventCollector.Register(serviceProvider.SubmodelEventObservable);
            _logger.LogInformation("Registered new event driven submodel service provider for submodel {SubmodelId}",
                submodel.Identification);
        }
        
        return serviceProvider;
    }
}