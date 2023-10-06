﻿/*******************************************************************************
* Copyright (c) 2023 the Eclipse BaSyx Authors
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Clients.Persistency;

public interface IPersistentIdentifiables<TIdentifiable> : IPersistentCollection<string, TIdentifiable>  where TIdentifiable : IIdentifiable, IModelElement
{
}
