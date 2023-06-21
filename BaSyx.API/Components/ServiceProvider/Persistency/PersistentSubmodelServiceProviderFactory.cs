/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

/// <summary>
/// Provides basic functions for a Submodel persisted in ArangoDB
/// </summary>
public class PersistentSubmodelServiceProviderFactory : ISubmodelServiceProviderFactory
{
    public ISubmodelServiceProvider CreateSubmodelServiceProvider(ISubmodel submodel)
    {
        PersistentSubmodelServiceProvider persistentSubmodelServiceProvider = new(submodel);

        return persistentSubmodelServiceProvider;
    }
}