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
using BaSyx.API.Clients;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.Components;

public class PersistentSubmodelRepositoryServiceProvider : ISubmodelRepositoryServiceProvider
{

    public IStorageClient<ISubmodel> StorageClient { get; set; }

    public ISubmodelRepositoryDescriptor ServiceDescriptor => throw new System.NotImplementedException();

    public void BindTo(IEnumerable<ISubmodel> element)
    {
        throw new System.NotImplementedException();
    }

    public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
    {
        return this.StorageClient.Retrieve(submodel.Identification.Id);
    }

    public IResult DeleteSubmodel(string submodelId)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<ISubmodel> GetBinding()
    {
        throw new System.NotImplementedException();
    }

    public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string id)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
    {
        throw new System.NotImplementedException();
    }

    public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string id, ISubmodelServiceProvider submodelServiceProvider)
    {
        throw new System.NotImplementedException();
    }

    public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
    {
        throw new System.NotImplementedException();
    }

    public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
    {
        throw new System.NotImplementedException();
    }

    public IResult UnregisterSubmodelServiceProvider(string id)
    {
        throw new System.NotImplementedException();
    }

    public IResult UpdateSubmodel(string submodelId, ISubmodel submodel)
    {
        throw new System.NotImplementedException();
    }
}
