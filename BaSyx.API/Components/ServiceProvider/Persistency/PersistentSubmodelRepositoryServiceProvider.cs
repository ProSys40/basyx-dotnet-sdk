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
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Clients.Persistency;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace BaSyx.API.Components;

public class PersistentSubmodelRepositoryServiceProvider : ISubmodelRepositoryServiceProvider
{
    IPersistentCollection<string, ISubmodel> _persistentSubmodels;
    ISubmodelServiceProviderRegistry _servicePrividerRegistry;

    public PersistentSubmodelRepositoryServiceProvider()
    {
    }

    public ISubmodelRepositoryDescriptor ServiceDescriptor
    {
        get
        {
            return new SubmodelRepositoryDescriptor(GetBinding(), null);
        }
        private set
        {
            _ = value;
        }
    }
    public void BindTo(IEnumerable<ISubmodel> submodels)
    {
        submodels.ToList()
            .ForEach(submodel =>
            {
                UpdateSubmodel(submodel.Identification.Id, submodel);
            });
    }

    public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
    {
        return _persistentSubmodels.CreateOrUpdate(submodel.Identification.Id, submodel);
    }

    public IResult DeleteSubmodel(string submodelId)
    {
        return _persistentSubmodels.Delete(submodelId);
    }

    public IEnumerable<ISubmodel> GetBinding()
    {

        List<ISubmodel> submodels = new List<ISubmodel>();
        if (GetSubmodelServiceProviders().TryGetEntity(out IEnumerable<ISubmodelServiceProvider> serviceProviders))
        {
            serviceProviders.ToList()
                .Select(serviceProvider => serviceProvider.GetBinding()).ToList()
                .ForEach(submodels.Add);
        }
        return submodels;

    }

    public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string submodelId)
    {
        return _servicePrividerRegistry.GetSubmodelServiceProvider(submodelId);
    }

    public IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
    {
       return _servicePrividerRegistry.GetSubmodelServiceProviders();
    }

    public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string id, ISubmodelServiceProvider submodelServiceProvider)
    {
        return _servicePrividerRegistry.RegisterSubmodelServiceProvider(id, submodelServiceProvider);
    }

    public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
    {
        if (GetSubmodelServiceProvider(submodelId).TryGetEntity(out ISubmodelServiceProvider serviceProvider))
        {
            return new Result<ISubmodel>(true, serviceProvider.GetBinding());
        }
        return new Result<ISubmodel>(false, new NotFoundMessage($"Submodel Service Provider for submodelId '{submodelId}'"));
    }

    public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
    {
        return new Result<IElementContainer<ISubmodel>>(true, new ElementContainer<ISubmodel>(null, GetBinding()));
    }

    public IResult UnregisterSubmodelServiceProvider(string submodelId)
    {
        return _servicePrividerRegistry.UnregisterSubmodelServiceProvider(submodelId);
    }

    public IResult UpdateSubmodel(string submodelId, ISubmodel submodel)
    {
        return _persistentSubmodels.CreateOrUpdate(submodelId, submodel);
    }
}
