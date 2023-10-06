/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
* Authors: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.Clients.Persistency;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using Basyx.API.Tests.Components.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Basyx.API.Tests.Components.Persistency;

public class PersistentSubmodelRepositoryServiceProviderTests : SubmodelRepositoryServiceProviderTestSuite
{
    protected override ISubmodelRepositoryServiceProvider GetSubmodelRepositoryServiceProvider()
    {
        throw new NotImplementedException();
    }
}
