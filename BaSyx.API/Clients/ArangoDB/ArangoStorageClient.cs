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
using System.Linq;

namespace BaSyx.API.Clients;

public class ArangoStorageClient<T> : StorageClient<T>
{
    private readonly AsyncArangoAPIWrapper _arangoAPI;
    private readonly object _lock = new object();

    public ArangoStorageClient(string storageName, string collectionName, AsyncArangoAPIWrapper arangoAPI) : base(storageName, collectionName)
    {
        _arangoAPI = arangoAPI;
    }

    public override IResult<T> CreateOrUpdate(string key, T updateEntry)
    {
        IResult<T> updateResult = Update(key, updateEntry);
        if (updateResult != null)
            return new Result<T>(true, updateResult.Entity);
        IResult<T> createResult = Create(updateEntry);
        if (createResult != null)
            return new Result<T>(true, createResult.Entity);
        return new Result<T>(false);
    }

    private IResult<T> Update(string key, T updateEntry)
    {
        PutDocumentResponse<T> updateResult;
        try
        {
            lock (_lock)
            {
                updateResult = _arangoAPI.Update(_collectionName, key, updateEntry).Result;
            }
            return new Result<T>(true, updateResult.New);
        }
        catch (Exception e)
        {
            return new Result<T>(e);
        }
    }

    private IResult<T> Create(T updateEntry)
    {
        PostDocumentResponse<T> createResult;
        try
        {
            lock (_lock)
            {
                createResult = _arangoAPI.Create(_collectionName, updateEntry).Result;
            }
            return new Result<T>(true, createResult.New);
        }
        catch (Exception e)
        {
            return new Result<T>(e);
        }
    }

    public override IResult Delete(string key)
    {
        DeleteDocumentResponse<T> deleteResult;
        try
        {
            lock (_lock)
            {
                deleteResult = _arangoAPI.Delete<T>(_collectionName, key).Result;
            }

            return new Result(deleteResult.Old != null);
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
                deleteResult = _arangoAPI.DeleteCollection(_collectionName).Result;
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
                deleteResult = _arangoAPI.DeleteDatabase(_collectionName).Result;
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
                getResult = _arangoAPI.RetrieveCollection(_collectionName).Result;
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
            databeseInfoResult = _arangoAPI.RetrieveDatabaseInfo().Result;
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
                entryResult = _arangoAPI.Retrieve<T>(_collectionName, key).Result;
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
                entriesResult = _arangoAPI.RetrieveAll<T>(_collectionName).Result;
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
                createResult = _arangoAPI.CreateCollection(_collectionName).Result;
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
