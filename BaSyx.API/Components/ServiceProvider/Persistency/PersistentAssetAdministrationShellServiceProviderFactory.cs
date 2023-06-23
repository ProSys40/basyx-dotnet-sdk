/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using System.Linq;
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

/// <summary>
/// Creates providers for the AAS that use a persistency backend like ArangoDB
/// </summary>
public class PersistentAssetAdministrationShellServiceProviderFactory : IAssetAdministrationShellServiceProviderFactory
{

    private readonly ISubmodelServiceProviderFactory _submodelServiceProviderFactory;

    public PersistentAssetAdministrationShellServiceProviderFactory(ISubmodelServiceProviderFactory submodelServiceProviderFactory)
    {
        _submodelServiceProviderFactory = submodelServiceProviderFactory;
    }


    public IAssetAdministrationShellServiceProvider CreateServiceProvider(IAssetAdministrationShell aas, bool includeSubmodels)
    {
        PersistentAssetAdministrationShellServiceProvider persistentServiceProvider = new(aas);

        if (includeSubmodels && aas.Submodels.Any())
        {
            foreach (var submodel in aas.Submodels.Values)
            {
                var submodelSp = _submodelServiceProviderFactory.CreateSubmodelServiceProvider(submodel);
                persistentServiceProvider.RegisterSubmodelServiceProvider(submodel.IdShort, submodelSp);
            }
        }

        return persistentServiceProvider;

    }
}