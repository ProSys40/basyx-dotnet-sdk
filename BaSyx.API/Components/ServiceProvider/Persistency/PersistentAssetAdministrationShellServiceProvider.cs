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
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Components.ServiceProvider.Persistency;

public class PersistentAssetAdministrationShellServiceProvider : AssetAdministrationShellServiceProvider
{
    public ISubmodelServiceProviderRegistry PersistentSubmodelServiceProviderRegistry { get; set; }

    public IPersistentIdentifiables<IAssetAdministrationShell> PersistentShells { get; set; }
    public IPersistentIdentifiables<ISubmodel> PersistentSubmodels { get; set; }

    private IAssetAdministrationShell _assetAdministrationShell;

    public PersistentAssetAdministrationShellServiceProvider(IAssetAdministrationShellDescriptor assetAdministrationShellDescriptor) : base(assetAdministrationShellDescriptor)
    {
    }

    public PersistentAssetAdministrationShellServiceProvider()
    {
    }

    public PersistentAssetAdministrationShellServiceProvider(IAssetAdministrationShell assetAdministrationShell) : base(assetAdministrationShell)
    {
    }

    public virtual IAssetAdministrationShell AssetAdministrationShell
    {
        get
        {
            if (_assetAdministrationShell == null)
            {
                throw new Exception("No AssetAdministrationShell bound");
            }
            return GetBinding();
        }
    }

    public override void BindTo(IAssetAdministrationShell shell)
    {
        base.BindTo(shell);
        _assetAdministrationShell = shell;
        BuildAssetAdministrationShell();
    }

    public override IAssetAdministrationShell BuildAssetAdministrationShell()
    {
        IResult<IAssetAdministrationShell> persistentShellResult = PersistentShells.CreateOrUpdate(_assetAdministrationShell.Identification.Id, _assetAdministrationShell);
        if (!persistentShellResult.Success || persistentShellResult == null)
        {
            throw new Exception("Could not Create Or Update persistent AssetAdministrationShell.");
        }
        return persistentShellResult.Entity;
    }


    public override void UseDefaultSubmodelServiceProvider()
    {
        AssetAdministrationShell.Submodels.Values.ToList().ForEach(submodel =>
        {
            var submodelServiceProvider = submodel.CreateServiceProvider();
            RegisterSubmodelServiceProvider(submodel.IdShort, submodelServiceProvider);
        });
    }

    public override IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
    {
        return PersistentSubmodelServiceProviderRegistry.GetSubmodelServiceProviders();
    }

    public override IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string submodelId, ISubmodelServiceProvider submodelServiceProvider)
    {
        return PersistentSubmodelServiceProviderRegistry.RegisterSubmodelServiceProvider(submodelId, submodelServiceProvider);
    }
    public override IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string submodelId)
    {
        return PersistentSubmodelServiceProviderRegistry.GetSubmodelServiceProvider(submodelId);
    }

    public virtual IResult UnregisterSubmodelServiceProvider(string submodelId)
    {
        return PersistentSubmodelServiceProviderRegistry.UnregisterSubmodelServiceProvider(submodelId);
    }
}
