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

using BaSyx.API.Components;
using BaSyx.Models.Communication;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.Logging;
using Moq;

namespace BaSyx.ServiceProvider.EventDriven.Tests
{
    public class EventDrivenSubmodelServiceProviderTests
    {
        [Fact]
        public async Task BindTo_WhenCalled_CallsPersister()
        {
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                persistingMock.Object);

            var submodelMock = new Mock<ISubmodel>();
            eventDrivenProvider.BindTo(submodelMock.Object);

            persistingMock.Verify(m => m.BindTo(submodelMock.Object), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetBinding_WhenCalled_CallsPersister()
        {
            var submodelMock = new Mock<ISubmodel>();
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            persistingMock.Setup(m => m.GetBinding()).Returns(submodelMock.Object);
            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();
            var descriptorMock = new Mock<ISubmodelDescriptor>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                persistingMock.Object);

            var submodel = eventDrivenProvider.GetBinding();
            Assert.Equal(submodelMock.Object, submodel);

            persistingMock.Verify(m => m.GetBinding(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task RetrieveSubmodel_WhenCalled_CallsPersister()
        {
            var submodelMock = new Mock<ISubmodel>();
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            persistingMock.Setup(m => m.RetrieveSubmodel()).Returns(new Result<ISubmodel>(true, submodelMock.Object));
            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();
            var descriptorMock = new Mock<ISubmodelDescriptor>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                persistingMock.Object);

            var submodel = eventDrivenProvider.RetrieveSubmodel();
            Assert.Equal(submodelMock.Object, submodel.Entity);

            persistingMock.Verify(m => m.RetrieveSubmodel(), Times.AtLeastOnce);
        }
        
        [Fact]
        public async Task PublishEvent_WhenCalled_CallsPersisterAndPublishesToSubject()
        {
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            var evtMsg = new EventMessage("mySourceElement", "Something changed");
            persistingMock.Setup(m => m.PublishEvent(evtMsg)).Returns(new Result(true));
            persistingMock.Setup(m => m.GetBinding()).Returns(new Submodel("mySubmodel",
                new Identifier("https://test.org/mySubmodel", KeyType.IRI)));
            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                 persistingMock.Object);
            SubmodelEventData? capturedEvent = null; 
            eventDrivenProvider.SubmodelEventObservable.Subscribe(e => capturedEvent = e);

            var result = eventDrivenProvider.PublishEvent(evtMsg);

            var capturedEvtMesage = capturedEvent as SubmodelEventInvokedEventData;
            Assert.NotNull(capturedEvtMesage);
            Assert.Equal(evtMsg, capturedEvtMesage.EventMessage);
            persistingMock.Verify(m => m.PublishEvent(evtMsg), Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeOperation_WhenCalled_CallsPersisterAndGeneratesEvent()
        {
            var submodelMock = new Mock<ISubmodel>();
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            persistingMock.Setup(m => m.GetBinding()).Returns(new Submodel("mySubmodel",
                new Identifier("https://test.org/mySubmodel", KeyType.IRI)));

            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();
            var descriptorMock = new Mock<ISubmodelDescriptor>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                persistingMock.Object);

            SubmodelEventData capturedEvent = null;
            eventDrivenProvider.SubmodelEventObservable.Subscribe(e => capturedEvent = e);

            var opId = "myOperation";
            var requestId = Guid.NewGuid().ToString("N");
            var invocationRequest = new InvocationRequest(requestId);
            var invocationResponse = new InvocationResponse(requestId);
            persistingMock.Setup(m => m.InvokeOperation(opId, invocationRequest))
                .Returns(new Result<InvocationResponse>(true, invocationResponse));
            var invocationResult = eventDrivenProvider.InvokeOperation(opId, invocationRequest);

            Assert.True(invocationResult.Success);
            Assert.NotNull(capturedEvent);
            Assert.IsType<SubmodelInvokedOperationEventData>(capturedEvent);
            var invokedOperationEvent = capturedEvent as SubmodelInvokedOperationEventData;
            Assert.Equal(invocationRequest, invokedOperationEvent.Request);
            Assert.Equal(invocationResponse, invokedOperationEvent.Response);
            persistingMock.Verify(m => m.InvokeOperation(opId, invocationRequest), Times.Once);
        }
        
        [Fact]
        public async Task InvokeOperationAsync_WhenCalled_CallsPersisterAndGeneratesEvent()
        {
            var submodelMock = new Mock<ISubmodel>();
            var persistingMock = new Mock<ISubmodelServiceProvider>();
            persistingMock.Setup(m => m.GetBinding()).Returns(new Submodel("mySubmodel",
                new Identifier("https://test.org/mySubmodel", KeyType.IRI)));

            var loggerMock = new Mock<ILogger<EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>>>();
            var descriptorMock = new Mock<ISubmodelDescriptor>();

            var eventDrivenProvider = new EventDrivenSubmodelServiceProvider<ISubmodelServiceProvider>(
                loggerMock.Object,
                persistingMock.Object);

            SubmodelEventData capturedEvent = null;
            eventDrivenProvider.SubmodelEventObservable.Subscribe(e => capturedEvent = e);

            var opId = "myOperation";
            var requestId = Guid.NewGuid().ToString("N");
            var invocationRequest = new InvocationRequest(requestId);
            var invocationResponse = new CallbackResponse(requestId);
            persistingMock.Setup(m => m.InvokeOperationAsync(opId, invocationRequest))
                .Returns(new Result<CallbackResponse>(true, invocationResponse));
            var invocationResult = eventDrivenProvider.InvokeOperationAsync(opId, invocationRequest);

            Assert.True(invocationResult.Success);
            Assert.NotNull(capturedEvent);
            Assert.IsType<SubmodelInvokedOperationAsyncEventData>(capturedEvent);
            var invokedOperationEvent = capturedEvent as SubmodelInvokedOperationAsyncEventData;
            Assert.Equal(invocationRequest, invokedOperationEvent.Request);
            Assert.Equal(invocationResponse, invokedOperationEvent.Callback);
            persistingMock.Verify(m => m.InvokeOperationAsync(opId, invocationRequest), Times.Once);
        }
    }
}