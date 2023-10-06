using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Clients.Persistency;

internal interface IPersistentSubmodels : IPersistentCollection<string, ISubmodel>
{
}
