/*******************************************************************************
* Copyright (c) 2023 the Eclipse BaSyx Authors
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaSyx.Models.Core.Common;
using BaSyx.API.Clients.Persistency;
using BaSyx.API.Components.ServiceProvider.Persistency;

namespace BaSyx.API.Components.Persistency;

public class PersistentAssetAdministrationShellServiceProviderRegistry : IAssetAdministrationShellServiceProviderRegistry
{

    public IPersistentIdentifiables<IAssetAdministrationShellDescriptor> PersistentShellDescriptors { get; set; }
    public IPersistentIdentifiables<IAssetAdministrationShell> PersistentShells { get; set; }


    public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
    {
        IResult<IAssetAdministrationShell> shellResult = PersistentShells.Retrieve(id);
        if (!shellResult.Success || shellResult.Entity == null)
        {
            return new Result<IAssetAdministrationShellServiceProvider>(false, new NotFoundMessage(id));
        }

        IAssetAdministrationShell shell = shellResult.Entity;

        IAssetAdministrationShellServiceProvider servideProvider = GetBoundServiceProvider(shell);
        return new Result<IAssetAdministrationShellServiceProvider>(true, servideProvider);
    }

    public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
    {
        IResult<IQueryableElementContainer<IAssetAdministrationShell>> shellsResult = PersistentShells.RetrieveAll<IAssetAdministrationShell>();
        if (!shellsResult.Success || shellsResult.Entity == null)
        {
            return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(false, new NotFoundMessage());
        }

        IQueryableElementContainer<IAssetAdministrationShell> shells = shellsResult.Entity;
        List<IAssetAdministrationShellServiceProvider> serviceProviders = shells.Select(GetBoundServiceProvider).ToList();
        return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(true, serviceProviders);
    }

    private IAssetAdministrationShellServiceProvider GetBoundServiceProvider(IAssetAdministrationShell shell)
    {
        IAssetAdministrationShellServiceProvider serviceProvider;
        IResult<IAssetAdministrationShellDescriptor> descriptorResult = PersistentShellDescriptors.Retrieve(shell.Identification.Id);
        if (!descriptorResult.Success || descriptorResult == null)
        {
            serviceProvider = new PersistentAssetAdministrationShellServiceProvider();
        }
        else
        {
            serviceProvider = new PersistentAssetAdministrationShellServiceProvider(descriptorResult.Entity);
        }
        serviceProvider.BindTo(shell);
        return serviceProvider;
    }

    public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
    {
        IResult<IAssetAdministrationShell> shellResult = PersistentShells.Retrieve(id);
        if (!shellResult.Success || shellResult.Entity == null)
        {
            return new Result<IAssetAdministrationShellDescriptor>(false, new NotFoundMessage(id));
        }
        _ = PersistentShellDescriptors.CreateOrUpdate(id, assetAdministrationShellServiceProvider.ServiceDescriptor);
        return new Result<IAssetAdministrationShellDescriptor>(true, assetAdministrationShellServiceProvider.ServiceDescriptor);
    }

    public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
    {
        IResult<IAssetAdministrationShell> shellResult = PersistentShells.Retrieve(id);
        return new Result(shellResult.Success);
    }
}
