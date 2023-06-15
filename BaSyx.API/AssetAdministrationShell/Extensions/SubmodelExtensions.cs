/*******************************************************************************
* Copyright (c) 2020, 2021, 2023 Robert Bosch GmbH, Fraunhofer IESE
* Authors: Constantin Ziesche (constantin.ziesche@bosch.com),
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

namespace BaSyx.API.AssetAdministrationShell.Extensions;

public static class SubmodelExtensions
{
    public static ISubmodelServiceProvider CreateServiceProvider(this ISubmodel submodel, Backend backend)
    {
        switch (backend)
        {
            case Backend.INTERNAL:
                return CreateServiceProvider(submodel);
            case Backend.ARANGO:
                return CreateArangoServiceProvider(submodel);
            default:
                return CreateServiceProvider(submodel);
        }
    }

    public static ISubmodelServiceProvider CreateServiceProvider(this ISubmodel submodel)
    {
        InternalSubmodelServiceProvider sp = new InternalSubmodelServiceProvider(submodel);

        return sp;
    }

    public static ISubmodelServiceProvider CreateArangoServiceProvider(this ISubmodel submodel)
    {
        return new ArangoSubmodelServiceProvider(submodel);
    }
}

