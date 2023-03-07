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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations.ArangoDB;

public class ArangoSubmodelFactory
{
    public static ISubmodel Create(ISubmodel submodel)
    {
        return new SubmodelWithArangoKey(submodel);
    }
}
