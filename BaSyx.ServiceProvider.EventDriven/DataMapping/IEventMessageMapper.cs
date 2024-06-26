﻿// /*******************************************************************************
// * Copyright (c) 2023 LTSoft - Agentur für Leittechnik-Software GmbH
// * Author: Björn Höper
// *
// * This program and the accompanying materials are made available under the
// * terms of the Eclipse Public License 2.0 which is available at
// * http://www.eclipse.org/legal/epl-2.0
// *
// * SPDX-License-Identifier: EPL-2.0
// *******************************************************************************/

namespace BaSyx.ServiceProvider.EventDriven.DataMapping;


/// <summary>
/// Base interface for event message mappers
/// </summary>
public interface IEventMessageMapper
{
    
}

/// <summary>
/// Maps event data to messages of producers
/// </summary>
/// <typeparam name="TEvent">Data type of the event</typeparam>
/// <typeparam name="TMessage">Type of message</typeparam>
public interface IEventMessageMapper<in TEvent, out TMessage>: IEventMessageMapper
{
    /// <summary>
    /// Maps the event data to a message
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <returns>Message</returns>
    TMessage Map(TEvent eventData);
}


/// <summary>
/// Extension methods for getting types of the mapper
/// </summary>
public static class IEventMessageMapperExtensions
{
    /// <summary>
    /// Gets the input type of generic mappers
    /// </summary>
    // <param name="mapper">Mapper to get the input type for</param>
    /// <returns>Null if no generic implementation. The input type otherwise.</returns>
    public static Type? GetInputType(this IEventMessageMapper mapper)
    {
        var eventMapperInputType =
            mapper.GetType().GetInterfaces()
                .Where(t => t.IsGenericType)
                .Where(t => t.IsAssignableTo(typeof(IEventMessageMapper)))
                .Where(t => t.GenericTypeArguments.Length >= 2)
                .Select(t => t.GenericTypeArguments[0])
                .SingleOrDefault();

        return eventMapperInputType;
    }

    /// <summary>
    /// Gets the output type of generic mappers
    /// </summary>
    /// <param name="mapper">Mapper to get the output for</param>
    /// <returns>Null if no generic implementation. The input type otherwise.</returns>
    public static Type? GetOutputType(this IEventMessageMapper mapper)
    {
        var eventMapperInputType =
            mapper.GetType().GetInterfaces()
                .Where(t => t.IsGenericType)
                .Where(t => t.IsAssignableTo(typeof(IEventMessageMapper)))
                .Where(t => t.GenericTypeArguments.Length >= 2)
                .Select(t => t.GenericTypeArguments[1])
                .SingleOrDefault();

        return eventMapperInputType;
    }
}