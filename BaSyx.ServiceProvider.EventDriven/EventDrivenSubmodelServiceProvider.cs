// /*******************************************************************************
// * Copyright (c) 2022 LTSoft - Agentur für Leittechnik-Software GmbH
// * Author: Björn Höper
// *
// * This program and the accompanying materials are made available under the
// * terms of the Eclipse Public License 2.0 which is available at
// * http://www.eclipse.org/legal/epl-2.0
// *
// * SPDX-License-Identifier: EPL-2.0
// *******************************************************************************/

using System.Reactive.Subjects;
using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Components;
using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.Client;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;

namespace BaSyx.ServiceProvider.EventDriven;

/// <summary>
/// Submodel service provider that generates events for manipulations of the submodels
/// </summary>
/// <typeparam name="TPersisting">Type of persisting service provider</typeparam>
public class EventDrivenSubmodelServiceProvider<TPersisting>: ISubmodelServiceProvider, ISubmodelEventPublisher 
    where TPersisting: ISubmodelServiceProvider
{
    private readonly ILogger<EventDrivenSubmodelServiceProvider<TPersisting>> _logger;
    private readonly TPersisting _persistingServiceProvider;

    private readonly Subject<SubmodelEventData> _submodelSubject = new Subject<SubmodelEventData>();

    public EventDrivenSubmodelServiceProvider(ILogger<EventDrivenSubmodelServiceProvider<TPersisting>> logger, 
        TPersisting persistingServiceProvider)
    {
        _logger = logger;
        _persistingServiceProvider = persistingServiceProvider;
    }

    public EventDrivenSubmodelServiceProvider(ILogger<EventDrivenSubmodelServiceProvider<TPersisting>> logger, TPersisting persistingServiceProvider, 
        ISubmodel submodel): this(logger, persistingServiceProvider)
    {
        BindTo(submodel);
    }

    public ISubmodelDescriptor ServiceDescriptor => _persistingServiceProvider.ServiceDescriptor;
    

    public void BindTo(ISubmodel boundElement)
    {
        _logger.LogDebug("Bound service provider to submodel {SubmodelIdentifier}", boundElement.Identification);
        _persistingServiceProvider.BindTo(boundElement);
    }

    public ISubmodel GetBinding()
    {
        return _persistingServiceProvider.GetBinding();
    }

    public IResult<ISubmodel> RetrieveSubmodel()
    {
        return _persistingServiceProvider.RetrieveSubmodel();
    }

    public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
    {
        var result = _persistingServiceProvider.CreateOrUpdateSubmodelElement(rootSeIdShortPath, submodelElement);
        if (!result.Success)
        {
            //TODO: How to check if created or updated...
            return result;
        }

        _logger.LogDebug("Updated submodel element {SubmodelShortPath}", rootSeIdShortPath);

        var createdEvent = new CreateOrUpdateSubmodelEventData(rootSeIdShortPath);
        _submodelSubject.OnNext(createdEvent);

        return result;
    }

    public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
    {
        _logger.LogDebug("Retrieving submodel elements from persisting provider");
        return _persistingServiceProvider.RetrieveSubmodelElements();
    }

    public IResult<ISubmodelElement> RetrieveSubmodelElement(string seIdShortPath)
    {
        return _persistingServiceProvider.RetrieveSubmodelElement(seIdShortPath);
    }

    public IResult<IValue> RetrieveSubmodelElementValue(string seIdShortPath)
    {
        return _persistingServiceProvider.RetrieveSubmodelElementValue(seIdShortPath);
    }

    public IResult UpdateSubmodelElementValue(string seIdShortPath, IValue value)
    {
        var result = _persistingServiceProvider.UpdateSubmodelElementValue(seIdShortPath, value);

        if (result.Success)
        {
            var updateEvent = new UpdatedValueSubmodelEventData(seIdShortPath, value.ToString());
            _submodelSubject.OnNext(updateEvent);
        }

        return result;
    }

    public IResult DeleteSubmodelElement(string seIdShortPath)
    {
        var result = _persistingServiceProvider.DeleteSubmodelElement(seIdShortPath);

        if (result.Success)
        {
            var deleteEvent = new DeleteSubmodelElementEventData(seIdShortPath);
            _submodelSubject.OnNext(deleteEvent);
        }

        return result;
    }

    public IResult<InvocationResponse> InvokeOperation(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        var invocationResult = _persistingServiceProvider.InvokeOperation(operationIdShortPath, invocationRequest);
        var invocationResponse = invocationResult.Entity;
        
        if (invocationResult.Success)
        {
            var boundSubmodel = GetBinding();

            var operationInvokedEvent = new SubmodelInvokedOperationEventData(boundSubmodel.IdShort, operationIdShortPath, 
                invocationRequest, invocationResponse);
            _submodelSubject.OnNext(operationInvokedEvent);
        }

        return invocationResult;
    }

    public IResult<CallbackResponse> InvokeOperationAsync(string operationIdShortPath, InvocationRequest invocationRequest)
    {
        var invocationResult = _persistingServiceProvider.InvokeOperationAsync(operationIdShortPath, invocationRequest);

        if (invocationResult.Success)
        {
            var boundSubmodel = GetBinding();


            var invocationEvent = new SubmodelInvokedOperationAsyncEventData(boundSubmodel.IdShort,
                operationIdShortPath, invocationRequest, invocationResult.Entity);
            _submodelSubject.OnNext(invocationEvent);
        }

        return invocationResult;
    }

    public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
    {
        _logger.LogDebug("Requested invocation result from service provider for {RequestId}", requestId);
        return _persistingServiceProvider.GetInvocationResult(operationIdShortPath, requestId);
    }

    public void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction)
    {
        _logger.LogDebug("Subscribing for submodel element {ElementPath}", pathToSubmodelElement);
        _persistingServiceProvider.SubscribeUpdates(pathToSubmodelElement, updateFunction);
    }

    public void PublishUpdate(string pathToSubmodelElement, IValue value)
    {
        _logger.LogDebug("Publishing update with {ElementValue} on {ElementPath} requested", value, 
            pathToSubmodelElement);
        
        _persistingServiceProvider.PublishUpdate(pathToSubmodelElement, value);
    }

    public IResult PublishEvent(IEventMessage eventMessage)
    {
        var publishResult = _persistingServiceProvider.PublishEvent(eventMessage);

        if (publishResult.Success)
        {
            var boundSubmodel = GetBinding();
            var eventMessageWrapper = new SubmodelEventInvokedEventData(boundSubmodel.IdShort, eventMessage);
            
            _submodelSubject.OnNext(eventMessageWrapper);
        }

        return publishResult;
    }

    public SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement)
    {
        _logger.LogDebug("Retrieving submodel element handler for {ElementPath}", pathToElement);
        return _persistingServiceProvider.RetrieveSubmodelElementHandler(pathToElement);
    }

    public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
    {
        _logger.LogDebug("Registering submodel element handler for {ElementPath}", pathToElement);
        _persistingServiceProvider.RegisterSubmodelElementHandler(pathToElement, elementHandler);
    }

    public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
    {
        _logger.LogDebug("Retrieving method called Handler for {OperationPath}", pathToOperation);
        return _persistingServiceProvider.RetrieveMethodCalledHandler(pathToOperation);
    }

    public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler methodCalledHandler)
    {
        _logger.LogDebug("Registering method called Handler for {OperationPath}", pathToOperation);
        _persistingServiceProvider.RegisterMethodCalledHandler(pathToOperation, methodCalledHandler);
        _logger.LogDebug("Successfully registered method called Handler for {OperationPath}", pathToOperation);
    }

    public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
    {
        _logger.LogDebug("Registering event delegate for event {EventPath}", pathToEvent);
        _persistingServiceProvider.RegisterEventDelegate(pathToEvent, eventDelegate);
    }

    public void ConfigureEventHandler(IMessageClient messageClient)
    {
        //TODO: Check if other implementation needed
        _logger.LogDebug("Configuring event handler on message client {MessageClient}", messageClient);
        
        _persistingServiceProvider.ConfigureEventHandler(messageClient);
    }

    ///<inheritdoc cref="ISubmodelEventPublisher"/>
    public IObservable<SubmodelEventData> SubmodelEventObservable => _submodelSubject;
}