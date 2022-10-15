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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.API.Components;

/// <summary>
/// Provides the internal submodel service provider as default implementation
/// </summary>
public class InternalSubmodelServiceProviderFactory : ISubmodelServiceProviderFactory
{
    public ISubmodelServiceProvider CreateSubmodelServiceProvider(ISubmodel submodel)
    {
        InternalSubmodelServiceProvider sp = new InternalSubmodelServiceProvider(submodel);

        return sp;
    }
}