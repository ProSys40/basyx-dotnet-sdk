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
using BaSyx.Utils.Client;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Clients;
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

namespace BaSyx.API.Components;

/// <summary>
/// Provides basic functions for a Submodel persisted in ArangoDB
/// </summary>
public sealed class ArangoSubmodelServiceProvider : DistributedSubmodelServiceProvider
{
    private static readonly ILogger logger = LoggingExtentions.CreateLogger<ArangoSubmodelServiceProvider>();

    public ISubmodelDescriptor ServiceDescriptor { get; internal set; }

    public const int DEFAULT_TIMEOUT = 60000;

    // TODO: can these stay locally or have they to be remotely e.g. on arango?
    private readonly Dictionary<string, MethodCalledHandler> methodCalledHandler;
    private readonly Dictionary<string, SubmodelElementHandler> submodelElementHandler;
    private readonly Dictionary<string, Action<IValue>> updateFunctions;
    private readonly Dictionary<string, EventDelegate> eventDelegates;
    private readonly Dictionary<string, InvocationResponse> invocationResults;

    private readonly ISubmodel _submodel;

    private IMessageClient messageClient;

    public ArangoSubmodelServiceProvider(ISubmodelClientFactory arangoSubmodelClientFactory, ISubmodelDescriptor arangoServiceDescriptor) : base(arangoSubmodelClientFactory, arangoServiceDescriptor)
    {
    }

    public void BindTo(ISubmodel submodel)
    {
        if (GetRemoteSubmodel(submodel) == null)
        {
            PutRemoteSubmodel(submodel);
        }
        ServiceDescriptor = new SubmodelDescriptor(submodel, null); // TODO: Do We need an ArangoSubmodelDescriptor? / Do We need an Endpoint?
    }

    private void PutRemoteSubmodel(ISubmodel submodel)
    {   //TODO ASAP
        throw new NotImplementedException();
    }

    private ISubmodel GetRemoteSubmodel(ISubmodel submodel)
    {   //TODO ASAP
        throw new NotImplementedException();
    }


