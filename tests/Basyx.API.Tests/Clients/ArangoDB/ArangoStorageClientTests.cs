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
    protected override IStorageClient<TestObject> GetStorageClient()
    {
        ArangoDbConfiguration config = new()
        {
            Port = "8529",
            Server = "localhost",
        };

        AsyncArangoAPIWrapper arangoAPI = new(config);
        return new ArangoStorageClient<TestObject>(storageName, collectionName, arangoAPI);
    }
}
