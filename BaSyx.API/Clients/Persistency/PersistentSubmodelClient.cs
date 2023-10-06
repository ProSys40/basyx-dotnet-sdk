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

using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.API.Clients.Persistency;

internal class PersistentSubmodelClient : ISubmodelClient
{

    private ISubmodel _submodel;

    public const int DEFAULT_TIMEOUT = 60000;

    private IStorageClient storageClient;
    private IPersistentCompositeKeyCollection<string, ISubmodelElement> submodelElements;

    private readonly Dictionary<string, SubmodelElementHandler> submodelElementHandler;

    public ISubmodelDescriptor ServiceDescriptor { get; internal set; }

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
    {
        CompositeKey<string> submodelElementKey = new CompositeKey<string>
        {
            Keys = new[] { rootSeIdShortPath, _submodel.IdShort }
        };
        return submodelElements.CreateOrUpdate(submodelElementKey, submodelElement);
    }

    public IResult DeleteSubmodelElement(string seIdShortPath)
    {
        CompositeKey<string> submodelElementKey = new CompositeKey<string>
        {
            Keys = new[] { seIdShortPath, _submodel.IdShort }
        };
        return submodelElements.Delete(submodelElementKey);
    }

    public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {

        return OperationHandler.GetInvocationResult(operationIdShortPath, requestId);
    }

    public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
    {
       return OperationHandler.InvokeOperation(operationIdShortPath, _submodel, invocationRequest);
    }

    public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        return OperationHandler.InvokeOperationAsync(operationIdShortPath, _submodel, ServiceDescriptor, invocationRequest);
    }

    public IResult<ISubmodel> RetrieveSubmodel()
    {
        return new Result<ISubmodel>(_submodel != null, _submodel);
    }

    public IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath)
    {
        CompositeKey<string> submodelElementKey = new CompositeKey<string>
        {
            Keys = new[] { seIdShortPath, _submodel.IdShort }
        };
        return submodelElements.Retrieve(submodelElementKey);
    }

    public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
    {
        return submodelElements.RetrieveAll<ISubmodelElement>();
    }

    public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
    {
        IResult<ISubmodelElement> submodelElementResult = RetrieveSubmodelElement(seIdShortPath);
        if (!submodelElementResult.Success || submodelElementResult.Entity == null)
        {
            return new Result<IValue>(false, new NotFoundMessage($"SubmodelElement for {seIdShortPath}"));
        }

        ISubmodelElement submodelElement = submodelElementResult.Entity;
        if (submodelElementHandler.TryGetValue(seIdShortPath, out SubmodelElementHandler elementHandler) && elementHandler.GetValueHandler != null)
            return new Result<IValue>(true, elementHandler.GetValueHandler.Invoke(submodelElement));
        else if (submodelElement.Get != null)
            return new Result<IValue>(true, submodelElement.Get.Invoke(submodelElement));
        else
            return new Result<IValue>(false, new NotFoundMessage($"SubmodelElementHandler for {seIdShortPath}"));
    }

    public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
    {
        IResult<ISubmodelElement> submodelElementResult = RetrieveSubmodelElement(seIdShortPath);
        if (!submodelElementResult.Success || submodelElementResult.Entity == null)
        {
            return new Result<IValue>(false, new NotFoundMessage($"SubmodelElement for {seIdShortPath}"));
        }

        ISubmodelElement submodelElement = submodelElementResult.Entity;
        if (submodelElementHandler.TryGetValue(seIdShortPath, out SubmodelElementHandler elementHandler) && elementHandler.SetValueHandler != null)
        {
            elementHandler.SetValueHandler.Invoke(submodelElement, value);
            return new Result(true);
        }
        else if (submodelElement.Set != null)
        {
            submodelElement.Set.Invoke(submodelElement, value);
            return new Result(true);
        }
        else
            return new Result(false, new NotFoundMessage($"SubmodelElementHandler for {seIdShortPath}"));
    }
}
