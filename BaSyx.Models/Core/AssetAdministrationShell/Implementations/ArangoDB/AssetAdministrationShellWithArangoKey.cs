/*******************************************************************************
* Copyright (c) 2023 Fraunhofer IESE
* Author: Jannis Jung (jannis.jung@iese.fraunhofer.de)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.AssetAdministrationShell.Views;
using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations.ArangoDB;

public class AssetAdministrationShellWithArangoKey : IAssetAdministrationShell
{
    private readonly IAssetAdministrationShell _assetAdministrationShell;

    public string _key
    {
        get { return _assetAdministrationShell.Identification.Id; }
        private set { }
    }

    public AssetAdministrationShellWithArangoKey(IAssetAdministrationShell wrappedAssetAdministrationShell)
    {
        _assetAdministrationShell = wrappedAssetAdministrationShell;
    }

    public IReference<IAssetAdministrationShell> DerivedFrom => _assetAdministrationShell.DerivedFrom;

    public IAsset Asset => _assetAdministrationShell.Asset;

    public IElementContainer<ISubmodel> Submodels { get => _assetAdministrationShell.Submodels; set => _assetAdministrationShell.Submodels = value; }

    public IElementContainer<IView> Views => _assetAdministrationShell.Views;

    public IElementContainer<IConceptDictionary> ConceptDictionaries => _assetAdministrationShell.ConceptDictionaries;

    public Identifier Identification => _assetAdministrationShell.Identification;

    public AdministrativeInformation Administration => _assetAdministrationShell.Administration;

    public string IdShort => _assetAdministrationShell.IdShort;

    public string Category => _assetAdministrationShell.Category;

    public LangStringSet Description => _assetAdministrationShell.Description;

    public IReferable Parent { get => _assetAdministrationShell.Parent; set => _assetAdministrationShell.Parent = value; }

    public ModelType ModelType => _assetAdministrationShell.ModelType;

    public IConceptDescription ConceptDescription => _assetAdministrationShell.ConceptDescription;

    public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications => _assetAdministrationShell.EmbeddedDataSpecifications;
}
