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

using BaSyx.ServiceProvider.EventDriven.DataMapping;
using Microsoft.Extensions.Logging;
using Moq;

namespace BaSyx.ServiceProvider.EventDriven.Tests.DataMapping;

/// <summary>
/// Test implemantation of <see cref="IEventMessageMapper{TSource,TMessage}"/>
/// </summary>
/// <typeparam name="TSource">Source type</typeparam>
/// <typeparam name="TMessage">Message type to map to</typeparam>
internal class TestEventMessageMapper<TSource, TMessage> : IEventMessageMapper<TSource, TMessage>
{
    public TMessage Map(TSource eventData)
    {
        throw new NotImplementedException();
    }
}

public class EventMessageMapperManagerTests
{
    [Fact]
    public async Task GetMapper_WhenOneAvailable_GetsOne()
    {
        var loggerMock = new Mock<ILogger<EventMessageMapperManager>>();
        var mapper1 = new TestEventMessageMapper<object, string>();

        var mapperManager = new EventMessageMapperManager(loggerMock.Object, new[] { mapper1 });

        var mappers = mapperManager.GetMapper<string>();
        
        Assert.Collection<IEventMessageMapper?>(mappers, m => Assert.Equal(m, mapper1));
    }
    
    [Fact]
    public async Task GetMapper_WhenOneOfTargetTypeAvailable_GetsOne()
    {
        var loggerMock = new Mock<ILogger<EventMessageMapperManager>>();
        var mapper1 = new TestEventMessageMapper<object, string>();
        var mapperWithDifferentTargetType = new TestEventMessageMapper<object, double>();

        var mapperManager = new EventMessageMapperManager(loggerMock.Object, 
            new IEventMessageMapper[] { mapper1, mapperWithDifferentTargetType });

        var mappers = mapperManager.GetMapper<string>();
        
        Assert.Collection<IEventMessageMapper?>(mappers, m => Assert.Equal(m, mapper1));
    }
    
    [Fact]
    public async Task GetMapper_WhenTwoOfTargetTypeAvailable_GetsTwo()
    {
        var loggerMock = new Mock<ILogger<EventMessageMapperManager>>();
        var mapper1 = new TestEventMessageMapper<object, string>();
        var mapper2 = new TestEventMessageMapper<object, string>();
        var mapperWithDifferentTargetType = new TestEventMessageMapper<object, double>();

        var mapperManager = new EventMessageMapperManager(loggerMock.Object, 
            new IEventMessageMapper[] { mapper1, mapper2, mapperWithDifferentTargetType });

        var mappers = mapperManager.GetMapper<string>();
        
        Assert.Collection<IEventMessageMapper?>(mappers, 
            m => Assert.Equal(m, mapper1), 
            m => Assert.Equal(m , mapper2));
    }
    
    [Fact]
    public async Task GetMapper_WhenNoneOfTargetTypeAvailable_GetsEmptyEnumerable()
    {
        var loggerMock = new Mock<ILogger<EventMessageMapperManager>>();
        
        var mapperWithDifferentTargetType = new TestEventMessageMapper<object, double>();

        var mapperManager = new EventMessageMapperManager(loggerMock.Object, 
            new IEventMessageMapper[] { mapperWithDifferentTargetType });

        var mappers = mapperManager.GetMapper<string>();
        
        Assert.Empty(mappers);
    }
}