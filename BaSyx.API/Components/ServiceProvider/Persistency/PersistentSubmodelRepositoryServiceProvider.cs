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
using BaSyx.API.Clients;
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

namespace BaSyx.API.Components;

public class PersistentSubmodelRepositoryServiceProvider : ISubmodelRepositoryServiceProvider
{
    private readonly ISubmodelServiceProviderFactory _serviceProviderFactory;

    public IStorageClient<ISubmodel> StorageClient {get; set; }

    public PersistentSubmodelRepositoryServiceProvider() {
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
            .ForEach(submodel => {
                UpdateSubmodel(submodel.Identification.Id, submodel);
             });
    }

    public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
    {
        return StorageClient.CreateOrUpdate(submodel.Identification.Id, submodel);
    }

    public IResult DeleteSubmodel(string submodelId)
    {
        if (string.IsNullOrEmpty(submodelId))
        {
            return new Result<ISubmodel>(new ArgumentNullException(nameof(submodelId)));
        }

        return StorageClient.Delete(submodelId);
    }

    public IEnumerable<ISubmodel> GetBinding()
    {
        IResult<List<ISubmodel>> result = StorageClient.RetrieveAll();
        if (!result.Success || result.Entity.Count == 0)
        {
            return new List<ISubmodel>();
        }
        return result.Entity;
    }

    public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string submodelId)
    {
        IResult<ISubmodel> submodelResult = RetrieveSubmodel(submodelId);
        ISubmodel submodel = ExtractSubmodel(submodelResult);

        if (submodel == null)
        {
            return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(submodelId));
        }
        return new Result<ISubmodelServiceProvider>(true, CreateSubmodelSrviceProvider(submodel));
    }

    public IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
    {
        IEnumerable<ISubmodel> submodels = GetBinding();
        List<ISubmodelServiceProvider> submodelServiceProviders = submodels.Select(submodel => CreateSubmodelSrviceProvider(submodel)).ToList();
        return new Result<IEnumerable<ISubmodelServiceProvider>>(true, submodelServiceProviders);
    }

    public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string id, ISubmodelServiceProvider submodelServiceProvider)
    {
        ISubmodel submodel = submodelServiceProvider.GetBinding();
        StorageClient.CreateOrUpdate(id, submodel);
        return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
    }

    public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
    {
        return StorageClient.Retrieve(submodelId);
    }

    public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
    {
        // TODO: könnte in abstrakte Klasse
        return new Result<IElementContainer<ISubmodel>>(true, new ElementContainer<ISubmodel>(null, GetBinding()));
    }

    public IResult UnregisterSubmodelServiceProvider(string submodelId)
    {
        // TODO: should this delete the submodel from the storage?
        //       if not: adapt this method!!!
        return DeleteSubmodel(submodelId);
    }

    public IResult UpdateSubmodel(string submodelId, ISubmodel submodel)
    {
        return StorageClient.CreateOrUpdate(submodelId, submodel);
    }

    private ISubmodel ExtractSubmodel(IResult<ISubmodel> submodelResult)
    {
        if (!submodelResult.Success || submodelResult.Entity == null)
        {
            return null;
        }
        return submodelResult.Entity;
    }

    private ISubmodelServiceProvider CreateSubmodelSrviceProvider(ISubmodel submodel)
    {
        return _serviceProviderFactory.CreateSubmodelServiceProvider(submodel);
    }
}
