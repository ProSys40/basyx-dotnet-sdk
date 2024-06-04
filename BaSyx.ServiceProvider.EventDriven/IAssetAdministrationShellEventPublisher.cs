// /*******************************************************************************
// * Copyright (c) 2022 LTSoft - Agentur für Leittechnik-Software GmbH
// * Author: Björn Höper
// *
// * This program and the accompanying materials are made available under the
// * terms of the Eclipse Public License 2.0 which is available at
// * http://www.eclipse.org/legal/epl-2.0
// *
// * SPDX-License-Identifier: EPL-2.0
// *******************************************************************************/

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Interface for components that publish lifecycle events for Asset Administration Shells (AAS). The events are made
/// available through observables 
/// </summary>
public interface IAssetAdministrationShellEventPublisher
{
    /// <summary>
    /// Observable providing the events of manipulations of the asset administration shell repository
    /// </summary>
    IObservable<AssetAdministrationShellEventData> AasEventObservable { get; }
}