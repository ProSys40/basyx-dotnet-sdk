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

using BaSyx.Models.Communication;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Clients;

public class ArangoSubmodelClient : ISubmodelClient
{
    private readonly ArangoSubmodelRepositoryClient _repositoryClient;

    public ArangoSubmodelClient(ArangoSubmodelRepositoryClient repositoryClient)
    {
        _repositoryClient = repositoryClient;
    }

    public IResult<ISubmodel> RetrieveSubmodel()
    {
        throw new NotImplementedException();
    }

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
    {
        throw new NotImplementedException();
    }

    public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
    {
        throw new NotImplementedException();
    }

    public IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath)
    {
        throw new NotImplementedException();
    }

    public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
    {
        throw new NotImplementedException();
    }

    public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
    {
        throw new NotImplementedException();
    }

    public IResult DeleteSubmodelElement(string seIdShortPath)
    {
        throw new NotImplementedException();
    }

    public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        throw new NotImplementedException();
    }

    public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        throw new NotImplementedException();
    }

    public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {
        throw new NotImplementedException();
    }
}
