﻿/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System.Xml.Serialization;

namespace BaSyx.Utils.Client.Mqtt
{
    public class MqttCredentials : IMqttCredentials
    {
        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string Password { get; set; }

        public MqttCredentials() { }

        public MqttCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }       
    }
}
