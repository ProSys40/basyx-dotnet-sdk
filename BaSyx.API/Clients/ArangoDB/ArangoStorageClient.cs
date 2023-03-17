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
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace BaSyx.API.Clients;

public class ArangoStorageClient<T> : StorageClient<T>
{
    private AsyncArangoAPIWrapper _arangoAPI;
    public AsyncArangoAPIWrapper ArangoAPI { get { return _arangoAPI; } private set { _arangoAPI = value; } }
    private readonly object _lock = new();

    public ArangoStorageClient(string storageName, string collectionName, AsyncArangoAPIWrapper arangoAPI) : base(storageName, collectionName)
    {
        ArangoAPI = arangoAPI;
        CreateStorageIfNotExists();
        CreateCollectionIfNotExists();
        Console.WriteLine(_arangoAPI.GetType());
    }

    public override IResult<T> CreateOrUpdate(string key, T entry)
    {
        IResult<T> updateResult = Update(key, entry);
        if (updateResult.Success && updateResult != null)
            return updateResult;
        IResult<T> createResult = Create(key, entry);
        if (createResult.Success && createResult != null)
            return createResult;
        return new Result<T>(false);
    }

    private IResult<T> Update(string key, T updateEntry)
    {
        PutDocumentResponse<T> updateResult;
        try
        {
            lock (_lock)
            {
                updateResult = _arangoAPI.Update(_storageName, _collectionName, key, updateEntry).Result;
            }
            return new Result<T>(true, updateResult.New);
        }
        catch (Exception e)
        {
            return new Result<T>(e);
        }
    }

    private IResult<T> Create(string key, T entry)
    {
        PostDocumentResponse<T> createResult;
        dynamic entryWithKey = InjectKey(entry, key);
        try
        {
            lock (_lock)
            {
                createResult = _arangoAPI.Create<dynamic, T>(_storageName, _collectionName, entryWithKey).Result;
            }
            return new Result<T>(true, createResult.New);
        }
        catch (Exception e)
        {
            return new Result<T>(e);
        }
    }

    private dynamic InjectKey(T entry, string key)
    {
        dynamic entryWithKey = new ExpandoObject();
        entryWithKey = MapPropertiesToDynamicObject(entry, entryWithKey);

        entryWithKey._key = key;
        return entryWithKey;
    }

    private dynamic MapPropertiesToDynamicObject(T entry, dynamic dynObject)
    {
        var dynDict = (IDictionary<string, object>)dynObject;

        foreach (var property in entry.GetType().GetProperties())
            dynDict.Add(property.Name, property.GetValue(entry));

        return dynObject;
    }

    public override IResult Delete(string key)
    {
        DeleteDocumentResponse<T> deleteResult;
        try
        {
            lock (_lock)
            {
                deleteResult = _arangoAPI.Delete<T>(_storageName, _collectionName, key).Result;
            }

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(e);
        }
    }

    public override IResult DeleteCollection()
    {
        DeleteCollectionResponse deleteResult;
        try
        {
            lock (_lock)
            {
                deleteResult = _arangoAPI.DeleteCollection(_storageName, _collectionName).Result;
            }

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(e);
        }
    }

    public override IResult DeleteStorage()
    {
        DeleteDatabaseResponse deleteResult;
        try
        {
            lock (_lock)
            {
                deleteResult = _arangoAPI.DeleteDatabase(_storageName).Result;
            }
            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(e);
        }
    }

    public override IResult<string> GetCollection()
    {
        GetCollectionResponse getResult;
        try
        {
            lock (_lock)
            {
                getResult = _arangoAPI.RetrieveCollection(_storageName, _collectionName).Result;
            }
            return new Result<string>(true, getResult.Name);
        }
        catch (Exception e)
        {
            return new Result<string>(e);
        }
    }

    public override IResult<string> GetStorageName()
    {
        GetCurrentDatabaseInfoResponse databeseInfoResult;
        lock (_lock)
        {
            databeseInfoResult = _arangoAPI.RetrieveDatabaseInfo(_storageName).Result;
        }
        return new Result<string>(true, databeseInfoResult.Result.Name);
    }

    public override IResult<T> Retrieve(string key)
    {
        T entryResult;
        try
        {
            lock (_lock)
            {
                entryResult = _arangoAPI.Retrieve<T>(_storageName, _collectionName, key).Result;
            }
            return new Result<T>(true, entryResult);
        }
        catch (Exception e)
        {
            return new Result<T>(e);
        }
    }

    public override IResult<List<T>> RetrieveAll()
    {
        List<T> entriesResult;
        try
        {
            lock (_lock)
            {
                entriesResult = _arangoAPI.RetrieveAll<T>(_storageName, _collectionName).Result;
            }
            return new Result<List<T>>(true, entriesResult);
        }
        catch (Exception e)
        {
            return new Result<List<T>>(e);
        }
    }

    public override IResult<List<T>> RetrieveMultiple(List<string> keys)
    {
        List<T> entriesResult;
        try
        {
            lock (_lock)
            {
                entriesResult = _arangoAPI.RetrieveMultiple<T>(_storageName, _collectionName, keys).Result;
            }
            return new Result<List<T>>(true, entriesResult);
        }
        catch (Exception e)
        {
            return new Result<List<T>>(e);
        }
    }

    protected override IResult<string> CreateCollection()
    {
        PostCollectionResponse createResult;
        try
        {
            lock (_lock)
            {
                createResult = _arangoAPI.CreateCollection(_storageName, _collectionName).Result;
            }
            return new Result<string>(true, createResult.Name);
        }
        catch (Exception e)
        {
            return new Result<string>(e);
        }
    }

    protected override IResult<string> CreateStorage()
    {
        PostDatabaseResponse createResult;
        try
        {
            lock (_lock)
            {
                createResult = _arangoAPI.CreateDB(_storageName).Result;
            }
            return new Result<string>(createResult.Result, _storageName);
        } catch (Exception e)
        {
            return new Result<string>(e);
        }
    }

    protected override IResult<List<string>> GetStorageNames()
    {
        GetDatabasesResponse retrieveDbResult;
        try
        {
            lock (_lock)
            {
                retrieveDbResult = _arangoAPI.RetrieveAllDatabases().Result;
            }
            return new Result<List<string>>(true, retrieveDbResult.Result.ToList());
        } catch (Exception e)
        {
            return new Result<List<string>>(e);
        }
    }
}
