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

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Interface for external producers that write the stream to
/// an external event message broker.
/// </summary>
/// <typeparam name="TMessage">Type of the message</typeparam>
public interface IChangeMessageProducer<TMessage>
{
    /// <summary>
    /// Starts producing the events to the external producer
    /// </summary>
    /// <returns>Task to await</returns>
    Task StartProduceAsync();
    
    /// <summary>
    /// Stop producing the events to the external producer
    /// </summary>
    /// <returns>Task to await</returns>
    Task StopProduceAsync();

    /// <summary>
    /// Messages that should be produced by this producer
    /// </summary>
    IObservable<TMessage> MessagesToProduce { get; }
}