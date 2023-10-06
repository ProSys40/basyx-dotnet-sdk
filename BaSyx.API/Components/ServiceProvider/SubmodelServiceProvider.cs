/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Client;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;
using System.Threading.Tasks;
using System.Threading;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Microsoft.Extensions.Logging;
using BaSyx.API.Clients;

namespace BaSyx.API.Components
{
    /// <summary>
    /// Reference implementation of ISubmodelServiceProvider interface
    /// </summary>
    public class SubmodelServiceProvider : ISubmodelServiceProvider
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<SubmodelServiceProvider>();

        private ISubmodel _submodel;

        public ISubmodelDescriptor ServiceDescriptor { get; internal set; }

        private readonly Dictionary<string, SubmodelElementHandler> submodelElementHandler;
        private readonly Dictionary<string, Action<IValue>> updateFunctions;
        private readonly Dictionary<string, EventDelegate> eventDelegates;
        
        private IMessageClient messageClient;

        /// <summary>
        /// Constructor for SubmodelServiceProvider
        /// </summary>
        public SubmodelServiceProvider()
        {
            submodelElementHandler = new Dictionary<string, SubmodelElementHandler>();
            updateFunctions = new Dictionary<string, Action<IValue>>();
            eventDelegates = new Dictionary<string, EventDelegate>();
        }
        /// <summary>
        /// Contructor for SubmodelServiceProvider with a Submodel object to bind to
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        public SubmodelServiceProvider(ISubmodel submodel) : this()
        {
            BindTo(submodel);
        }
        /// <summary>
        /// Contructor for SubmodelServiceProvider with a Submodel object to bind to and a SubmodelDescriptor as ServiceDescriptor
        /// </summary>
        /// <param name="submodel">Submodel object</param>
        /// <param name="submodelDescriptor">SubmodelDescriptor object</param>
        public SubmodelServiceProvider(ISubmodel submodel, ISubmodelDescriptor submodelDescriptor) : this()
        {
            _submodel = submodel;
            ServiceDescriptor = submodelDescriptor;
        }

        /// <summary>
        /// Bind this SubmodelServiceProvider to a specific Submodel
        /// </summary>
        /// <param name="boundElement">Submodel object</param>
        public void BindTo(ISubmodel boundElement)
        {
            _submodel = boundElement;
            ServiceDescriptor = new SubmodelDescriptor(boundElement, null);
        }

        /// <summary>
        /// Returns the model binding of this SubmodelServiceProvider
        /// </summary>
        /// <returns>Submodel object</returns>
        public ISubmodel GetBinding()
        {
            return _submodel;
        }

