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

using BaSyx.Utils.Client;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.API.AssetAdministrationShell;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.API.Clients.Persistency;
using BaSyx.API.Clients;
using Serilog.Core;

namespace BaSyx.API.Components;

/// <summary>
/// Provides basic functions for a Submodel persisted submodel
/// </summary>
public sealed class PersistentSubmodelServiceProvider : SubmodelServiceProvider
{
    private ISubmodelClient submodelClient;

    internal PersistentSubmodelServiceProvider(ISubmodel submodel) : base(submodel)
    {
        UseInMemorySubmodelElementHandler();
    }

    public IPersistentCompositeKeyCollection<string, SubmodelElement> SubmodelElements { get; set; }

    public PersistentSubmodelServiceProvider() : base()
    {
    }

    public PersistentSubmodelServiceProvider(ISubmodel submodel, ISubmodelDescriptor submodelDescriptor) : base(submodel, submodelDescriptor)
    {
    }

    public ISubmodelDescriptor ServiceDescriptor { get; internal set; }

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
    {
        return submodelClient.CreateOrUpdateSubmodelElement(rootSeIdShortPath, submodelElement);
    }

    public IResult DeleteSubmodelElement(string seIdShortPath)
    {
        return submodelClient.DeleteSubmodelElement(seIdShortPath);
    }

    public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {
        return submodelClient.GetInvocationResult(operationIdShortPath, requestId);
    }

    public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        return submodelClient.InvokeOperation(operationIdShortPath, invocationRequest);
    }

    public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        return submodelClient.InvokeOperationAsync(operationIdShortPath, invocationRequest);
    }

    public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
    {
        return submodelClient.RetrieveSubmodelElements();
    }

    public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
    {
        return submodelClient.RetrieveSubmodelElementValue(seIdShortPath);
    }

    public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
    {
        return submodelClient.UpdateSubmodelElementValue(seIdShortPath, value);
    }
}
