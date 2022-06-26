/*******************************************************************************
* Copyright (c) 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export;
using BaSyx.Models.Extensions;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.JsonHandling;
using BaSyx.Utils.ResultHandling;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class ExportTest
    {
        [TestMethod]
        public void TestMethod1_JsonEnvironment()
        {
            var shell = new AssetAdministrationShell("TestShell", new BaSyxShellIdentifier("TestShell", "1.0.0"))
            {
                Asset = new Asset("TestAsset", new BaSyxAssetIdentifier("TestAsset", "1.0.0")),
                Description = new LangStringSet()
                {
                   new LangString("de-DE", "TestShell"),
                   new LangString("en-US", "TestShell")
                },
                Administration = new AdministrativeInformation()
                {
                    Version = "1.0",
                    Revision = "120"
                }
            };
            Submodel submodel = new Submodel("TestSubmodel", new BaSyxSubmodelIdentifier("TestSubmodel", "1.0.0"))
            {
                SubmodelElements =
                {
                    new Property<string>("TestStringProperty", "TestStringValue"),
                    new Operation("TestOperation")
                    {
                        InputVariables = new OperationVariableSet()
                        {
                             new Property<string>("TestInParam1"),
                             new Property<int>("TestInParam2"),
                             new SubmodelElementCollection("TestInParam3")
                             {
                                 new Property<bool>("TestInBool"),
                                new SubmodelElementCollection<int>("TestIntSubParam")
                             }
                        },
                        OutputVariables = new OperationVariableSet()
                        {
                            new Property<string>("TestOutParam1"),
                            new SubmodelElementCollection("TestOutParam2") 
                            {
                                new Property<bool>("TestOutBool"),
                                new SubmodelElementCollection<int>("TestIntSubParam")
                            }
                        },
                        OnMethodCalled = (op, inArgs, inOutArgs, outArgs, cancellationToken) =>
                        {
                            outArgs.Add(new Property<string>("TestOutParam1", "TestOutValue1"));
                            outArgs.Add(new SubmodelElementCollection("TestOutParam2")
                            {
                                new SubmodelElementCollection<int>("TestIntSubParam", new[] { 5, 6, 7 })
                            });
                            return new OperationResult(true);
                        }
                    }
                }
            };

            shell.Submodels.Add(submodel);
            AssetAdministrationShellEnvironment_V2_0 aasEnv = new AssetAdministrationShellEnvironment_V2_0(shell);

            string tempFileName = Path.Join(Path.GetTempPath(), "aasEnv.json");
            aasEnv.WriteEnvironment_V2_0(ExportType.Json, tempFileName);

            AssetAdministrationShellEnvironment_V2_0 aasEnv2 = AssetAdministrationShellEnvironment_V2_0.ReadEnvironment_V2_0(tempFileName);
        }

        [TestMethod]
        public void TestMethod2_ComplianceCheck_AASX()
        {
            using(AASX aasx = new AASX(@"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example2.aasx"))
            {
                var oldEnvironment = aasx.GetEnvironment_V2_0();
                var files = aasx.SupplementaryFiles;
                
                using (AASX aasx_rewritten = new AASX(@"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example2_rewritten.aasx"))
                {
                    aasx_rewritten.AddEnvironment(new Identifier("data", KeyType.Custom), oldEnvironment, ExportType.Xml);
                    foreach (var file in files)
                    {
                        var stream = aasx.GetFileAsStream(file.Uri, out string contentType);
                        aasx_rewritten.AddStreamToAASX(file.Uri.ToString(), stream, contentType);
                    }
                    /*
                    var oldProps = aasx.GetPackageProperties();
                    aasx_rewritten.AddCoreProperties(pp =>
                    {
                        pp.Category = oldProps.Category;
                        pp.ContentStatus = oldProps.ContentStatus;
                        pp.ContentType = oldProps.ContentType;
                        pp.Created = oldProps.Created;
                        pp.Creator = oldProps.Creator;
                        pp.Description = oldProps.Description;
                        pp.Identifier = oldProps.Identifier;
                        pp.Keywords = oldProps.Keywords;
                        pp.Language = oldProps.Language;
                        pp.LastModifiedBy = oldProps.LastModifiedBy;
                        pp.LastPrinted = oldProps.LastPrinted;
                        pp.Modified = oldProps.Modified;
                        pp.Revision = oldProps.Revision;
                        pp.Subject = oldProps.Subject;
                        pp.Title = oldProps.Title;
                        pp.Version = oldProps.Version;
                    });
                    */
                }

            }
        }

        [TestMethod]
        public void TestMethod3_ComplianceCheckXml()
        {
            var environment = AssetAdministrationShellEnvironment_V2_0.ReadEnvironment_V2_0(@"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example.xml");
            AssetAdministrationShellEnvironment_V2_0
                .WriteEnvironment_V2_0(environment, ExportType.Xml, @"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example_rewritten.xml");            
        }

        [TestMethod]
        public void TestMethod4_ComplianceCheckJson()
        {
            var environment = AssetAdministrationShellEnvironment_V2_0.ReadEnvironment_V2_0(@"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example.json");
            AssetAdministrationShellEnvironment_V2_0
                .WriteEnvironment_V2_0(environment, ExportType.Json, @"C:\Users\ZIC5HO\Desktop\Tests\test_demo_full_example_rewritten.json");
        }
    }
}
