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

using System.Linq;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

/// <summary>
/// Creates internal In-Memory providers for the AAS
/// </summary>
public class InternalAssetAdministrationShellServiceProviderFactory : IAssetAdministrationShellServiceProviderFactory
{
    private readonly ISubmodelServiceProviderFactory _submodelServiceProviderFactory;

    public InternalAssetAdministrationShellServiceProviderFactory(ISubmodelServiceProviderFactory submodelServiceProviderFactory)
    {
        _submodelServiceProviderFactory = submodelServiceProviderFactory;
    }

    public IAssetAdministrationShellServiceProvider CreateServiceProvider(IAssetAdministrationShell aas,
        bool includeSubmodels)
    {
        InternalAssetAdministrationShellServiceProvider sp = new InternalAssetAdministrationShellServiceProvider(aas);

        if (includeSubmodels && aas.Submodels.Any())
        {
            foreach (var submodel in aas.Submodels.Values)
            {
                var submodelSp = _submodelServiceProviderFactory.CreateSubmodelServiceProvider(submodel);
                sp.RegisterSubmodelServiceProvider(submodel.IdShort, submodelSp);
            }
        }

        return sp;
    }
}