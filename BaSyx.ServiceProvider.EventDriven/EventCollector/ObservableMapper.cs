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

using System.Reactive.Subjects;
using BaSyx.ServiceProvider.EventDriven.DataMapping;

namespace BaSyx.ServiceProvider.EventDriven.EventCollector;

/// <summary>
/// Combines an <see cref="IEventCollector{TEventData}"/> and an <see cref="IEventMessageMapper{TEvent,TMessage}"/> to
/// emit messages instead of events from the collector
/// </summary>
/// <typeparam name="TEventData">Events created by the collector</typeparam>
/// <typeparam name="TMessage">Type of the messages produced after mapping</typeparam>
public class ObservableMapper<TEventData, TMessage> : IObservable<TMessage>, IDisposable
{
    private readonly Subject<TMessage> _mappedSubject;
    private readonly IDisposable _observableSubscription;

    /// <summary>
    /// Create the combined observable
    /// </summary>
    /// <param name="observable"></param>
    /// <param name="mapper"></param>
    public ObservableMapper(IObservable<TEventData> observable, IEventMessageMapper<TEventData, TMessage> mapper)
    {
        _mappedSubject = new Subject<TMessage>();
        _observableSubscription = observable.Subscribe(e => _mappedSubject.OnNext(mapper.Map(e)));
    }
    
    /// <inheritdoc cref="IDisposable"/>>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Free managed ressources 
        }
        
        _mappedSubject.Dispose();
        _observableSubscription.Dispose();
    }
    
    /// <inheritdoc cref="IObservable{T}"/>>
    public IDisposable Subscribe(IObserver<TMessage> observer)
    {
        return _mappedSubject.Subscribe(observer);
    }
}