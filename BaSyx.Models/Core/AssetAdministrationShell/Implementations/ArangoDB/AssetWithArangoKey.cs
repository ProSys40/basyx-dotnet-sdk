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
using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations.ArangoDB;

public class AssetWithArangoKey : IAsset
{
    private readonly IAsset _asset;

    public string _key
    {
        get { return _asset.Identification.Id; }
        private set { }
    }

    public AssetWithArangoKey(IAsset wrappedAsset)
    {
        _asset = wrappedAsset;
    }

    public IReference<ISubmodel> AssetIdentificationModel => _asset.AssetIdentificationModel;

    public IReference<ISubmodel> BillOfMaterial => _asset.BillOfMaterial;

    public AssetKind Kind => _asset.Kind;

    public Identifier Identification => _asset.Identification;

    public AdministrativeInformation Administration => _asset.Administration;

    public string IdShort => _asset.IdShort;

    public string Category => _asset.Category;

    public LangStringSet Description => _asset.Description;

    public IReferable Parent { get => _asset.Parent; set => _asset.Parent = value; }

    public ModelType ModelType => _asset.ModelType;

    public IConceptDescription ConceptDescription => _asset.ConceptDescription;

    public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications => _asset.EmbeddedDataSpecifications;
}
