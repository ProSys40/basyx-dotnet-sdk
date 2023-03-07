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
using BaSyx.Models.Core.AssetAdministrationShell.Constraints;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace BaSyx.Models.Core.AssetAdministrationShell.Implementations.ArangoDB;

// TODO: geht das nicht eleganter??
/// <summary>
/// this class wraps a Submodel and adds a _key Property which is used by ArangoDb as _key
/// </summary>
public class SubmodelWithArangoKey : ISubmodel
{
    private readonly ISubmodel _submodel;

    public string _key { 
        get { return _submodel.Identification.Id; }
        private set { }
    }

    public SubmodelWithArangoKey(ISubmodel wrappedSubmodel)
    {
        _submodel = wrappedSubmodel;
    }

    public IElementContainer<ISubmodelElement> SubmodelElements => _submodel.SubmodelElements;

    public Identifier Identification => _submodel.Identification;

    public AdministrativeInformation Administration => _submodel.Administration;

    public string IdShort => _submodel.IdShort;

    public string Category => _submodel.Category;

    public LangStringSet Description => _submodel.Description;

    IReferable IReferable.Parent { get => _submodel.Parent; set => _submodel.Parent = value; }

    public ModelingKind Kind => _submodel.Kind;

    public IReference SemanticId => _submodel.SemanticId;

    public ModelType ModelType => _submodel.ModelType;

    public IConceptDescription ConceptDescription => _submodel.ConceptDescription;

    public IEnumerable<IEmbeddedDataSpecification> EmbeddedDataSpecifications => _submodel.EmbeddedDataSpecifications;

    public IEnumerable<IConstraint> Constraints => _submodel.Constraints;

}
