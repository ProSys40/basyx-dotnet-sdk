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

using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reflection;
using BaSyx.ServiceProvider.EventDriven.DataMapping;
using BaSyx.ServiceProvider.EventDriven.EventCollector;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Change message producer that takes mappers that produce their
/// type of event data 
/// </summary>
/// <typeparam name="TMessage">Type of messages emitted</typeparam>
public abstract class MapperBasedChangeMessageProducerBase<TMessage> : IChangeMessageProducer<TMessage>
{
    private readonly ILogger<MapperBasedChangeMessageProducerBase<TMessage>> _logger;
    private readonly Dictionary<IEventCollector, IEventMessageMapper> _mapperByCollector;
    private readonly Subject<TMessage> _messagesToProduce = new();

    protected MapperBasedChangeMessageProducerBase(ILogger<MapperBasedChangeMessageProducerBase<TMessage>> logger, 
        IEnumerable<IEventCollector> eventCollectors, IEventMessageMapperManager eventMessageMapperManager)
    {
        _logger = logger;
        
        // Get the mappers that produce the type of message
        var mappers = eventMessageMapperManager.GetMapper<TMessage>();
        
        // Get the event collectors that have the generic parameter TEventData equal
        // to the generic TEvent parameter of the mapper
        Dictionary<Type?, IEventCollector> eventCollectorOutputTypes = eventCollectors
            .Select(c => GetCollectorOutputType(c))
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .ToDictionary(p => p.Key, p => p.Value);

        // Get the mappers that have the generic parameter TEventData equal to the input of the mapper
        _mapperByCollector = mappers
            .Where(m => m.GetInputType() != null)
            .Where(m => eventCollectorOutputTypes.ContainsKey(m.GetInputType()))
            .ToDictionary(m => eventCollectorOutputTypes[m.GetInputType()], m => m);
        
        // Create the transforming observable
        foreach (var (collector, mapper) in _mapperByCollector)
        {
            var genericMethod = this.GetType().GetMethod(nameof(CreateCollectorMapper), 
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                ?.MakeGenericMethod(new[] { mapper.GetInputType(), mapper.GetOutputType() }!);

            if (genericMethod == null)
            {
                _logger.LogError("Could not create generic method for collector {Collector} and mapper {Mapper}", 
                    collector, mapper);
                continue;
            }
            
            var factoryMethodResult = genericMethod.Invoke(null, new object[] { collector, mapper });
            var observableMapper = factoryMethodResult as IObservable<TMessage>;
            
            if (observableMapper == null)
            {
                _logger.LogError("Could not create observable mapper for collector {Collector} and mapper {Mapper}", 
                    collector, mapper);
            }
            
            // Emit the messages to the stream
            observableMapper?.Subscribe(m => _messagesToProduce.OnNext(m));
        }
    }

    /// <summary>
    /// Messages that should be produced by this producer
    /// </summary>
    public IObservable<TMessage> MessagesToProduce => _messagesToProduce;
    
    /// <summary>
    /// Creates an observable that incorporates the event collector and the mapper
    /// </summary>
    /// <param name="collector">Collector to create the observable collector from</param>
    /// <param name="mapper">Mapper to use for transforming from input to output</param>
    /// <typeparam name="TEventData">Type of event data provided by the collector</typeparam>
    /// <typeparam name="TMessage">Type of message produced</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected static IObservable<TMessage> CreateCollectorMapper<TEventData, TMessage>(
        IEventCollector collector, IEventMessageMapper mapper)
    {
        var castedObservable = (collector as IEventCollector<TEventData>)?.EventObservable ?? throw new InvalidOperationException();
        var castedMapper = (mapper as IEventMessageMapper<TEventData, TMessage>) ??
                           throw new InvalidOperationException();

        var observableMapper = new ObservableMapper<TEventData, TMessage>(castedObservable, castedMapper);
        return observableMapper as IObservable<TMessage> ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Extracts the generic output parameter of the event collector
    /// </summary>
    /// <param name="collector">Collector to get the output from</param>
    /// <returns>KeyValuePair of output type when a collector matches. Empty KeyValuePair otherwise.</returns>
    private KeyValuePair<Type, IEventCollector>? GetCollectorOutputType(IEventCollector collector)
    {
        var outputType = collector.GetType().GetInterfaces()
            .Where(i => i.IsGenericType)
            .Where(i => i.IsAssignableTo(typeof(IEventCollector)))
            .Select(i => i.GenericTypeArguments[0])
            .SingleOrDefault();

            return outputType != null ? new KeyValuePair<Type, IEventCollector>(outputType, collector) : null;
    }

    /// <summary>
    /// Mappers used to produce the messages from the output of the event collectors
    /// </summary>
    public IReadOnlyCollection<IEventMessageMapper> Mappers => _mapperByCollector.Values;

    /// <summary>
    /// Event collectors that are used by this producer
    /// </summary>
    public IReadOnlyCollection<IEventCollector> EventCollectors => _mapperByCollector.Keys;

    /// <inheritdoc cref="IChangeMessageProducer{TMessage}"/>
    public abstract Task StartProduceAsync();
    
    /// <inheritdoc cref="IChangeMessageProducer{TMessage}"/>
    public abstract Task StopProduceAsync();
}