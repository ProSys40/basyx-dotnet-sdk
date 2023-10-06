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

using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace Basyx.API.Tests.Components;

public class InternalAssetAdministrationShellServiceProviderTests
{
    [Fact]
    public void InternalAssetAdministrationShellServiceProviderTest()
    {
        // FIXME: This test will currently not completely run through because `provider.AssetAdministrationShell` would get stuck in an endless recursion.
        InternalAssetAdministrationShellServiceProviderFactory shellProviderFactory = new InternalAssetAdministrationShellServiceProviderFactory(null);
        AssetAdministrationShellServiceProvider provider = (AssetAdministrationShellServiceProvider)shellProviderFactory.CreateServiceProvider(null, false);
        IAssetAdministrationShell shell = provider.AssetAdministrationShell;
        Assert.IsAssignableFrom<IAssetAdministrationShell>(shell);
    }
}
