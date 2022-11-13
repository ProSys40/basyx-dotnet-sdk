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

using System.Net.NetworkInformation;
using BaSyx.API.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

public static class EventDrivenServiceProviderExtensions
{
    /// <summary>
    /// Adds an event driven Asset Administration Shell Repository provider to the services in dependency
    /// injection. The persisting happens through the 
    /// </summary>
    /// <param name="serviceCollection">Service collection to add the required components to</param>
    /// <typeparam name="TAasPersister">
    /// Type of persisting repository service provider.
    /// The type must already be registered in the dependency injection container (as the type itself not with the interface)
    /// </typeparam>
    public static void AddEventDrivenAasRepositoryServiceProvider<TAasPersister>(
        this IServiceCollection serviceCollection) 
        where TAasPersister : IAssetAdministrationShellRepositoryServiceProvider 
    {
        serviceCollection.AddSingleton<IAssetAdministrationShellRepositoryServiceProvider,
                EventDrivenAssetAdministrationShellRepositoryServiceProvider<TAasPersister>>(serviceProvider =>
        {
            var aasPersister = serviceProvider.GetRequiredService<TAasPersister>();
            var logger = serviceProvider
                .GetRequiredService<
                    ILogger<EventDrivenAssetAdministrationShellRepositoryServiceProvider<TAasPersister>>>();

            return new EventDrivenAssetAdministrationShellRepositoryServiceProvider<TAasPersister>(logger,
                aasPersister);
        });
    }
}