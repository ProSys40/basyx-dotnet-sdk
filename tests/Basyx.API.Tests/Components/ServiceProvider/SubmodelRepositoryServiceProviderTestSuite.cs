/*******************************************************************************
* Copyright (c) 2022, 2023 LTSoft - Agrentur für Leittechnik-Software GmbH,
*                          Fraunhofer IESE
* Authors: Björn Höper (hoeper@ltsoft.de),
*          Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using ArangoDBNetStandard.UserApi.Models;
using BaSyx.API.Clients;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Moq;

namespace Basyx.API.Tests.Components.ServiceProvider;
public abstract class SubmodelRepositoryServiceProviderTestSuite
{
    private ISubmodelRepositoryServiceProvider _submodelRepositoryServiceProvider;
    private ISubmodel _submodel;
    private Mock<ISubmodelServiceProviderFactory> _factoryMock;

    protected abstract ISubmodelRepositoryServiceProvider GetSubmodelRepositoryServiceProvider(ISubmodelServiceProviderFactory submodelServiceProviderFactory);

    public SubmodelRepositoryServiceProviderTestSuite()
    {
        Mock<ISubmodel> submodelMock = new Mock<ISubmodel>();
        submodelMock
            .Setup(submodel => submodel.Identification)
            .Returns(new Identifier("http://assetadminshell.io/1/0/0/testmodel", KeyType.URI));
        _submodel = submodelMock.Object;

        var serviceProviderMock = new Mock<ISubmodelServiceProvider>();
        serviceProviderMock
            .Setup(provider => provider.GetBinding())
            .Returns(_submodel);

        _factoryMock = new Mock<ISubmodelServiceProviderFactory>();
        _factoryMock
            .Setup(factory => factory.CreateSubmodelServiceProvider(_submodel))
            .Returns(serviceProviderMock.Object);

        _submodelRepositoryServiceProvider = GetSubmodelRepositoryServiceProvider(_factoryMock.Object);
    }


    [Fact]
    public async Task CreateSubmodel_WhenCorrect_CallsFactory()
    {
        var result = _submodelRepositoryServiceProvider.CreateSubmodel(_submodel);

        _factoryMock.Verify(factory => factory.CreateSubmodelServiceProvider(_submodel), Times.AtLeastOnce);

        Assert.Equal(_submodel, result.Entity);
    }

    [Fact]
    public async Task DeleteSubmodel_WhenCorrect_SubmodelIsGone()
    {
        _ = _submodelRepositoryServiceProvider.CreateSubmodel(_submodel);

        var result = _submodelRepositoryServiceProvider.DeleteSubmodel(_submodel.Identification.Id);
        var control = _submodelRepositoryServiceProvider.RetrieveSubmodel(_submodel.Identification.Id);

        Assert.True(result.Success);
        Assert.False(control.Success);
    }


    [Fact]
    public async Task RetrieveSubmodel()
    {
        _ = _submodelRepositoryServiceProvider.CreateSubmodel(_submodel);

        var result = _submodelRepositoryServiceProvider.RetrieveSubmodel(_submodel.Identification.Id);

        Assert.Equal(_submodel, result.Entity);
    }
}