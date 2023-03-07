using BaSyx.API.Clients;
using BaSyx.Utils.Settings.ArangoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Clients.ArangoDB;

public class ArangoStorageClientTests : StorageClientTestSuite
{
    protected override IStorageClient<object> GetStorageClient()
    {
        ArangoDbConfiguration config = new ArangoDbConfiguration()
        {
            Port = "8529",
            Server = "localhost",
            Database = "Hello_BaSyx"
        };

        AsyncArangoAPIWrapper arangoAPI = new AsyncArangoAPIWrapper(config);
        return new ArangoStorageClient<object>(storageName, collectionName, arangoAPI);
    }
}
