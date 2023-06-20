/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.Clients;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.Components;

public class PersistentAssetAdministrationShellServiceProvider : AssetAdministrationShellServiceProvider
{
    public IStorageClient<IAssetAdministrationShell> storageClient { get; set; }
    public IAssetAdministrationShell Aas { get; }
    
    public PersistentAssetAdministrationShellServiceProvider(IAssetAdministrationShell aas)
    {
        Aas = aas;
    }

    public override IAssetAdministrationShell BuildAssetAdministrationShell()
    {
        string identificationId = Aas.Identification.Id;
        IResult<IAssetAdministrationShell> aasResult = storageClient.Retrieve(identificationId);
        if (!aasResult.Success || aasResult.Entity == null)
        {
            throw new KeyNotFoundException("Could not find AssetAdministrationShell with identification id '" + identificationId + "' in remote storage.");
        }
        return aasResult.Entity;
    }
}
