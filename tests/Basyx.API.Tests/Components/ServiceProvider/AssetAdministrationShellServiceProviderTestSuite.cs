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
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Components.ServiceProvider
{
    public abstract class AssetAdministrationShellServiceProviderTestSuite
    {
        protected abstract IAssetAdministrationShellServiceProvider GetAssetAdministrationShellServiceProvider();

        public ISubmodelServiceProviderRegistry SubmodelRegistry => throw new NotImplementedException();

        public IAssetAdministrationShellDescriptor ServiceDescriptor => throw new NotImplementedException();

        public void BindTo(IAssetAdministrationShell element)
        {
            throw new NotImplementedException();
        }

        public IAssetAdministrationShell GetBinding()
        {
            throw new NotImplementedException();
        }
    }
}
