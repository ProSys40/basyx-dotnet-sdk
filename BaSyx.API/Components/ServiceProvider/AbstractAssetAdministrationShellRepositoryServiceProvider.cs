using BaSyx.API.AssetAdministrationShell.Extensions;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Components.ServiceProvider
{
    public abstract class AbstractAssetAdministrationShellRepositoryServiceProvider : IAssetAdministrationShellRepositoryServiceProvider
    {
        public abstract IAssetAdministrationShellRepositoryDescriptor ServiceDescriptor { get; protected set; }

        public virtual IEnumerable<IAssetAdministrationShell> GetBinding()
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

        public virtual void BindTo(IEnumerable<IAssetAdministrationShell> element)
        {
            foreach (var assetAdministrationShell in element)
            {
                RegisterAssetAdministrationShellServiceProvider(assetAdministrationShell.Identification.Id, assetAdministrationShell.CreateServiceProvider(true));
            }
            ServiceDescriptor = ServiceDescriptor ?? new AssetAdministrationShellRepositoryDescriptor(element, null);
        }
        public abstract IResult<IAssetAdministrationShell> CreateAssetAdministrationShell(IAssetAdministrationShell aas);
        public abstract IResult DeleteAssetAdministrationShell(string aasId);
        public abstract IResult<IAssetAdministrationShellServiceProvider> GetAssetAdministrationShellServiceProvider(string id);
        public abstract IResult<IEnumerable<IAssetAdministrationShellServiceProvider>> GetAssetAdministrationShellServiceProviders();
        public abstract IResult<IAssetAdministrationShellDescriptor> RegisterAssetAdministrationShellServiceProvider(string id, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider);
        public abstract IResult<IAssetAdministrationShell> RetrieveAssetAdministrationShell(string aasId);
        public abstract IResult<IElementContainer<IAssetAdministrationShell>> RetrieveAssetAdministrationShells();
        public abstract IResult UnregisterAssetAdministrationShellServiceProvider(string id);
        public abstract IResult UpdateAssetAdministrationShell(string aasId, IAssetAdministrationShell aas);
    }
}
