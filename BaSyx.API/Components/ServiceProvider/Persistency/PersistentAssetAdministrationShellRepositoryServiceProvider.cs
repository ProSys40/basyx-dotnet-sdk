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
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Clients.Persistency;
using BaSyx.API.Components.Persistency;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.API.Components;

public class PersistentAssetAdministrationShellRepositoryServiceProvider : IAssetAdministrationShellRepositoryServiceProvider
{

    private IAssetAdministrationShellRepositoryDescriptor _serviceDescriptor;
    public IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor
    {
        get
        {
            if (_serviceDescriptor == null)
                _serviceDescriptor = new AssetAdministrationShellRepositoryDescriptor(GetBinding(), null);

            return _serviceDescriptor;
        }
        private set
        {
            _serviceDescriptor = value;
        }
    }
    public IAssetAdministrationShellServiceProviderRegistry ServiceProviderRegistry { get; set; }

    public IPersistentIdentifiables<IAssetAdministrationShell> PersistentShells { get; set; }

    public void BindTo(IEnumerable<IAssetAdministrationShell> shells)
    {
        shells.ToList().ForEach(shell => RegisterAssetAdministrationShellServiceProvider(shell.Identification.Id, shell.CreateServiceProvider(true)));
        
        ServiceDescriptor = ServiceDescriptor ?? new AssetAdministrationShellRepositoryDescriptor(shells, null);
    }

    public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell shell)
    {
        return PersistentShells.CreateOrUpdate(shell.Identification.Id, shell);
    }

    public IResult DeleteAssetAdministrationShell(string shellId)
    {
        return PersistentShells.Delete(shellId);
    }

    public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
    {
        return ServiceProviderRegistry.GetAssetAdministrationShellServiceProvider(id);
    }

    public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
    {
        return ServiceProviderRegistry.GetAssetAdministrationShellServiceProviders();
    }

    public IEnumerable<IAssetAdministrationShell> GetBinding()
    {
        List<IAssetAdministrationShell> assetAdministrationShells = new List<IAssetAdministrationShell>();
        if (GetAssetAdministrationShellServiceProviders().TryGetEntity(out IEnumerable<IAssetAdministrationShellServiceProvider> serviceProviders))
        {
            serviceProviders.ToList()
                .Select(serviceProvider => serviceProvider.GetBinding()).ToList()
                .ForEach(assetAdministrationShells.Add);
        }
        return assetAdministrationShells;
    }

    public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
    {
        return ServiceProviderRegistry.RegisterAssetAdministrationShellServiceProvider(id, assetAdministrationShellServiceProvider);
    }

    public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string shellId)
    {
        if (GetAssetAdministrationShellServiceProvider(shellId).TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
        {
            return new Result<IAssetAdministrationShell>(true, serviceProvider.GetBinding());
        }
        return new Result<IAssetAdministrationShell>(false, new NotFoundMessage($"Asset Administration Shell Service Provider for shellId '{shellId}'"));
    }

    public IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
    {
        return new Result<IElementContainer<IAssetAdministrationShell>>(true, new ElementContainer<IAssetAdministrationShell>(null, GetBinding()));
    }

    public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
    {
        return ServiceProviderRegistry.UnregisterAssetAdministrationShellServiceProvider(id);
    }

    public IResult UpdateAssetAdministrationShell(string shellId, IAssetAdministrationShell shell)
    {
        return PersistentShells.CreateOrUpdate(shellId, shell); 
    }
}
