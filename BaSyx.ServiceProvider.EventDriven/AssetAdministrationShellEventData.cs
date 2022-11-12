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

using System.Reactive;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Base record for all event data that is used to 
/// </summary>
public record AssetAdministrationShellEventData
{
    /// <summary>
    /// Identification of the Aas 
    /// </summary>
    public string AasId { get; }

    public AssetAdministrationShellEventData(string aasId)
    {
        AasId = aasId;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Timestamp of the event
    /// </summary>
    public DateTime Timestamp { get; }
}

/// <summary>
/// Record with the data data is 
/// </summary>
public record AssetAdministrationShellCreatedEventData : AssetAdministrationShellEventData
{
    public IAssetAdministrationShell CreatedShell { get; }

    public AssetAdministrationShellCreatedEventData(string aasId, IAssetAdministrationShell createdShell): base(aasId)
    {
        CreatedShell = createdShell;
    }
}

/// <summary>
/// Record with event data for AAS updates
/// </summary>
public record AssetAdministrationShellUpdatedEventData : AssetAdministrationShellEventData
{
    public IAssetAdministrationShell UpdatedShell { get; }

    public AssetAdministrationShellUpdatedEventData(string aasId, 
        IAssetAdministrationShell updatedShell): base(aasId)
    {
        UpdatedShell = updatedShell;
    }
}

/// <summary>
/// Record with event data for deleted AAS
/// </summary>
public record AssetAdministrationShellDeletedEventData : AssetAdministrationShellEventData
{
    public AssetAdministrationShellDeletedEventData(string aasId): base(aasId)
    {
    }
}

/// <summary>
/// Event data for registration of service providers
/// </summary>
public record AssetAdministrationShellServiceProviderRegisteredEventData : AssetAdministrationShellEventData
{
    public AssetAdministrationShellServiceProviderRegisteredEventData(string aasId,
        IAssetAdministrationShellServiceProvider serviceProvider) : base(aasId)
    {
        
    }
}

/// <summary>
/// Event data for unregistration of service providers
/// </summary>
public record AssetAdministrationShellServiceProviderUnregisteredEventData : AssetAdministrationShellEventData
{
    public AssetAdministrationShellServiceProviderUnregisteredEventData(string aasId) : base(aasId)
    {
        
    }
}