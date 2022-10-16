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

using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

/// <summary>
/// Creates service providers for Asset Administration Shells
/// </summary>
public interface IAssetAdministrationShellServiceProviderFactory
{
    /// <summary>
    /// Create a new service provider for the AAS
    /// </summary>
    /// <param name="aas">Asset administration shell to create the service provider for</param>
    /// <returns>Created service provider</returns>
    public IAssetAdministrationShellServiceProvider CreateServiceProvider(IAssetAdministrationShell aas, bool includeSubmodels);
}