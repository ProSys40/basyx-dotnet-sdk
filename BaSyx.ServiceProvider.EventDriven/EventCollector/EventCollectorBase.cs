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

using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven.EventCollector;

public class EventCollectorBase<TEventData> : IEventCollector<TEventData>, IDisposable
{
    private readonly ILogger<EventCollectorBase<TEventData>> _logger;
    private readonly Dictionary<IObservable<TEventData>, IDisposable> _registeredObservables = 
        new Dictionary<IObservable<TEventData>, IDisposable>();
    private readonly Subject<TEventData> _combinedObservable = new Subject<TEventData>();
    private bool _disposed = false;

    public EventCollectorBase(ILogger<EventCollectorBase<TEventData>> logger)
    {
        _logger = logger;
    }

    public void Register(IObservable<TEventData> observable)
    {
        var subscription = observable.Subscribe(e => _combinedObservable.OnNext(e));
        _registeredObservables.Add(observable, subscription);
        
        _logger.LogDebug("Registered new observable. Now tracking {ObservableCount} observables", 
            _registeredObservables.Count);
        
    }

    public void Unregister(IObservable<TEventData> observable)
    {
        IDisposable? subscription = null;
        if (!_registeredObservables.TryGetValue(observable, out subscription))
        {
            _logger.LogWarning("Tried to unregister nonexisting observable");
            return;
        }
        
        subscription.Dispose();
        _registeredObservables.Remove(observable);
        
        _logger.LogInformation("Unregistered observable");
    }

    public IObservable<TEventData> EventObservable => _combinedObservable;

    public int RegisteredProviderCount => _registeredObservables.Count;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        
        if (disposing)
        {
            foreach (var keyValuePair in _registeredObservables)
            {
                keyValuePair.Value.Dispose();
            }
            _combinedObservable.Dispose();            
        }

        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}