/*******************************************************************************
* Copyright (c) 2022 LTSoft - Agrentur für Leittechnik-Software GmbH,
*               2023 Fraunhofer IESE
* Authors: Björn Höper (hoeper@ltsoft.de),
*          Jannis Jung (jannis.jung@iese.fraunhofer.de)
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

public class SubmodelRepositoryServiceProviderTests : SubmodelRepositoryServiceProviderTestSuite
{
    protected override ISubmodelRepositoryServiceProvider GetSubmodelRepositoryServiceProvider(ISubmodelServiceProviderFactory submodelServiceProviderFactory)
    {
        return new SubmodelRepositoryServiceProvider(submodelServiceProviderFactory);
    }
}