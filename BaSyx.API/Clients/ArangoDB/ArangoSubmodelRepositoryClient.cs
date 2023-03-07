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
using System.Reflection.Metadata;

namespace BaSyx.API.Clients;

public class ArangoSubmodelRepositoryClient : ISubmodelRepositoryClient
{
    private readonly AsyncArangoAPIWrapper _storageClient;

    public ArangoSubmodelRepositoryClient(AsyncArangoAPIWrapper storageClient)
    {
        _storageClient = storageClient;
    }

    public IResult<ISubmodel> CreateOrUpdateSubmodel(ISubmodel submodel)
    {
        SubmodelWithArangoKey arangoSubmodel = (SubmodelWithArangoKey)ArangoSubmodelFactory.Create(submodel);
        var createSubmodelResponse = new object(); // _storageClient.CreateDocument<SubmodelWithArangoKey>(AsyncArangoAPIWrapper.SUBMODEL_COLLECTION, arangoSubmodel);


        //IResult<ISubmodel> submodelResult = _storageClient.CreateDocument<ISubmodel>(ArangoAPIWrapper.SUBMODEL_COLLECTION, submodel);
        //return submodelResult;
        return null;
        
    }

    private IResult<IElementContainer<ISubmodelElement>> HandleAssociations(IElementContainer<ISubmodelElement> submodelElements, ISubmodel submodel)
    {
        throw new NotImplementedException();
    }

    public IResult DeleteSubmodel(string submodelId)
    {
        return null;
        //return _storageClient.Delete(submodelId);
    }

    public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
    {
        return null;
        //return _storageClient.Retrieve<ISubmodel>(submodelId);
    }

    public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
    {
        return null;
        //return _storageClient.RetrieveAll<IElementContainer<ISubmodel>>();
    }


}