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

using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.Common;
using System;

namespace BaSyx.API.Clients.Persistency;

/// <summary>
/// Represents a persistent collection for elements with a combined key of type <typeparamref name="TIdentifier"/>.
/// </summary>
/// <typeparam name="TIdentifier">The type of each element of the Composite key.</typeparam>
/// <typeparam name="TElement">The type of elements in the collection.</typeparam>
public interface IPersistentCompositeKeyCollection<TIdentifier, TElement> : IPersistentCollection<CompositeKey<TIdentifier>, TElement> where TElement : IReferable, IModelElement
{
}
            
