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

using BaSyx.Utils.ResultHandling;
using System.Collections.Generic;

namespace BaSyx.API.Clients
{
    public interface IStorageClient<T>
    {

        /// <summary>
        /// Returns the name of the current storage
        /// A storage may be e.g. a database or a object storage like Amazon S3
        /// </summary>
        /// <returns>If successfull an IResult with the name of the current storage</returns>
        IResult<string> GetStorageName();

        /// <summary>
        /// Deletes a storage with a given name
        /// </summary>
        /// <returns></returns>
        IResult DeleteStorage();

        /// <summary>
        /// Returns the current collection name
        /// A collection is a container for data that may be called different with different storages.
        /// E.g. generic relational database: Table, Amazon S3: Bucket, MongoDB: Collection
        /// </summary>
        /// <returns>If successfull an IResult with the name of the current colleciton</returns>
        IResult<string> GetCollection();

        /// <summary>
        /// Delets the collection with the given collectionName 
        /// A collection is a container for data that may be called different with different storages.
        /// E.g. generic relational database: Table, Amazon S3: Bucket, MongoDB: Collection
        /// </summary>
        /// <returns></returns>
        IResult DeleteCollection();

        /// <summary>
        /// Creates an entry within the given collection
        /// </summary>
        /// <param name="key">The key of the entry to be updated</param>
        /// <param name="updateEntry">The updated version of the entry</param>
        /// <returns>If successful an IResult with the created or updated entry</returns>
        IResult<T> CreateOrUpdate(string key, T updateEntry);

        /// <summary>
        /// Retruns an specified entry from the given collection
        /// </summary>
        /// <param name="key">The key of the entry to be retrieved</param>
        /// <returns>If successful an IResult with the retrieved entry</returns>
        IResult<T> Retrieve(string key);

        /// <summary>
        /// Returns a List of all entries of a collection
        /// </summary>
        /// <returns>If successful an IResult with a list of entries</returns>
        IResult<List<T>> RetrieveAll();

        /// <summary>
        /// Deletes a specified Entry
        /// </summary>
        /// <param name="key">The key of the entry to be deleted</param>
        /// <returns></returns>
        IResult Delete(string key);
    }
}