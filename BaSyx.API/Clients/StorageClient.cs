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
using BaSyx.Utils.JsonHandling;
using BaSyx.Utils.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSyx.API.Clients
{
    public abstract class StorageClient<T> : IStorageClient<T>
    {
        protected readonly string _storageName;
        protected readonly string _collectionName;

        public StorageClient(string storageName, string collectionName)
        {
            _storageName = storageName;
            _collectionName = collectionName;
        }

        public abstract IResult<T> CreateOrUpdate(string key, T updateEntry);
        public abstract IResult<T> Retrieve(string key);
        public abstract IResult<List<T>> RetrieveAll();
        public abstract IResult<List<T>> RetrieveMultiple(List<string> keys);
        public abstract IResult Delete(string key);
        
        public abstract IResult<string> GetStorageName();
        public abstract IResult<string> GetCollection();
        public abstract IResult DeleteStorage();
        public abstract IResult DeleteCollection();

        protected abstract IResult<string> CreateCollection();
        protected abstract IResult<string> CreateStorage();
        protected abstract IResult<List<string>> GetStorageNames();

        protected void CreateStorageIfNotExists()
        {
            if (storageExists())
                return;

            CreateStorage();
        }

        private bool storageExists()
        {
            IResult<List<string>> storagesResult = GetStorageNames();
            if (!storagesResult.Success && storagesResult.Entity != null)
                throw new Exception("Could not laod storage names");

            List<string> storageNames = storagesResult.Entity;
            if (storageNames == null)
            {
                return false;
            }
            return storageNames.Contains(_storageName);
        }

        protected void CreateCollectionIfNotExists()
        {
            IResult<string> collectionResult = GetCollection();
            if (collectionResult.Success && collectionResult.Entity.Equals(_collectionName))
                return;
            CreateCollection();
        }
    }
}
