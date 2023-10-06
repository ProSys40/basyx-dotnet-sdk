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
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Components
{
    public abstract class AssetAdministrationShellServiceProviderRegistryTestSuite
    {
        protected abstract IAssetAdministrationShellServiceProviderRegistry GetAssetAdministrationShellServiceProviderRegistry();

        public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
        {
            throw new NotImplementedException();
        }

        public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
        {
            throw new NotImplementedException();
        }

        public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {
            throw new NotImplementedException();
        }

        public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
        {
            throw new NotImplementedException();
        }
    }
}