    public void ConfigureEventHandler(IMessageClient messageClient)
    {
        throw new NotImplementedException();
    }

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string pathToSubmodelElement, ISubmodelElement submodelElement)
        // TODO: create Aranfo Submodel Element hander and use here!
        => CreateOrUpdateSubmodelElement(pathToSubmodelElement, submodelElement, new SubmodelElementHandler(submodelElement.Get, submodelElement.Set));

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string pathToSubmodelElement, ISubmodelElement submodelElement, SubmodelElementHandler submodelElementHandler)
    {   //TODO: create ArangoSubmodelElementHandler and use in Signature!
        var submodel = GetBinding();

        if (submodel == null)
        {
            return new Result<ISubmodelElement>(false, new NotFoundMessage("Submodel"));
        }

        IResult<ISubmodelElement> submodelElementResult = submodel.SubmodelElements.CreateOrUpdate(pathToSubmodelElement, submodelElement);
        if (submodelElementResult.Success && submodelElementResult.Entity != null)
            RegisterSubmodelElementHandler(pathToSubmodelElement, submodelElementHandler);
        return submodelElementResult;
    }

    public IResult DeleteSubmodelElement(string submodelElementIdShortPath)
    {
        var submodel = GetBinding();

        if (submodel == null)
        {
            return new Result(false, new NotFoundMessage("Submodel"));
        }

        if (submodel.SubmodelElements == null)
        {
            return new Result(false, new NotFoundMessage(submodelElementIdShortPath));
        }
        IResult deletionResult = submodel.SubmodelElements.Delete(submodelElementIdShortPath);
        if (deletionResult.Success)
            UnregisterSubmodelElementHandler(submodelElementIdShortPath);
        return deletionResult;
    }

    private void UnregisterSubmodelElementHandler(string pathToElement)
    {
        if (submodelElementHandler.ContainsKey(pathToElement))
            submodelElementHandler.Remove(pathToElement);
    }

    public new ISubmodel GetBinding() => base.GetBinding();

    public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {
        string key = GenerateCompositeKey(operationIdShortPath, requestId);
        if (!invocationResults.ContainsKey(key))
        {
            return new Result<InvocationResponse>(false, new NotFoundMessage($"Request with id {requestId}"));
        }
        return new Result<InvocationResponse>(true, invocationResults[key]);
    }

    private string GenerateCompositeKey(params string[] parts)
    {
        return String.Join(":", parts);
    }

    public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        var submodel = GetBinding();

        if (submodel == null)
            return new Result<InvocationResponse>(false, new NotFoundMessage("Submodel"));

        IResult<IOperation> operationResult = submodel.SubmodelElements.Retrieve<IOperation>(operationIdShortPath);
        
        if (!operationResult.Success || operationResult.Entity == null)
        {
            return new Result<InvocationResponse>(operationResult);
        }

        MethodCalledHandler methodHandler;
        if (methodCalledHandler.TryGetValue(operationIdShortPath, out MethodCalledHandler handler))
        {
            methodHandler = handler;
        }
        else if (operationResult.Entity.OnMethodCalled != null)
        {
            methodHandler = operationResult.Entity.OnMethodCalled;
        }
        else
        {
            return new Result<InvocationResponse>(false, new NotFoundMessage($"MethodHandler for {operationIdShortPath}"));
        }

        IOperation operation = operationResult.Entity;
        InvocationResponse invocationResponse = InvocationResponseFromOperation(invocationRequest, operation);

        int timeout = handleInvocationRequestTiemout(invocationRequest, DEFAULT_TIMEOUT);

        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
        {
            Task<OperationResult> runner = Task.Run(async () =>
            {
                return await TryInvokeOperation(methodHandler, operation, invocationRequest, invocationResponse, cancellationTokenSource);
            }, cancellationTokenSource.Token);

            if (Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)).Result != runner)
            {
                cancellationTokenSource.Cancel();
                invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                invocationResponse.ExecutionState = ExecutionState.Timeout;
            }
            else
            {
                cancellationTokenSource.Cancel();
                invocationResponse.OperationResult = runner.Result;
                var executionState = invocationResponse.ExecutionState;
                if (invocationResponse.ExecutionState != ExecutionState.Failed)
                    invocationResponse.ExecutionState = ExecutionState.Completed;
            }
            return new Result<InvocationResponse>(true, invocationResponse);
        }
    }

    private InvocationResponse InvocationResponseFromOperation(InvocationRequest invocationRequest, IOperation operation)
    {
        return new InvocationResponse(invocationRequest.RequestId)
        {
            InOutputArguments = invocationRequest.InOutputArguments,
            OutputArguments = ToTypedOperationVariableSet(operation.OutputVariables)
        };
    }

    private IOperationVariableSet ToTypedOperationVariableSet(IOperationVariableSet outputVariables)
    {
        if (outputVariables == null)
            return null;
        List<ISubmodelElement> typedSubmodelElements = outputVariables.Select(outputVariable => OperationVariableToSubmodelElement(outputVariable)).ToList<ISubmodelElement>();
        return submodelElementsToOperationVariableSet(typedSubmodelElements);
    }

    private ISubmodelElement OperationVariableToSubmodelElement(IOperationVariable outputVariable)
    {
        DataType dataType;
        if (outputVariable.Value is IProperty property)
            dataType = property.ValueType;
        else if (outputVariable.Value is IRange range)
            dataType = range.ValueType;
        else
            dataType = null;
        return SubmodelElementFactory.CreateSubmodelElement(outputVariable.Value.IdShort, outputVariable.Value.ModelType, dataType);
    }

    private IOperationVariableSet submodelElementsToOperationVariableSet(List<ISubmodelElement> submodelElements)
    {
        OperationVariableSet variables = new OperationVariableSet();
        submodelElements.ForEach(submodelElement => variables.Add(submodelElement));
        return variables;
    }

    public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        ISubmodel submodel = GetBinding();
        if (submodel == null)
        {
            return new Result<CallbackResponse>(false, new NotFoundMessage("Submodel"));
        }
        if (invocationRequest == null)
        {
            return new Result<CallbackResponse>(new ArgumentNullException(nameof(invocationRequest)));
        }

        IResult<IOperation> operationResult = submodel.SubmodelElements.Retrieve<IOperation>(operationIdShortPath);

        if (!operationResult.Success || operationResult.Entity == null)
        {
            return new Result<CallbackResponse>(operationResult);
        }

        MethodCalledHandler methodHandler;
        if (methodCalledHandler.TryGetValue(operationIdShortPath, out MethodCalledHandler handler))
            methodHandler = handler;
        else if (operationResult.Entity.OnMethodCalled != null)
            methodHandler = operationResult.Entity.OnMethodCalled;
        else
            return new Result<CallbackResponse>(false, new NotFoundMessage($"MethodHandler for {operationIdShortPath}"));

        Task invocationTask = Task.Run(async () =>
        {
            IOperation operation = operationResult.Entity;

            InvocationResponse invocationResponse = InvocationResponseFromOperation(invocationRequest, operation);
            SetInvocationResult(operationIdShortPath, invocationRequest.RequestId, ref invocationResponse);

            int timeout = handleInvocationRequestTiemout(invocationRequest, DEFAULT_TIMEOUT);

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Task<OperationResult> runner = Task.Run(async () =>
                {
                    return await TryInvokeOperation(methodHandler, operation, invocationRequest, invocationResponse, cancellationTokenSource);
                }, cancellationTokenSource.Token);

                if (await Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)) != runner)
                {
                    cancellationTokenSource.Cancel();
                    invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                    invocationResponse.ExecutionState = ExecutionState.Timeout;
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    invocationResponse.OperationResult = runner.Result;
                    if (invocationResponse.ExecutionState != ExecutionState.Failed)
                        invocationResponse.ExecutionState = ExecutionState.Completed;
                }
            }
        });

        CallbackResponse callbackResponse = GenetateCallbackResponse(operationIdShortPath, invocationRequest);
        return new Result<CallbackResponse>(true, callbackResponse);
    }

    private CallbackResponse GenetateCallbackResponse(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        CallbackResponse callbackResponse = new CallbackResponse(invocationRequest.RequestId);
        string endpoint = ServiceDescriptor?.Endpoints?.FirstOrDefault()?.Address;
        if (string.IsNullOrEmpty(endpoint))
            callbackResponse.CallbackUrl = new Uri($"/submodelElements/{operationIdShortPath}/invocationList/{invocationRequest.RequestId}", UriKind.Relative);
        else
            callbackResponse.CallbackUrl = new Uri($"{endpoint}/submodelElements/{operationIdShortPath}/invocationList/{invocationRequest.RequestId}", UriKind.Absolute);
        return callbackResponse;
    }

    private static async Task<OperationResult> TryInvokeOperation(MethodCalledHandler methodHandler, IOperation operation, InvocationRequest invocationRequest, InvocationResponse invocationResponse,  CancellationTokenSource cancellationTokenSource)
    {
        try
        {
            invocationResponse.ExecutionState = ExecutionState.Running;
            return await methodHandler.Invoke(operation, invocationRequest.InputArguments, invocationResponse.InOutputArguments, invocationResponse.OutputArguments, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            invocationResponse.ExecutionState = ExecutionState.Failed;
            return new OperationResult(e);
        }
    }

    private static int handleInvocationRequestTiemout(InvocationRequest invocationRequest, int defaultTimeout)
    {
        return invocationRequest.Timeout.HasValue ? invocationRequest.Timeout.Value : defaultTimeout;
    }

    private void SetInvocationResult(string operationId, string requestId, ref InvocationResponse invocationResponse)
    {
        string key = GenerateCompositeKey(operationId, requestId);
        if (invocationResults.ContainsKey(key))
        {
            invocationResults[key] = invocationResponse;
        }
        else
        {
            invocationResults.Add(key, invocationResponse);
        }
    }

    public void PublishUpdate(string pathToSubmodelElement, IValue value)
    {
        if (updateFunctions.TryGetValue(pathToSubmodelElement, out Action<IValue> updateFunction))
            updateFunction.Invoke(value);
    }

    public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
    {
        if (!eventDelegates.ContainsKey(pathToEvent))
            eventDelegates.Add(pathToEvent, eventDelegate);
        else
            eventDelegates[pathToEvent] = eventDelegate;
    }

    public IResult PublishEvent(IEventMessage eventMessage) =>  PublishEventAsync(eventMessage).Result;

    public async Task<IResult> PublishEventAsync(IEventMessage eventMessage)
    {
        if (messageClient == null || !messageClient.IsConnected)
            return new Result(false, new Message(MessageType.Warning, "MessageClient is not initialized or not connected"));

        if (eventMessage == null)
            return new Result(new ArgumentNullException(nameof(eventMessage)));

        if (eventDelegates.TryGetValue(eventMessage.SourceIdShort, out EventDelegate eventDelegate))
            eventDelegate.Invoke(this, eventMessage);

        try
        {
            string message = JsonConvert.SerializeObject(eventMessage, Formatting.Indented);
            return await messageClient.PublishAsync(eventMessage.Topic, message).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending event message");
            return new Result(e);
        }
    }

    public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler handler)
    {
        if (!methodCalledHandler.ContainsKey(pathToOperation))
            methodCalledHandler.Add(pathToOperation, handler);
        else
            methodCalledHandler[pathToOperation] = handler;
    }

    public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
    {
        if (!submodelElementHandler.ContainsKey(pathToElement))
        {
            submodelElementHandler.Add(pathToElement, elementHandler);
            return;
        }
        submodelElementHandler[pathToElement] = elementHandler;
    }

    public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
    {
        if (!methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
        {
            return null;
        }
        return handler;
        
    }

    public IResult<ISubmodel> RetrieveSubmodel()
    {
        ISubmodel submodel = GetBinding();
        return new Result<ISubmodel>(submodel != null, submodel);
    }

    public IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath)
    {
        throw new NotImplementedException();
    }

    public SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement)
    {
        throw new NotImplementedException();
    }

    public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
    {
        throw new NotImplementedException();
    }

    public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
    {
        throw new NotImplementedException();
    }

    public void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction)
    {
        throw new NotImplementedException();
    }

    public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
    {
        throw new NotImplementedException();
    }
}
