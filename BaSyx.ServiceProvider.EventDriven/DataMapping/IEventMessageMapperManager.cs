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

namespace BaSyx.ServiceProvider.EventDriven.DataMapping;

/// <summary>
/// Gets registered mappers that emit the correct type of message
/// </summary>
public interface IEventMessageMapperManager
{
    /// <summary>
    /// Gets all mappers that emit the given message type
    /// </summary>
    /// <typeparam name="TMessage">Type of event data</typeparam>
    /// <returns>Mapper</returns>
    IEnumerable<IEventMessageMapper> GetMapper<TMessage>();
}