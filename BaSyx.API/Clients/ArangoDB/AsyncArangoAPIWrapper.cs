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
using ArangoDBNetStandard.CollectionApi.Models;
using ArangoDBNetStandard.DatabaseApi.Models;
using ArangoDBNetStandard.DocumentApi.Models;
using ArangoDBNetStandard;
using ArangoDBNetStandard.Transport.Http;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings.ArangoDB;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ArangoDBNetStandard.DatabaseApi;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System.Runtime.InteropServices;
using static BaSyx.API.Clients.AsyncArangoAPIWrapper;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using ArangoDBNetStandard.Serialization;
using BaSyx.Utils.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

namespace BaSyx.API.Clients;

/// <summary>
/// A simple arango systemClient client wrapper that provides basic functionality in an async manner 
/// </summary>
public class AsyncArangoAPIWrapper
{
    private readonly ArangoDbConfiguration _config;

    /// <summary>
    /// Constructor to create an ArangoDB client.
    /// The method CreateBaSyxContextIfNotExists trys to connect log in a given database.
    /// If that database does not exist and the config contains a system user and a system password
    /// it is tried to create a new database with the system credentials.
    /// The extractKeyMethod shall be provided to create an unique key from an object.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="generateUniqueKey"></param>
    public AsyncArangoAPIWrapper(ArangoDbConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Creates a database with the given name and a list of databank users
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="users"></param> nullable
    /// <returns></returns>
    public async Task<PostDatabaseResponse> CreateDB(string dbName, List<DatabaseUser> users)
    {
        return await ArangoSystemWorker<PostDatabaseResponse>(async systemClient =>
        {
            var body = new PostDatabaseBody()
            {
                Name = dbName,
                Users = users
            };
            return await systemClient.PostDatabaseAsync(body);
        });
    }

    /// <summary>
    /// Creates a database with the fiven name and a database user defined by a username and a password.
    /// The credentials may be handed over in the signature or it is tried to be loaded from the config.
    /// If either username or password or both is not set, an unauthorised databse will be created.
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="username"></param> nullable
    /// <param name="password"></param> nullable
    /// <returns></returns>
    public async Task<PostDatabaseResponse> CreateDB(string dbName, string username = null, string password = null)
    {
        username = username ?? _config.DbUser;
        password = password ?? _config.DbPassword;
        List<DatabaseUser> users = username == null || password == null ? null : new List<DatabaseUser>
        {
            new DatabaseUser
            {
                Username = username,
                Passwd = password
            }
        };
        return await CreateDB(dbName, users);
    }

    /// <summary>
    /// returns all available databases
    /// </summary>
    /// <returns></returns>
    public async Task<GetDatabasesResponse> RetrieveAllDatabases()
    {
        return await ArangoSystemWorker<GetDatabasesResponse>(async systemClient => await systemClient.GetDatabasesAsync());
    }

    /// <summary>
    /// returns the current database
    /// </summary>
    /// <returns></returns>
    public async Task<GetCurrentDatabaseInfoResponse> RetrieveDatabaseInfo()
    {
        return await ArangoWorker<GetCurrentDatabaseInfoResponse>(async db => await db.Database.GetCurrentDatabaseInfoAsync());
    }

    /// <summary>
    /// Deletes the database with the given name
    /// </summary>
    /// <param name="dbName"></param>
    /// <returns></returns>
    public async Task<DeleteDatabaseResponse> DeleteDatabase(string dbName)
    {
        return await ArangoSystemWorker<DeleteDatabaseResponse>(async systemClient => await systemClient.DeleteDatabaseAsync(dbName));
    }

    /// <summary>
    /// Creates a collection with the given collection name.
    /// A collection is a abstraction for a set of data of the same type.
    /// In a relational systemClient context this would be a table.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <returns></returns>
    public async Task<PostCollectionResponse> CreateCollection(string collectionName)
    {
        return await ArangoWorker<PostCollectionResponse>(async dbClient =>
        {
            var body = new PostCollectionBody()
            {
                Type = CollectionType.Document,
                Name = collectionName
            };
            return await dbClient.Collection.PostCollectionAsync(body);
        });
    }

    /// <summary>
    /// returns all available collections
    /// </summary>
    /// <returns></returns>
    public async Task<GetCollectionsResponse> RetrieveAllCollections()
    {
        return await ArangoWorker<GetCollectionsResponse>(async dbClient => await dbClient.Collection.GetCollectionsAsync());
    }

    public async Task<GetCollectionResponse> RetrieveCollection(string collectionName)
    {
        return await ArangoWorker<GetCollectionResponse>(async dbClient => await dbClient.Collection.GetCollectionAsync(collectionName));
    }

    /// <summary>
    /// Creates a systemClient entry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    public async Task<PostDocumentResponse<T>> Create<T>(string collectionName, T document)
    {
        var query = new PostDocumentsQuery();
        query.Overwrite = true;
        query.ReturnNew = true;

        return await ArangoWorker<PostDocumentResponse<T>>(async dbClient => await dbClient.Document.PostDocumentAsync<T>(collectionName, document, query));
    }

    /// <summary>
    /// Updates a systemClient entry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <param name="documentKey"></param>
    /// <param name="updateObject"></param>
    /// <returns></returns>
    public async Task<PutDocumentResponse<T>> Update<T>(string collectionName, string documentKey, T updateObject)
    {
        var query = new PutDocumentQuery();
        query.ReturnNew = true; 
        return await ArangoWorker<PutDocumentResponse<T>>(async dbClient => await dbClient.Document.PutDocumentAsync<T>(collectionName, documentKey, updateObject, query));
    }

    /// <summary>
    /// Partially updates a systemClient entry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="collectionName"></param>
    /// <param name="documentKey"></param>
    /// <param name="patchObject"></param>
    /// <returns></returns>
    public async Task<PatchDocumentResponse<U>> Patch<T, U>(string collectionName, string documentKey, T patchObject)
    {
        return await ArangoWorker<PatchDocumentResponse<U>>(async dbClient => await dbClient.Document.PatchDocumentAsync<T, U>(collectionName, documentKey, patchObject));
    }

    public async Task<DeleteCollectionResponse> DeleteCollection(string collectionName)
    {
        return await ArangoWorker<DeleteCollectionResponse>(async db => await db.Collection.DeleteCollectionAsync(collectionName));
    }

    /// <summary>
    /// Returns all entries from the given collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <returns></returns>
    public async Task<List<T>> RetrieveAll<T>(string collectionName)
    {
        return await ArangoWorker<List<T>>(async db => await db.Document.GetDocumentsAsync<T>(collectionName, null));
    }

    /// <summary>
    /// Returns a entry with the given key from the given collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> Retrieve<T>(string collectionName, string key) {
        return await ArangoWorker<T>(async db => await db.Document.GetDocumentAsync<T>(collectionName, key));
    }

