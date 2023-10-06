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
using BaSyx.Utils.ResultHandling;

namespace BaSyx.API.Clients.Persistency;


internal class PersistentAssetAdministrationShellClient : IAssetAdministrationShellClient
{
    IStorageClient StorageClient { get; set; }
    IPersistentIdentifiables<ISubmodel> Submodels { get; set; }

    public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell()
    {
        throw new System.NotImplementedException();
    }
}
