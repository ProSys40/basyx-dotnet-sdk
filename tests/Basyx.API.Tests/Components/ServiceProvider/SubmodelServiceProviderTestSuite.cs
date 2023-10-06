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

using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Components;
using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.Client;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Components.ServiceProvider
{
    public abstract class SubmodelServiceProviderTestSuite
    {
        protected abstract ISubmodelServiceProvider getSubmodelServiceProvider();

        public ISubmodelDescriptor ServiceDescriptor => throw new NotImplementedException();

        public void BindTo(ISubmodel element)
        {
            throw new NotImplementedException();
        }

        public void ConfigureEventHandler(IMessageClient messageClient)
        {
            throw new NotImplementedException();
        }

        public IResult<ISubmodelElement> CreateOrUpdateSubmodelElement(string rootSeIdShortPath, ISubmodelElement submodelElement)
        {
            throw new NotImplementedException();
        }

        public IResult DeleteSubmodelElement(string seIdShortPath)
        {
            throw new NotImplementedException();
        }

        public ISubmodel GetBinding()
        {
            throw new NotImplementedException();
        }

        public IResult<InvocationResponse> GetInvocationResult(string operationIdShortPath, string requestId)
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

        public IResult PublishEvent(IEventMessage eventMessage)
        {
            throw new NotImplementedException();
        }

        public void PublishUpdate(string pathToSubmodelElement, IValue value)
        {
            throw new NotImplementedException();
        }

        public void RegisterEventDelegate(string pathToEvent, EventDelegate eventDelegate)
        {
            throw new NotImplementedException();
        }

        public void RegisterMethodCalledHandler(string pathToOperation, MethodCalledHandler methodCalledHandler)
        {
            throw new NotImplementedException();
        }

        public void RegisterSubmodelElementHandler(string pathToElement, SubmodelElementHandler elementHandler)
        {
            throw new NotImplementedException();
        }

        public MethodCalledHandler RetrieveMethodCalledHandler(string pathToOperation)
        {
            throw new NotImplementedException();
        }

        public IResult<ISubmodel> RetrieveSubmodel()
        {
            throw new NotImplementedException();
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
}
