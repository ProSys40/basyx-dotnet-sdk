/*******************************************************************************
* Copyright (c) 2022 LTSoft - Agrentur für Leittechnik-Software GmbH
* Author: Björn Höper (hoeper@ltsoft.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Moq;

namespace Basyx.API.Tests.Components.ServiceProvider;

public class SubmodelRepositoryServiceProviderTests
{
    [Fact]
    public async Task CreateSubmodel_WhenCorrect_CallsFactory()
    {
        var submodelMock = new Mock<ISubmodel>();
        submodelMock.Setup(s => s.Identification)
            .Returns(new Identifier("http://assetadminshell.io/1/0/0/testmodel", KeyType.URI));

        var serviceProviderMock = new Mock<ISubmodelServiceProvider>();
        serviceProviderMock.Setup(p => p.GetBinding()).Returns(submodelMock.Object);

        var factoryMock = new Mock<ISubmodelServiceProviderFactory>();
        factoryMock.Setup(f => f.CreateSubmodelServiceProvider(submodelMock.Object)).Returns(serviceProviderMock.Object);
        
        var submodelRepositoryServiceProvider = new SubmodelRepositoryServiceProvider(factoryMock.Object);
        var result = submodelRepositoryServiceProvider.CreateSubmodel(submodelMock.Object);

        factoryMock.Verify(f => f.CreateSubmodelServiceProvider(submodelMock.Object), Times.AtLeastOnce);

        Assert.Equal(submodelMock.Object, result.Entity);
    }
}