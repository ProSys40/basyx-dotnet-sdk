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

using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven.EventCollector;

/// <summary>
/// Non-generic interface for event collectors
/// </summary>
public interface IEventCollector
{
    
}

/// <summary>
/// Collects events from the event driven AAS repositories
/// </summary>
public interface IEventCollector<TEventData>: IEventCollector
{
    /// <summary>
    /// Register the observable to be merged into the event stream
    /// </summary>
    /// <param name="observable"></param>
    void Register(IObservable<TEventData> observable);
    
    /// <summary>
    /// Unregister the observable from the event stream
    /// </summary>
    /// <param name="observable"></param>
    void Unregister(IObservable<TEventData> observable);
    
    /// <summary>
    /// Exposed merged event observable
    /// </summary>
    IObservable<TEventData> EventObservable { get; }
    
    /// <summary>
    /// Gets the count of registered event providers
    /// </summary>
    int RegisteredProviderCount { get; }
}