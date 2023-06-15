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
using BaSyx.Utils.Client;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Utils.ResultHandling;
using System;
using BaSyx.API.AssetAdministrationShell;
using BaSyx.API.Clients;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Communication;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BaSyx.API.Components;

/// <summary>
/// Provides basic functions for a Submodel persisted in ArangoDB
/// </summary>
public sealed class ArangoSubmodelServiceProvider : SubmodelServiceProvider
{
    public ISubmodel Submodel { get; }

    public ArangoSubmodelServiceProvider(ISubmodel submodel)
    {
        Submodel = submodel;
    }
}
