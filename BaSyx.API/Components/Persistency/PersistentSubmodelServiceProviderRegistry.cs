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

using BaSyx.API.Clients.Persistency;
using BaSyx.API.Components.ServiceProvider.Persistency;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Components.Persistency
{
    internal class PersistentSubmodelServiceProviderRegistry : ISubmodelServiceProviderRegistry
    {
        public IPersistentIdentifiables<ISubmodelDescriptor> PersistentSubmodelDescriptors { get; set; }
        public IPersistentIdentifiables<ISubmodel> PersistentSubmodels { get; set; }

        public IResult<ISubmodelServiceProvider> GetSubmodelServiceProvider(string id)
        {
            IResult<ISubmodel> submodelResult = PersistentSubmodels.Retrieve(id);
            if (!submodelResult.Success || submodelResult.Entity == null)
            {
                return new Result<ISubmodelServiceProvider>(false, new NotFoundMessage(id));
            }

            ISubmodel submodel = submodelResult.Entity;

            ISubmodelServiceProvider servideProvider = GetBoundServiceProvider(submodel);
            return new Result<ISubmodelServiceProvider>(true, servideProvider);
        }

        public IResult<IEnumerable<ISubmodelServiceProvider>> GetSubmodelServiceProviders()
        {
            IResult<IQueryableElementContainer<ISubmodel>> submodelsResult = PersistentSubmodels.RetrieveAll<ISubmodel>();
            if (!submodelsResult.Success || submodelsResult.Entity == null)
            {
                return new Result<IEnumerable<ISubmodelServiceProvider>>(false, new NotFoundMessage());
            }

            IQueryableElementContainer<ISubmodel> submodels = submodelsResult.Entity;
            List<ISubmodelServiceProvider> serviceProviders = submodels.Select(GetBoundServiceProvider).ToList();
            return new Result<IEnumerable<ISubmodelServiceProvider>>(true, serviceProviders);
        }

        private ISubmodelServiceProvider GetBoundServiceProvider(ISubmodel submodel)
        {
            ISubmodelServiceProvider serviceProvider;
            IResult<ISubmodelDescriptor> descriptorResult = PersistentSubmodelDescriptors.Retrieve(submodel.Identification.Id);
            if (!descriptorResult.Success || descriptorResult == null)
            {
                serviceProvider = new PersistentSubmodelServiceProvider();
            }
            else
            {
                serviceProvider = new PersistentSubmodelServiceProvider(descriptorResult.Entity);
            }
            serviceProvider.BindTo(submodel);
            return serviceProvider;
        }

        public IResult<ISubmodelDescriptor> RegisterSubmodelServiceProvider(string id, ISubmodelServiceProvider submodelServiceProvider)
        {
            IResult<ISubmodel> shellResult = PersistentSubmodels.Retrieve(id);
            if (!shellResult.Success || shellResult.Entity == null)
            {
                return new Result<ISubmodelDescriptor>(false, new NotFoundMessage(id));
            }
            _ = PersistentSubmodelDescriptors.CreateOrUpdate(id, submodelServiceProvider.ServiceDescriptor);
            return new Result<ISubmodelDescriptor>(true, submodelServiceProvider.ServiceDescriptor);
        }

        public IResult UnregisterSubmodelServiceProvider(string id)
        {
            IResult<ISubmodel> shellResult = PersistentSubmodels.Retrieve(id);
            return new Result(shellResult.Success);
        }
    }
}
