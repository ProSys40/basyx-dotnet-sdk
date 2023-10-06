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
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using System;

namespace BaSyx.API.Clients.Persistency
{
    /// <summary>
    /// Represents a persistent collection for elements with keys of type <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <typeparam name="TIdentifier">The type of the keys for the elements in the collection.</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public interface IPersistentCollection<TIdentifier, TElement> : IPersistentCollectionType, ICrudContainer<TIdentifier, TElement> where TElement : IReferable, IModelElement
    {

        /// <summary>
        /// Returns the name of the current storage.
        /// A storage may be e.g. a database or a object storage like Amazon S3.
        /// In the context of a relational database a storage name would refere to a database name.
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
        /// E.g. generic relational database: Table; Amazon S3: Bucket; MongoDB: Collection
        /// </summary>
        /// <returns>If successfull an IResult with the name of the current colleciton</returns>
        IResult<string> GetCollectionName();

        /// <summary>
        /// Delets the collection with the given collectionName 
        /// A collection is a container for data that may be called different with different storages.
        /// E.g. generic relational database: Table, Amazon S3: Bucket, MongoDB: Collection
        /// </summary>
        /// <returns></returns>
        IResult DeleteCollection();

        /// <summary>
        /// Privides a Result with an object used by this class to establish the storage connection.
        /// This object typically is a database driver or similar. 
        /// </summary>
        /// <typeparam name="T">The type of the object that manages the database connection</typeparam>
        /// <returns>A Result with an object used by this class to establish the storage connection</returns>
        IResult<T> GetDriver<T>();

        /// <summary>
        /// Retruns an specified entry from the given collection
        /// </summary>
        /// <param name="key">The key of the entry to be retrieved</param>
        /// <returns>If successful an IResult with the retrieved entry</returns>
        new IResult<TElement> Retrieve(TIdentifier key);

        /// <summary>
        /// Creates an entry within the given collection
        /// </summary>
        /// <param name="key">The key of the entry to be updated</param>
        /// <param name="updateEntry">The updated version of the entry</param>
        /// <returns>If successful an IResult with the created or updated entry</returns>
        new IResult<TElement> CreateOrUpdate(TIdentifier key, TElement
            updateEntry);

        /// <summary>
        /// Returns a QueryableElementContainer of all entries of a collection
        /// </summary>
        /// <returns>If successful an IResult with a QueryableElementContainer of entries</returns>
        new IResult<IQueryableElementContainer<T>> RetrieveAll<T>() where T : class, IReferable, IModelElement;

        /// <summary>
        /// Returns a QueryableElementContainer of all entries of a collection that match the Predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Represents the method that defines a set of criteria and determines whether the specified object meets those criteria.</param>
        /// <returns>If successfull an IResult with a QueryableElementContainer of all entries of a collection that match the Predicate</returns>
        IResult<IQueryableElementContainer<T>> RetrieveAll<T>(Predicate<T> predicate) where T : class, IReferable, IModelElement;

        /// <summary>
        /// Deletes a specified Entry
        /// </summary>
        /// <param name="key">The key of the entry to be deleted</param>
        /// <returns></returns>
        new IResult Delete(TIdentifier key);
    }
}