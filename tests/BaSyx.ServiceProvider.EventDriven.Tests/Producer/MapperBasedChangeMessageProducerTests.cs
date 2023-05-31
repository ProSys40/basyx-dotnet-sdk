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
using BaSyx.ServiceProvider.EventDriven.EventCollector;
using Microsoft.Extensions.Logging;
using Moq;

namespace BaSyx.ServiceProvider.EventDriven.Tests.Producer;

/// <summary>
/// Test implementation for the <see cref="MapperBasedChangeMessageProducerBase{TEvent}"/> abstract class
/// </summary>
public class MapperBasedChangeMessageProducerImplementation: MapperBasedChangeMessageProducerBase<string>
{
    public MapperBasedChangeMessageProducerImplementation(ILogger<MapperBasedChangeMessageProducerBase<string>> logger, 
        IEnumerable<IEventCollector> eventCollectors, IEventMessageMapperManager eventMessageMapperManager) : base(logger, eventCollectors, eventMessageMapperManager)
    {
    }

    public override Task StartProduceAsync()
    {
        throw new NotImplementedException();
    }

    public override Task StopProduceAsync()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Fake implementation of <see cref="IEventCollector{TEvent}"/>
/// </summary>
internal class TestSubmodelEventCollector : IEventCollector<SubmodelEventData>
{
    public TestSubmodelEventCollector(IObservable<SubmodelEventData> eventObservable)
    {
        EventObservable = eventObservable;
    }

    public void Register(IObservable<SubmodelEventData> observable)
    {
        throw new NotImplementedException();
    }

    public void Unregister(IObservable<SubmodelEventData> observable)
    {
        throw new NotImplementedException();
    }

    public IObservable<SubmodelEventData> EventObservable { get; }
    public int RegisteredProviderCount { get; }
}

internal class TestEventMessageMapper : IEventMessageMapper<SubmodelEventData, string>
{
    private int count = 0;
    public TestEventMessageMapper()
    {
        
    }

    public string Map(SubmodelEventData eventData)
    {
        count++;
        return $"Invoked {count}";
    }
}

public class MapperBasedChangeMessageProducerTests
{
    [Fact]
    public async Task Constructor_WhenMatchingMappers_UsesMappers()
    {
        var logger = new Mock<ILogger<MapperBasedChangeMessageProducerImplementation>>();
        var eventCollectorObservableMock = new Mock<IObservable<SubmodelEventData>>();
        var submodelEventCollector = new TestSubmodelEventCollector(eventCollectorObservableMock.Object);
        
        var eventMessageMapperManagerMock = new Mock<IEventMessageMapperManager>();
        var mapper = new TestEventMessageMapper();
        eventMessageMapperManagerMock.Setup(m => m.GetMapper<string>()).Returns(new[] { mapper });

        var producerImplementation = new MapperBasedChangeMessageProducerImplementation(logger.Object,
            new[] { submodelEventCollector },
            eventMessageMapperManagerMock.Object);
        
        Assert.Collection(producerImplementation.Mappers, m => Assert.Equal(mapper, m));
        Assert.Collection(producerImplementation.EventCollectors, e => Assert.Equal(submodelEventCollector, e));
    }

    [Fact]
    public async Task Constructor_WhenMatchingMappers_ProvidesObservable()
    {
        var logger = new Mock<ILogger<MapperBasedChangeMessageProducerImplementation>>();
        var eventCollectorObservableSubject = new Subject<SubmodelEventData>();
        var submodelEventCollector = new TestSubmodelEventCollector(eventCollectorObservableSubject);
        
        var eventMessageMapperManagerMock = new Mock<IEventMessageMapperManager>();
        var mapper = new TestEventMessageMapper();
        eventMessageMapperManagerMock.Setup(m => m.GetMapper<string>()).Returns(new[] { mapper });
        
        var producerImplementation = new MapperBasedChangeMessageProducerImplementation(logger.Object,
            new[] { submodelEventCollector },
            eventMessageMapperManagerMock.Object);

        var producedMessages = new List<string>();
        using (producerImplementation.MessagesToProduce.Subscribe(m => producedMessages.Add(m)))
        {
            eventCollectorObservableSubject.OnNext(new SubmodelEventData("blibla"));
            eventCollectorObservableSubject.OnNext(new SubmodelEventData("blablu"));
        }
        
        Assert.Collection(producedMessages, m => Assert.Equal("Invoked 1", m), m => Assert.Equal("Invoked 2", m));
    }
}