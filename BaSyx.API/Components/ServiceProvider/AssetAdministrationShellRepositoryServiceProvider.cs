/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.API.Components.ServiceProvider;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.API.Components
{
    public class AssetAdministrationShellRepositoryServiceProvider : AbstractAssetAdministrationShellRepositoryServiceProvider
    {
        private readonly IAssetAdministrationShellServiceProviderFactory _assetAdministrationShellServiceProviderFactory;

        private Dictionary<string, IAssetAdministrationShellServiceProvider> AssetAdministrationShellServiceProviders { get; }
        public IEnumerable<IAssetAdministrationShell> AssetAdministrationShells => GetBinding();


        private IAssetAdministrationShellRepositoryDescriptor _serviceDescriptor;
        public override IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor
        {
            get
            {
                if (_serviceDescriptor == null)
                    _serviceDescriptor = new AssetAdministrationShellRepositoryDescriptor(AssetAdministrationShells, null);

                return _serviceDescriptor;
            }
            protected set
            {
                _serviceDescriptor = value;
            }
        }
        public AssetAdministrationShellRepositoryServiceProvider()
        {
            AssetAdministrationShellServiceProviders = new Dictionary<string, IAssetAdministrationShellServiceProvider>();
        }
        public AssetAdministrationShellRepositoryServiceProvider(IAssetAdministrationShellRepositoryDescriptor descriptor, 
            IAssetAdministrationShellServiceProviderFactory assetAdministrationShellServiceProviderFactory) : this(assetAdministrationShellServiceProviderFactory)
        {
            ServiceDescriptor = descriptor;
        }

        public AssetAdministrationShellRepositoryServiceProvider(IAssetAdministrationShellServiceProviderFactory assetAdministrationShellServiceProviderFactory)
        {
            _assetAdministrationShellServiceProviderFactory = assetAdministrationShellServiceProviderFactory;
            this.AssetAdministrationShellServiceProviders = new Dictionary<string, IAssetAdministrationShellServiceProvider>();
        }

        public override IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
        {
            if (aas == null)
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aas)));

            var assetAdministrationShellServiceProvider = _assetAdministrationShellServiceProviderFactory.CreateServiceProvider(aas, true);
            RegisterAssetAdministrationShellServiceProvider(aas.Identification.Id, assetAdministrationShellServiceProvider);

            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(aas.Identification.Id);
            if (retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
                return new Result<IAssetAdministrationShell>(true, serviceProvider.GetBinding());
            
            return new Result<IAssetAdministrationShell>(false, new Message(MessageType.Error, "Could not retrieve Asset Administration Shell Service Provider"));
        }

        public override IResult DeleteAssetAdministrationShell(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aasId)));
            
            return UnregisterAssetAdministrationShellServiceProvider(aasId);
        }

        public override IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
        {
            if (AssetAdministrationShellServiceProviders.TryGetValue(id, out IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider))
                return new Result<IAssetAdministrationShellServiceProvider>(true, assetAdministrationShellServiceProvider);
            else
                return new Result<IAssetAdministrationShellServiceProvider>(false, new NotFoundMessage(id));
        }

        public override IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
        {
            if (AssetAdministrationShellServiceProviders.Values == null)
                return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(false, new NotFoundMessage("Asset AdministrationShell Service Providers"));

            return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(true, AssetAdministrationShellServiceProviders.Values?.ToList());
        }

        public override IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
                AssetAdministrationShellServiceProviders[id] = assetAdministrationShellServiceProvider;
            else
                AssetAdministrationShellServiceProviders.Add(id, assetAdministrationShellServiceProvider);

            return new Result<IAssetAdministrationShellDescriptor>(true, assetAdministrationShellServiceProvider.ServiceDescriptor);
        }

        public override IResult UnregisterAssetAdministrationShellServiceProvider(string id)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
            {
                AssetAdministrationShellServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }

        public override IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId)
        {
            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(aasId);
            if(retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
            {
                IAssetAdministrationShell binding = serviceProvider.GetBinding();
                return new Result<IAssetAdministrationShell>(true, binding);
            }
            return new Result<IAssetAdministrationShell>(false, new NotFoundMessage("Asset Administration Shell Service Provider"));
        }

        public override IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
        {
            return new Result<IElementContainer<IAssetAdministrationShell>>(true, new ElementContainer<IAssetAdministrationShell>(null, AssetAdministrationShells));
        }

        public override IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aasId)));
            if (aas == null)
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aas)));
            return CreateAssetAdministrationShell(aas);
        }
    }
}
