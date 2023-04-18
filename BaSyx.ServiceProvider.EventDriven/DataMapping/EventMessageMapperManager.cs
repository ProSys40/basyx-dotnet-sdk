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

namespace BaSyx.ServiceProvider.EventDriven.DataMapping;

/// <summary>
/// Gets all registered mappers and provides them to the event collector
/// </summary>
public class EventMessageMapperManager : IEventMessageMapperManager
{
    private readonly ILogger<EventMessageMapperManager> _logger;
    private readonly Dictionary<Type, List<IEventMessageMapper>> _mapperDictionary;

    public EventMessageMapperManager(ILogger<EventMessageMapperManager> logger, 
        IEnumerable<IEventMessageMapper> mappers)
    {
        _logger = logger;

        _mapperDictionary = mappers
            .Where(m => m.GetType().IsGenericType)
            .Select(m => new { Mapper = m, TypeArgs = m.GetType().GenericTypeArguments })
            .Where(o => o.TypeArgs.Length >= 2)
            .GroupBy(o => o.TypeArgs[1])
            .ToDictionary(g => g.Key, g => g.Select(m => m.Mapper).ToList());
    }

    public IEnumerable<IEventMessageMapper> GetMapper<TMessage>()
    {
        if (_mapperDictionary.TryGetValue(typeof(TMessage), out var mappers))
        {
            return mappers;
        }

        _logger.LogWarning("No mapper found for message type {MessageType}", typeof(TMessage));
        return Enumerable.Empty<IEventMessageMapper>();
    }
}