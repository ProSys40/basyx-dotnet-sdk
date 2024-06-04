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
using BaSyx.ServiceProvider.EventDriven.EventCollector;
using Microsoft.Extensions.Logging;
using Moq;

namespace BaSyx.ServiceProvider.EventDriven.Tests.EventCollector;

public class EventCollectorBaseTests
{
    /// <summary>
    /// Test that two registered subjects are emitted through the joint observable after registering
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Register_TwoSubjects_EmitsBoth()
    {
        // Mock the logger
        var loggerMock = new Mock<ILogger<EventCollectorBase<string>>>();
        // Arrange
        var collector = new EventCollectorBase<string>(loggerMock.Object);
        
        var subject1 = new Subject<string>();
        var subject2 = new Subject<string>();
        
        var emitted = new List<string>();
        collector.Register(subject1);
        collector.Register(subject2);
        collector.EventObservable.Subscribe(emitted.Add);
        
        // Act
        subject1.OnNext("1");
        subject2.OnNext("2");
        
        // Assert
        Assert.Collection(emitted, 
            s => Assert.Equal("1", s), 
            s => Assert.Equal("2", s));
    }
    
    /// <summary>
    /// Test that two registered subjects are emitted through the joint observable after registering
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Register_TwoSubjectsOneAfterSubscribe_EmitsBoth()
    {
        // Mock the logger
        var loggerMock = new Mock<ILogger<EventCollectorBase<string>>>();
        // Arrange
        var collector = new EventCollectorBase<string>(loggerMock.Object);
        
        var subject1 = new Subject<string>();
        var subject2 = new Subject<string>();
        
        var emitted = new List<string>();
        collector.Register(subject1);
        collector.EventObservable.Subscribe(emitted.Add);
        collector.Register(subject2);
        
        // Act
        subject1.OnNext("1");
        subject2.OnNext("2");
        
        // Assert
        Assert.Collection(emitted, 
            s => Assert.Equal("1", s), 
            s => Assert.Equal("2", s));
    }

    ///<summary>
    /// Test that when unregistering only the remaining observable gets emitted
    /// </summary>
    [Fact]
    public async Task Unregister_WhenEmittedAfter_OnlyOneEmitted()
    {
        // Mock the logger
        var loggerMock = new Mock<ILogger<EventCollectorBase<string>>>();
        // Arrange
        var collector = new EventCollectorBase<string>(loggerMock.Object);
        
        var subject1 = new Subject<string>();
        var subject2 = new Subject<string>();
        
        var emitted = new List<string>();
        collector.Register(subject1);
        collector.EventObservable.Subscribe(emitted.Add);
        collector.Register(subject2);
        
        // Unregister subject1
        collector.Unregister(subject1);
        
        // Act
        subject1.OnNext("1");
        subject2.OnNext("2");
        
        // Assert
        Assert.Collection(emitted, 
            s => Assert.Equal("2", s));
    }
    
    ///<summary>
    /// Test that when unregistering a non existing observable that no exception is thrown 
    /// </summary>
    [Fact]
    public async Task Unregister_WhenUnregisteringNonExisting_NoExceptionThrown()
    {
        // Mock the logger
        var loggerMock = new Mock<ILogger<EventCollectorBase<string>>>();
        // Arrange
        var collector = new EventCollectorBase<string>(loggerMock.Object);
        
        var subject1 = new Subject<string>();
        var subject2 = new Subject<string>();
        var subject3 = new Subject<string>();
        
        var emitted = new List<string>();
        collector.Register(subject1);
        collector.EventObservable.Subscribe(emitted.Add);
        collector.Register(subject2);

        // Unregister the unregistered subject
        collector.Unregister(subject3);

        // Act
        subject1.OnNext("1");
        subject2.OnNext("2");
        
        // Assert
        Assert.Collection(emitted, 
            s => Assert.Equal("1", s),
            s => Assert.Equal("2", s));
    }
}