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

namespace BaSyx.ServiceProvider.EventDriven.Tests.DataMapping;

internal class TestGenericMapperInterfaceImplementation: IEventMessageMapper<string, int>
{
    public int Map(string eventData)
    {
        return eventData.Length;
    }
}

internal class TestGenericMapperImplementation<TInput, TOutput>: IEventMessageMapper<TInput, TOutput>
{
    public TOutput Map(TInput eventData)
    {
        throw new NotImplementedException();
    }
} 

internal class TestGenericInputImplementation<TInput> : IEventMessageMapper<TInput, string>
{
    public string Map(TInput eventData)
    {
        throw new NotImplementedException();
    }
}

internal class TestNonGenericMapperImplementation: IEventMessageMapper
{
    
}

public class EventMessageMapperExtensionsTests
{
    [Fact]
    public async Task GetInputType_WhenNonGenericClass_ReturnsType()
    {
        var mapper = new TestGenericMapperInterfaceImplementation();
        
        var inputType = mapper.GetInputType();
        
        Assert.Equal(typeof(string), inputType);
    }
    
    [Fact]
    public async Task GetInputType_WhenGenericClass_ReturnsGenericClassType()
    {
        var mapper = new TestGenericInputImplementation<string>();
        
        var inputType = mapper.GetInputType();
        
        Assert.Equal(typeof(string), inputType);
    }
    
    [Fact]
    public async Task GetInputType_WhenNonGenericInterfaceImplementation_ReturnsNull()
    {
        var mapper = new TestNonGenericMapperImplementation();
        
        var inputType = mapper.GetInputType();
        
        Assert.Null(inputType);
    }

    [Fact]
    public async Task GetInputType_WhenGenericInterfaceImplementation_ReturnsType()
    {
        var mapper = new TestGenericMapperImplementation<int, string>();
        
        var inputType = mapper.GetInputType();
        
        Assert.Equal(typeof(int), inputType);
    }
    
    [Fact]
    public async Task GetOutputType_WhenNonGenericClass_ReturnsType()
    {
        var mapper = new TestGenericMapperInterfaceImplementation();
        
        var outputType = mapper.GetOutputType();
        
        Assert.Equal(typeof(int), outputType);
    }
    
    [Fact]
    public async Task GetOutputType_WhenGenericClass_ReturnsGenericClassType()
    {
        var mapper = new TestGenericInputImplementation<int>();
        
        var outputType = mapper.GetOutputType();
        
        Assert.Equal(typeof(string), outputType);
    }
    
    [Fact]
    public async Task GetOutputType_WhenNonGenericInterfaceImplementation_ReturnsNull()
    {
        var mapper = new TestNonGenericMapperImplementation();
        
        var outputType = mapper.GetOutputType();
        
        Assert.Null(outputType);
    }

    [Fact]
    public async Task GetOutputType_WhenGenericInterfaceImplementation_ReturnsType()
    {
        var mapper = new TestGenericMapperImplementation<int, string>();
        
        var outputType = mapper.GetOutputType();
        
        Assert.Equal(typeof(string), outputType);
    }
    
}