        public void UseInMemorySubmodelElementHandler()
        {
            UseInMemorySubmodelElementHandlerInternal(_submodel.SubmodelElements);
        }
        private void UseInMemorySubmodelElementHandlerInternal(IElementContainer<ISubmodelElement> submodelElements)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseInMemorySubmodelElementHandlerInternal(child);
                }
            }
            if(submodelElements.Value != null)
            {
                if (submodelElements.Value.ModelType != ModelType.Operation)
                    RegisterSubmodelElementHandler(submodelElements.Path, new SubmodelElementHandler(submodelElements.Value.Get, submodelElements.Value.Set));                
            }
        }

        /// <summary>
        /// Use as specific SubmodelElementHandler for all SubmodelElements
        /// </summary>
        /// <param name="elementHandler">SubmodelElementHandler</param>
        public void UseSubmodelElementHandler(SubmodelElementHandler elementHandler)
        {
            UseSubmodelElementHandlerInternal(_submodel.SubmodelElements, elementHandler, null);
        }
        /// <summary>
        /// Use a specific SubmodelElementHandler for all SubmodelElements of a specific ModelType (e.g. Property) except Operations
        /// </summary>
        /// <param name="elementHandler">SubmodelElementHandler</param>
        /// <param name="modelType">ModelType</param>
        public void UseSubmodelElementHandlerForModelType(SubmodelElementHandler elementHandler, ModelType modelType)
        {
            UseSubmodelElementHandlerInternal(_submodel.SubmodelElements, elementHandler, modelType);
        }

        private void UseSubmodelElementHandlerInternal(IElementContainer<ISubmodelElement> submodelElements, SubmodelElementHandler elementHandler, ModelType modelType = null)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseSubmodelElementHandlerInternal(child, elementHandler, modelType);
                }
            }
            if (submodelElements.Value != null)
            {
                if (modelType == null)
                    RegisterSubmodelElementHandler(submodelElements.Path, elementHandler);
                else if (submodelElements.Value.ModelType == modelType)
                    RegisterSubmodelElementHandler(submodelElements.Path, elementHandler);
                else
                    return;
            }
        }
        /// <summary>
        /// Use a specific MethodCalledHandler for all Operations
        /// </summary>
        /// <param name="methodCalledHandler"></param>
        public void UseOperationHandler(MethodCalledHandler methodCalledHandler)
        {
            UseOperationHandlerInternal(_submodel.SubmodelElements, methodCalledHandler);
        }

        private void UseOperationHandlerInternal(IElementContainer<ISubmodelElement> submodelElements, MethodCalledHandler methodCalledHandler)
        {
            if (submodelElements.HasChildren())
            {
                foreach (var child in submodelElements.Children)
                {
                    UseOperationHandlerInternal(child, methodCalledHandler);
                }
            }
            else
            {
                if (submodelElements.Value is IOperation)
                    RegisterMethodCalledHandler(submodelElements.Path, methodCalledHandler);
                else
                    return;
            }
        }
        
        public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
        {
            return OperationHandler.RetrieveMethodCalledHandler(pathToOperation);
        }
       
        public SubmodelElementHandler RetrieveSubmodelElementHandler(string pathToElement)
        {
            if (submodelElementHandler.TryGetValue(pathToElement, out SubmodelElementHandler elementHandler))
                return elementHandler;
            else
                return null;
        }
      
        public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
        {
            if (!submodelElementHandler.ContainsKey(pathToElement))
                submodelElementHandler.Add(pathToElement, elementHandler);
            else
                submodelElementHandler[pathToElement] = elementHandler;
        }

        public void UnregisterSubmodelElementHandler(string pathToElement)
        {
            if (submodelElementHandler.ContainsKey(pathToElement))
                submodelElementHandler.Remove(pathToElement);
        }

        public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler handler)
        {
            OperationHandler.RegisterMethodCalledHandler(pathToOperation, handler);
        }

        public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
        {
            if (!eventDelegates.ContainsKey(pathToEvent))
                eventDelegates.Add(pathToEvent, eventDelegate);
            else
                eventDelegates[pathToEvent] = eventDelegate;
        }

        public IResult<InvocationResponse> InvokeOperation(string pathToOperation, InvocationRequest invocationRequest)
        {
            return OperationHandler.InvokeOperation(pathToOperation, _submodel, invocationRequest);
        }

        private IOperationVariableSet CreateOutputArguments(IOperationVariableSet outputVariables)
        {
            if (outputVariables == null)
                return null;

            OperationVariableSet variables = new OperationVariableSet();
            if(outputVariables.Count > 0)
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

        public IResult<CallbackResponse> InvokeOperationAsync(string pathToOperation, InvocationRequest invocationRequest)
        {
            return OperationHandler.InvokeOperationAsync(pathToOperation, _submodel, ServiceDescriptor, invocationRequest);
        }
      
        public IResult<InvocationResponse> GetInvocationResult(string pathToOperation, string requestId)
        {
            return OperationHandler.GetInvocationResult(pathToOperation, requestId);
        }

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

        public IResult PublishEvent(IEventMessage eventMessage) => PublishEventAsync(eventMessage).Result;        
       
        public virtual void ConfigureEventHandler(IMessageClient messageClient)
        {
            this.messageClient = messageClient;
        }
       
        public virtual void SubscribeUpdates(string pathToSubmodelElement, Action<IValue> updateFunction)
        {
            if (!updateFunctions.ContainsKey(pathToSubmodelElement))
                updateFunctions.Add(pathToSubmodelElement, updateFunction);
            else
                updateFunctions[pathToSubmodelElement] = updateFunction;
        }
       
        public virtual void PublishUpdate(string pathToSubmodelElement, IValue value)
        {
            if (updateFunctions.TryGetValue(pathToSubmodelElement, out Action<IValue> updateFunction))
                updateFunction.Invoke(value);

        }

        public IResult<ISubmodel> RetrieveSubmodel()
        {
            return new Result<ISubmodel>(_submodel != null, _submodel);
        }

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string pathToSubmodelElement, ISubmodelElement submodelElement)
            => CreateOrUpdateSubmodelElement(pathToSubmodelElement, submodelElement, new SubmodelElementHandler(submodelElement.Get, submodelElement.Set));

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string pathToSubmodelElement, ISubmodelElement submodelElement, SubmodelElementHandler submodelElementHandler)
        {
            if (_submodel == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage("Submodel"));

            var created = _submodel.SubmodelElements.CreateOrUpdate(pathToSubmodelElement, submodelElement);
            if (created.Success && created.Entity != null)
                RegisterSubmodelElementHandler(pathToSubmodelElement, submodelElementHandler);
            return created;
        }

        public IResult<IElementContainer<ISubmodelElement>> RetrieveSubmodelElements()
        {
            if (_submodel == null)
                return new Result<ElementContainer<ISubmodelElement>>(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result<ElementContainer<ISubmodelElement>>(false, new NotFoundMessage("SubmodelElements"));
            return _submodel.SubmodelElements.RetrieveAll();
        }

        public IResult<ISubmodelElement> RetrieveSubmodelElement(string submodelElementId)
        {
            if (_submodel == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result<ISubmodelElement>(false, new NotFoundMessage(submodelElementId));

            return _submodel.SubmodelElements.Retrieve(submodelElementId);
        }
        public IResult<IValue> RetrieveSubmodelElementValue(string submodelElementId)
        {
            if (_submodel == null)
                return new Result<IValue>(false, new NotFoundMessage("Submodel"));

            var submodelElement = _submodel.SubmodelElements.Retrieve<ISubmodelElement>(submodelElementId);
            if (!submodelElement.Success || submodelElement.Entity == null)
                return new Result<IValue>(false, new NotFoundMessage($"SubmodelElement {submodelElementId}"));

            if (submodelElementHandler.TryGetValue(submodelElementId, out SubmodelElementHandler elementHandler) && elementHandler.GetValueHandler != null)
                return new Result<IValue>(true, elementHandler.GetValueHandler.Invoke(submodelElement.Entity));
            else if (submodelElement.Entity.Get != null)
                return new Result<IValue>(true, submodelElement.Entity.Get.Invoke(submodelElement.Entity));
            else
                return new Result<IValue>(false, new NotFoundMessage($"SubmodelElementHandler for {submodelElementId}"));
        }

        public IResult UpdateSubmodelElementValue(string submodelElementId, IValue value)
        {
            if (_submodel == null)
                return new Result(false, new NotFoundMessage("Submodel"));

            var submodelElement = _submodel.SubmodelElements.Retrieve<ISubmodelElement>(submodelElementId);
            if (!submodelElement.Success || submodelElement.Entity == null)
                return new Result(false, new NotFoundMessage($"SubmodelElement {submodelElementId}"));

            if (submodelElementHandler.TryGetValue(submodelElementId, out SubmodelElementHandler elementHandler) && elementHandler.SetValueHandler != null)
            {
                elementHandler.SetValueHandler.Invoke(submodelElement.Entity, value);
                return new Result(true);
            }
            else if (submodelElement.Entity.Set != null)
            {
                submodelElement.Entity.Set.Invoke(submodelElement.Entity, value);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage($"SubmodelElementHandler for {submodelElementId}"));
        }

        public IResult DeleteSubmodelElement(string submodelElementId)
        {
            if (_submodel == null)
                return new Result(false, new NotFoundMessage("Submodel"));

            if (_submodel.SubmodelElements == null)
                return new Result(false, new NotFoundMessage(submodelElementId));

            var deleted = _submodel.SubmodelElements.Delete(submodelElementId);
            if (deleted.Success)
                UnregisterSubmodelElementHandler(submodelElementId);
            return deleted;
        }        
    }
}