    /// <summary>
    /// Deletes an entry with the given key from the given collection 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName"></param>
    /// <param name="documentKey"></param>
    /// <returns></returns>
    public async Task<DeleteDocumentResponse<T>> Delete<T>(string collectionName, string documentKey)
    {
        return await ArangoBasicAuthWorker<DeleteDocumentResponse<T>>(async db => await db.Document.DeleteDocumentAsync<T>(collectionName, documentKey));
    }

    private bool IsSetSysUserCredentials(ArangoDbConfiguration config)
    {
        return config.SysUser != null && config.SysPassword != null;
    }

    private bool IsSetUserCredentials(ArangoDbConfiguration config)
    {
        return config.DbUser != null && config.DbPassword != null;
    }

    private async Task<T> ArangoSystemWorker<T>(Func<DatabaseApiClient, Task<T>> func) => IsSetSysUserCredentials(_config) ? await ArangoSystemBasicAuthWorker<T>(func) : await ArangoSystemNoAuthWorker<T>(func);

    private async Task<T> ArangoWorker<T>(Func<ArangoDBClient, Task<T>> func) => IsSetUserCredentials(_config) ? await ArangoBasicAuthWorker<T>(func) : await ArangoNoAuthWorker<T>(func);

    private async Task<T> ArangoSystemBasicAuthWorker<T>(Func<DatabaseApiClient, Task<T>> func)
    {
        using (var transport = HttpApiTransport.UsingBasicAuth(generateHostUri(_config), _config.SysUser, _config.SysPassword))
        {
            DatabaseApiClient systemClient = new DatabaseApiClient(transport);
            return await func(systemClient);
        }
    }

    private async Task<T> ArangoSystemNoAuthWorker<T>(Func<DatabaseApiClient, Task<T>> func)
    {
        using (var transport = HttpApiTransport.UsingNoAuth(generateHostUri(_config)))
        {
            DatabaseApiClient systemClient = new DatabaseApiClient(transport);
            return await func(systemClient);
        }
    }

    private async Task<T> ArangoBasicAuthWorker<T>(Func<ArangoDBClient, Task<T>> func)
    {
        using (var transport = HttpApiTransport.UsingBasicAuth(generateHostUri(_config), _config.Database, _config.DbUser, _config.DbPassword))
        {
            ArangoDBClient arangoClient = new ArangoDBClient(transport);
            return await func(arangoClient);
        }
    }

    private async Task<T> ArangoNoAuthWorker<T>(Func<ArangoDBClient, Task<T>> func)
    {
        using (var transport = HttpApiTransport.UsingNoAuth(generateHostUri(_config), _config.Database))
        {
            ArangoDBClient arangoClient = new ArangoDBClient(transport);
            return await func(arangoClient);
        }
    }


    private Uri generateHostUri(ArangoDbConfiguration config)
    {
        //TODO: supports https?
        return new Uri($"http://{config.Server}:{config.Port}");
    }
}

