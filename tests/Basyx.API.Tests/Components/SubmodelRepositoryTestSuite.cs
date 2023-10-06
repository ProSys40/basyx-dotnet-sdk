/*******************************************************************************
* Copyright (c) 2023 the Eclipse BaSyx Authors
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/

using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Components
{
    public abstract class SubmodelRepositoryTestSuite
    {
        protected abstract ISubmodelRepository GetSubmodelRepository();

        public IResult<ISubmodel> CreateSubmodel(ISubmodel submodel)
        {
            throw new NotImplementedException();
        }

        public IResult DeleteSubmodel(string submodelId)
        {
            throw new NotImplementedException();
        }

        public IResult<ISubmodel> RetrieveSubmodel(string submodelId)
        {
            throw new NotImplementedException();
        }

        public IResult<IElementContainer<ISubmodel>> RetrieveSubmodels()
        {
            throw new NotImplementedException();
        }

        public IResult UpdateSubmodel(string submodelId, ISubmodel submodel)
        {
            throw new NotImplementedException();
        }
    }
}
