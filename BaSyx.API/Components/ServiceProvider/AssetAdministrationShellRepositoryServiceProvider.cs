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
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaSyx.API.Components
{
    public class AssetAdministrationShellRepositoryServiceProvider : IAssetAdministrationShellRepositoryServiceProvider
    {
        private readonly IAssetAdministrationShellServiceProviderFactory _assetAdministrationShellServiceProviderFactory;
        public IEnumerable<IAssetAdministrationShell> AssetAdministrationShells => GetBinding();

        private Dictionary<string, IAssetAdministrationShellServiceProvider> AssetAdministrationShellServiceProviders { get; }

        private IAssetAdministrationShellRepositoryDescriptor _serviceDescriptor;
        public IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor
        {
            get
            {
                if (_serviceDescriptor == null)
                    _serviceDescriptor = new AssetAdministrationShellRepositoryDescriptor(AssetAdministrationShells, null);

                return _serviceDescriptor;
            }
            private set
            {
                _serviceDescriptor = value;
            }
        }
        public AssetAdministrationShellRepositoryServiceProvider(IAssetAdministrationShellRepositoryDescriptor descriptor, 
            IAssetAdministrationShellServiceProviderFactory assetAdministrationShellServiceProviderFactory) : this(assetAdministrationShellServiceProviderFactory)
        {
            ServiceDescriptor = descriptor;
        }

        public AssetAdministrationShellRepositoryServiceProvider(IAssetAdministrationShellServiceProviderFactory assetAdministrationShellServiceProviderFactory)
        {
            _assetAdministrationShellServiceProviderFactory = assetAdministrationShellServiceProviderFactory;
            AssetAdministrationShellServiceProviders = new Dictionary<string, IAssetAdministrationShellServiceProvider>();
        }

        public void BindTo(IEnumerable<IAssetAdministrationShell> boundElement)
        {
            foreach (var assetAdministrationShell in boundElement)
            {
                RegisterAssetAdministrationShellServiceProvider(assetAdministrationShell.Identification.Id, assetAdministrationShell.CreateServiceProvider(true));
            }
            ServiceDescriptor = ServiceDescriptor ?? new AssetAdministrationShellRepositoryDescriptor(boundElement, null);
        }
        public IEnumerable<IAssetAdministrationShell> GetBinding()
        {
            List<IAssetAdministrationShell> assetAdministrationShells = new List<IAssetAdministrationShell>();
            var retrievedShellServiceProviders = GetAssetAdministrationShellServiceProviders();
            if (retrievedShellServiceProviders.TryGetEntity(out IEnumerable<IAssetAdministrationShellServiceProvider> serviceProviders))
            {
                foreach (var serviceProvider in serviceProviders)
                {
                    IAssetAdministrationShell binding = serviceProvider.GetBinding();
                    assetAdministrationShells.Add(binding);
                }
            }
            return assetAdministrationShells;
        }

        public IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas)
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

        public IResult DeleteAssetAdministrationShell(string aasId)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aasId)));
            
            return UnregisterAssetAdministrationShellServiceProvider(aasId);
        }

        public IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id)
        {
            if (AssetAdministrationShellServiceProviders.TryGetValue(id, out IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider))
                return new Result<IAssetAdministrationShellServiceProvider>(true, assetAdministrationShellServiceProvider);
            else
                return new Result<IAssetAdministrationShellServiceProvider>(false, new NotFoundMessage(id));
        }

        public IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders()
        {
            if (AssetAdministrationShellServiceProviders.Values == null)
                return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(false, new NotFoundMessage("Asset AdministrationShell Service Providers"));

            return new Result<IEnumerable<IAssetAdministrationShellServiceProvider>>(true, AssetAdministrationShellServiceProviders.Values?.ToList());
        }

        public IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
                AssetAdministrationShellServiceProviders[id] = assetAdministrationShellServiceProvider;
            else
                AssetAdministrationShellServiceProviders.Add(id, assetAdministrationShellServiceProvider);

            return new Result<IAssetAdministrationShellDescriptor>(true, assetAdministrationShellServiceProvider.ServiceDescriptor);
        }

        public IResult UnregisterAssetAdministrationShellServiceProvider(string id)
        {
            if (AssetAdministrationShellServiceProviders.ContainsKey(id))
            {
                AssetAdministrationShellServiceProviders.Remove(id);
                return new Result(true);
            }
            else
                return new Result(false, new NotFoundMessage(id));
        }

        public IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId)
        {
            var retrievedShellServiceProvider = GetAssetAdministrationShellServiceProvider(aasId);
            if(retrievedShellServiceProvider.TryGetEntity(out IAssetAdministrationShellServiceProvider serviceProvider))
            {
                IAssetAdministrationShell binding = serviceProvider.GetBinding();
                return new Result<IAssetAdministrationShell>(true, binding);
            }
            return new Result<IAssetAdministrationShell>(false, new NotFoundMessage("Asset Administration Shell Service Provider"));
        }

        public IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells()
        {
            return new Result<IElementContainer<IAssetAdministrationShell>>(true, new ElementContainer<IAssetAdministrationShell>(null, AssetAdministrationShells));
        }

        public IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas)
        {
            if (string.IsNullOrEmpty(aasId))
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aasId)));
            if (aas == null)
                return new Result<IAssetAdministrationShell>(new ArgumentNullException(nameof(aas)));
            return CreateAssetAdministrationShell(aas);
        }
    }
}
