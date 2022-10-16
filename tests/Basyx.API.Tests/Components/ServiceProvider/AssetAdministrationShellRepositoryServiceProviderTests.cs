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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using Moq;

namespace Basyx.API.Tests.Components.ServiceProvider;

public class AssetAdministrationShellRepositoryServiceProviderTests
{
    [Fact]
    public async Task CreateAssetAdministrationShell_WhenSuccess_CallsFactory()
    {
        var aasMock = new Mock<IAssetAdministrationShell>();
        aasMock.Setup(a => a.Identification)
            .Returns(new Identifier("http://assetadminshell.io/1/0/0/testshell", KeyType.URI));

        var serviceProviderMock = new Mock<IAssetAdministrationShellServiceProvider>();
        serviceProviderMock.Setup(p => p.GetBinding()).Returns(aasMock.Object);
            
        var factoryMock = new Mock<IAssetAdministrationShellServiceProviderFactory>();
        factoryMock.Setup(f => f.CreateServiceProvider(aasMock.Object, true)).Returns(serviceProviderMock.Object);

        var aasRepoProvider = new AssetAdministrationShellRepositoryServiceProvider(factoryMock.Object);
        var result = aasRepoProvider.CreateAssetAdministrationShell(aasMock.Object);

        factoryMock.Verify(f => f.CreateServiceProvider(aasMock.Object, true), Times.AtLeastOnce);
        Assert.Equal(aasMock.Object, result.Entity);
    }
}