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
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;


namespace Basyx.API.Tests.Components.ServiceProvider
{
    public abstract class AssetAdministrationShellRepositoryServiceProviderTestSuite
    {
        protected abstract IAssetAdministrationShellRepositoryServiceProvider GetAssetAdministrationShellRepositoryServiceProvider();

        public IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor => throw new NotImplementedException();

        public void BindTo(IEnumerable<IAssetAdministrationShell> element)
        {
            throw new NotImplementedException();
        }

        public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            throw new NotImplementedException();
        }

        public IResult DeleteAssetAdministrationShell(string aasId)
        {
            throw new NotImplementedException();
        }

        public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
        {
            throw new NotImplementedException();
        }

        public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAssetAdministrationShell> GetBinding()
        {
            throw new NotImplementedException();
        }

        public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {
            throw new NotImplementedException();
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId)
        {
            throw new NotImplementedException();
        }

        public IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
        {
            throw new NotImplementedException();
        }

        public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas)
        {
            throw new NotImplementedException();
        }
    }
}
