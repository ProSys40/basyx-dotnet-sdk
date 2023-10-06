/*******************************************************************************
* Copyright (c) 2023 the Eclipse BaSyx Authors
* Author: Jannis Jung(jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.Clients;

internal interface IAssetAdministrationShellRepositoryClient
{
    IResult<IAssetAdministrationShell> CreateOrUpdateShell(IAssetAdministrationShell shell);

    IResult<IElementContainer<IAssetAdministrationShell>> RetrieveShells();

    IResult<IAssetAdministrationShell> RetrieveShell(string shellIdentificationId);

    IResult DeleteShell(string shellIdentificationId);
}
