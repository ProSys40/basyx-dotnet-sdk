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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

public class EventDrivenSubmodelServiceProviderFactory<TPersisting>: ISubmodelServiceProviderFactory 
    where TPersisting: ISubmodelServiceProvider
{
    private readonly ILogger<EventDrivenSubmodelServiceProviderFactory<TPersisting>> _logger;
    private readonly IServiceProvider _serviceProvider;


    public EventDrivenSubmodelServiceProviderFactory(ILogger<EventDrivenSubmodelServiceProviderFactory<TPersisting>> logger, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public ISubmodelServiceProvider CreateSubmodelServiceProvider(ISubmodel submodel)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<EventDrivenSubmodelServiceProvider<TPersisting>>>();
        var persisting = _serviceProvider.GetRequiredService<TPersisting>();
        return new EventDrivenSubmodelServiceProvider<TPersisting>(logger, persisting, submodel);
    }
}