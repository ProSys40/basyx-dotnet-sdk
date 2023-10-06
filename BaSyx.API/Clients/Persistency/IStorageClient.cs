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

using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System.Xml.Linq;
using System;

namespace BaSyx.API.Clients.Persistency
{
    public interface IStorageClient : IDisposable
    {
        Dictionary<Type, IPersistentCollectionType> StorageClients { get; }

        /// <summary>
        /// Creates a Collection of Type type and adds it to the StorageClients dictionary 
        /// </summary>
        /// <returns></returns>
        IResult CreateCollection(IPersistentCollectionType persistentCollection);

        /// <summary>
        /// If Exists deletes the collection of Type type from the StorageClients dictionary 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IResult DeleteCollection(Type type);

        /// <summary>
        /// Returns the name of the current storage.
        /// A storage may be e.g. a database or a object storage.
        /// In the context of a relational database a storage name would refere to a database name.
        /// </summary>
        /// <returns>If successfull an IResult with the name of the current storage</returns>
        IResult<string> GetStorageName();


        /// <summary>
        /// Delets the collection with the given collectionName 
        /// A collection is a container for data that may be called different with different storages.
        /// E.g. generic relational database: Table, Amazon S3: Bucket, MongoDB: Collection
        /// </summary>
        /// <returns></returns>
        IResult DeleteCollection();

        /// <summary>
        /// Provides a storage connection that shall be automatically closed when used with the using syntax.
        /// </summary>
        /// <returns></returns>
        IResult<T> GetConnection<T>();
    }
}