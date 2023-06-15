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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations.ArangoDB;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace BaSyx.API.Clients;

public class ArangoSubmodelRepositoryClient : ISubmodelRepositoryClient
{
    private readonly ArangoStorageClient<ISubmodel> _storageClient;

    public ArangoSubmodelRepositoryClient(ArangoStorageClient<ISubmodel> storageClient)
    {
        _storageClient = storageClient;
    }

    public IResult<ISubmodel> CreateOrUpdateSubmodel(ISubmodel submodel)
    {
        return _storageClient.CreateOrUpdate(submodel.Identification.Id, submodel);
    }

    public IResult DeleteSubmodel(string submodelId)
    {
        return _storageClient.Delete(submodelId);
    }

    public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
    {
        return _storageClient.Retrieve(submodelId);
    }

    public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
    {
        IResult<List<ISubmodel>> result = _storageClient.RetrieveAll();

        if (!result.Success)
        {
            return new Result<IElementContainer<ISubmodel>>(false);
        }
        ElementContainer <ISubmodel> submodels = new();
        return null;

    }


}