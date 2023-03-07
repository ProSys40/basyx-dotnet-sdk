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
using ArangoDBNetStandard;
using ArangoDBNetStandard.AqlFunctionApi;
using ArangoDBNetStandard.CollectionApi;
using ArangoDBNetStandard.DatabaseApi;
using ArangoDBNetStandard.DocumentApi;
using ArangoDBNetStandard.GraphApi;
using ArangoDBNetStandard.TransactionApi;
using ArangoDBNetStandard.UserApi;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Transactions;

namespace BaSyx.API.Components;

internal class ArangoAssetAdministrationShellRepositoryServiceProvider : IAssetAdministrationShellRepositoryServiceProvider
{
    public IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor => throw new System.NotImplementedException();

    public void BindTo(IEnumerable<IAssetAdministrationShell> element)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
    {
        throw new System.NotImplementedException();
    }

    public IResult DeleteAssetAdministrationShell(string aasId)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<IAssetAdministrationShell> GetBinding()
    {
        throw new System.NotImplementedException();
    }

    public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
    {
        throw new System.NotImplementedException();
    }

    public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
    {
        throw new System.NotImplementedException();
    }

    public IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas)
    {
        throw new System.NotImplementedException();
    }
}
