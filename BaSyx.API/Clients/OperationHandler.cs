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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.API.Clients;

internal class OperationHandler
{
    public const int DEFAULT_TIMEOUT = 60000;

    private readonly static Dictionary<string, InvocationResponse> invocationResults = new();
    private readonly static Dictionary<string, MethodCalledHandler> methodCalledHandler = new();

    public static IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {
        string key = string.Join("_", operationIdShortPath, requestId);

        return invocationResults.ContainsKey(key) ?
            new Result<InvocationResponse>(true, invocationResults[key]) :
            new Result<InvocationResponse>(false, new NotFoundMessage($"Request with id {requestId}"));
    }

    public static IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, ISubmodel submodel, InvocationRequest invocationRequest)
    {
        if (submodel == null)
            return new Result<InvocationResponse>(false, new NotFoundMessage("Submodel"));

        var operation_Retrieved = submodel.SubmodelElements.Retrieve<IOperation>(operationIdShortPath);
        if (operation_Retrieved.Success && operation_Retrieved.Entity != null)
        {
            MethodCalledHandler methodHandler;
            if (methodCalledHandler.TryGetValue(operationIdShortPath, out MethodCalledHandler handler))
                methodHandler = handler;
            else if (operation_Retrieved.Entity.OnMethodCalled != null)
                methodHandler = operation_Retrieved.Entity.OnMethodCalled;
            else
                return new Result<InvocationResponse>(false, new NotFoundMessage($"MethodHandler for {operationIdShortPath}"));

            InvocationResponse invocationResponse = new InvocationResponse(invocationRequest.RequestId);
            invocationResponse.InOutputArguments = invocationRequest.InOutputArguments;
            invocationResponse.OutputArguments = CreateOutputArguments(operation_Retrieved.Entity.OutputVariables);

            int timeout = DEFAULT_TIMEOUT;
            if (invocationRequest.Timeout.HasValue)
                timeout = invocationRequest.Timeout.Value;

            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                Task<OperationResult> runner = Task.Run(async () =>
                {
                    try
                    {
                        invocationResponse.ExecutionState = ExecutionState.Running;
                        return await methodHandler.Invoke(operation_Retrieved.Entity, invocationRequest.InputArguments, invocationResponse.InOutputArguments, invocationResponse.OutputArguments, cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        invocationResponse.ExecutionState = ExecutionState.Failed;
                        return new OperationResult(e);
                    }

                }, cancellationTokenSource.Token);

                if (Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)).Result == runner)
                {
                    cancellationTokenSource.Cancel();

                    invocationResponse.OperationResult = runner.Result;
                    if (invocationResponse.ExecutionState != ExecutionState.Failed)
                        invocationResponse.ExecutionState = ExecutionState.Completed;

                    return new Result<InvocationResponse>(true, invocationResponse);
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                    invocationResponse.ExecutionState = ExecutionState.Timeout;
                    return new Result<InvocationResponse>(true, invocationResponse);
                }
            }
        }
        return new Result<InvocationResponse>(operation_Retrieved);
    }

    private static IOperationVariableSet CreateOutputArguments(IOperationVariableSet outputVariables)
    {
        if (outputVariables == null)
            return null;

        OperationVariableSet variables = new OperationVariableSet();
        if (outputVariables.Count > 0)
        {
            foreach (var outputVariable in outputVariables)
            {
                DataType dataType;
                if (outputVariable.Value is IProperty property)
                    dataType = property.ValueType;
                else if (outputVariable.Value is IRange range)
                    dataType = range.ValueType;
                else
                    dataType = null;

                var se = SubmodelElementFactory.CreateSubmodelElement(outputVariable.Value.IdShort, outputVariable.Value.ModelType, dataType);
                variables.Add(se);
            }
        }
        return variables;
    }

    public static IResult<CallbackResponse> InvokeOperationAsync(string pathToOperation, ISubmodel submodel, ISubmodelDescriptor serviceDescriptor, InvocationRequest invocationRequest)
    {
        if (submodel == null)
            return new Result<CallbackResponse>(false, new NotFoundMessage("Submodel"));
        if (invocationRequest == null)
            return new Result<CallbackResponse>(new ArgumentNullException(nameof(invocationRequest)));

        var operation_Retrieved = submodel.SubmodelElements.Retrieve<IOperation>(pathToOperation);
        if (operation_Retrieved.Success && operation_Retrieved.Entity != null)
        {
            MethodCalledHandler methodHandler;
            if (methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
                methodHandler = handler;
            else if (operation_Retrieved.Entity.OnMethodCalled != null)
                methodHandler = operation_Retrieved.Entity.OnMethodCalled;
            else
                return new Result<CallbackResponse>(false, new NotFoundMessage($"MethodHandler for {pathToOperation}"));

            Task invocationTask = Task.Run(async () =>
            {
                InvocationResponse invocationResponse = new InvocationResponse(invocationRequest.RequestId);
                invocationResponse.InOutputArguments = invocationRequest.InOutputArguments;
                SetInvocationResult(pathToOperation, invocationRequest.RequestId, ref invocationResponse);

                int timeout = DEFAULT_TIMEOUT;
                if (invocationRequest.Timeout.HasValue)
                    timeout = invocationRequest.Timeout.Value;

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    Task<OperationResult> runner = Task.Run(async () =>
                    {
                        try
                        {
                            invocationResponse.ExecutionState = ExecutionState.Running;
                            return await methodHandler.Invoke(operation_Retrieved.Entity, invocationRequest.InputArguments, invocationResponse.InOutputArguments, invocationResponse.OutputArguments, cancellationTokenSource.Token);
                        }
                        catch (Exception e)
                        {
                            invocationResponse.ExecutionState = ExecutionState.Failed;
                            return new OperationResult(e);
                        }
                    }, cancellationTokenSource.Token);

                    if (await Task.WhenAny(runner, Task.Delay(timeout, cancellationTokenSource.Token)) == runner)
                    {
                        cancellationTokenSource.Cancel();
                        invocationResponse.OperationResult = runner.Result;
                        if (invocationResponse.ExecutionState != ExecutionState.Failed)
                            invocationResponse.ExecutionState = ExecutionState.Completed;
                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                        invocationResponse.OperationResult = new OperationResult(false, new TimeoutMessage());
                        invocationResponse.ExecutionState = ExecutionState.Timeout;
                    }
                }
            });

            string endpoint = serviceDescriptor?.Endpoints?.FirstOrDefault()?.Address;
            CallbackResponse callbackResponse = new CallbackResponse(invocationRequest.RequestId);
            if (string.IsNullOrEmpty(endpoint))
                callbackResponse.CallbackUrl = new Uri($"/submodelElements/{pathToOperation}/invocationList/{invocationRequest.RequestId}", UriKind.Relative);
            else
                callbackResponse.CallbackUrl = new Uri($"{endpoint}/submodelElements/{pathToOperation}/invocationList/{invocationRequest.RequestId}", UriKind.Absolute);
            return new Result<CallbackResponse>(true, callbackResponse);
        }
        return new Result<CallbackResponse>(operation_Retrieved);
    }

    private static void SetInvocationResult(string operationId, string requestId, ref InvocationResponse invocationResponse)
    {
        string key = string.Join("_", operationId, requestId);
        if (invocationResults.ContainsKey(key))
        {
            invocationResults[key] = invocationResponse;
        }
        else
        {
            invocationResults.Add(key, invocationResponse);
        }
    }

    public static void RegisterMethodCalledHandler(string operationIdShortPath, MethodCalledHandler handler)
    {
        if (!methodCalledHandler.ContainsKey(operationIdShortPath))
            methodCalledHandler.Add(operationIdShortPath, handler);
        else
            methodCalledHandler[operationIdShortPath] = handler;
    }

    public static MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
    {
        if (methodCalledHandler.TryGetValue(pathToOperation, out MethodCalledHandler handler))
            return handler;
        else
            return null;
    }
}
