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

using BaSyx.API.Clients.Persistency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyx.API.Tests.Clients.Persistency;

public abstract class StroageClientTestSuite : IDisposable
{


    public abstract IStorageClient StorageClient { get; }

    public StroageClientTestSuite()
    {
        SetUp();
    }

    public static void SetUp()
    {
        // TODO
    }

    [Fact]
    public void TestConneciton()
    {
        // before connect: assert not connected
        // connect: assert connected 
        // disconnect: assert disconnected
    }

    [Fact]
    public void ConnecttionWithInvalidConnectionsParams_Fails()
    {
        // TODO
    }

    [Fact]
    public void CreateCollection()
    {
        // TODO
    }

    [Fact]
    public void GetCollection()
    {
        // TODO
    }

    [Fact]
    public void DeleteCollection()
    {
        // TODO
    }


    public void ClaenUp()
    {
        // TODO
    }

    public void Dispose()
    {
        ClaenUp();
    }
}
