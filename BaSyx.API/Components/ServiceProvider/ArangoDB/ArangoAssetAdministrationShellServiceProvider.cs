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
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

public class ArangoAssetAdministrationShellServiceProvider : IAssetAdministrationShellServiceProvider
{
    public IAssetAdministrationShellDescriptor ServiceDescriptor => throw new System.NotImplementedException();

    ISubmodelServiceProviderRegistry SubmodelRegistry { get; }

    ISubmodelServiceProviderRegistry IAssetAdministrationShellServiceProvider.SubmodelRegistry => throw new System.NotImplementedException();

    public void BindTo(IAssetAdministrationShell element)
    {
        throw new System.NotImplementedException();
    }

    public IAssetAdministrationShell GetBinding()
    {
        throw new System.NotImplementedException();
    }
}
