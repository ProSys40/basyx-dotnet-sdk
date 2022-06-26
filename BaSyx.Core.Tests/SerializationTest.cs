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
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Utils.StringOperations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BaSyx.Core.Tests
{
    [TestClass]
    public class SerializationTest
    {

        [TestMethod]
        public void TestMethod1_Property()
        {
            Property<int> property = new Property<int>("TestIntegerProperty", 0);
            property.SetValue(1);
            string jsonProperty = property.ToJson();

            Property deserializedProperty = JsonConvert.DeserializeObject<Property>(jsonProperty);
            deserializedProperty.Should().NotBeNull();
            deserializedProperty.ToObject<int>().Should().Be(1);
        }

        [TestMethod]
        public void TestMethod2_DataType()
        {
            DataType dataType = new DataType(typeof(int));

            string jsonDataType = JsonConvert.SerializeObject(dataType, Formatting.Indented);
            DataType deserializedDataType = JsonConvert.DeserializeObject<DataType>(jsonDataType);
            deserializedDataType.Should().NotBeNull();
            deserializedDataType.SystemType.Should().Be(typeof(int));
        }

        [TestMethod]
        public void TestMethod3_Range()
        {
            DataType dataType = new DataType(typeof(int));
            Range range = new Range("MyRange", dataType);
            range.Min = new ElementValue(5, dataType);
            range.Max = new ElementValue(8, dataType);

            string jsonDataType = JsonConvert.SerializeObject(range, Formatting.Indented);
            Range deserializedRange = JsonConvert.DeserializeObject<Range>(jsonDataType);
            deserializedRange.Should().NotBeNull();           
        }

        [TestMethod]
        public void TestMethod4_Blob()
        {
            string textValue = "This is my blob";
            Blob blob = new Blob("MyBlob");
            blob.MimeType = "application/pdf";
            blob.SetValue(textValue);

            string jsonDataType = JsonConvert.SerializeObject(blob, Formatting.Indented);
            Blob deserializedBlob = JsonConvert.DeserializeObject<Blob>(jsonDataType);
            deserializedBlob.Should().NotBeNull();
            StringOperations.Base64Decode(deserializedBlob.Value).Should().BeEquivalentTo(textValue);
        }
    }
}
