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

using System.Linq;
using System.Text;

namespace BaSyx.Utils.Settings.ArangoDB;

public class ArangoDbConfiguration
{
    public string Server { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string DbUser { get; set; }
    public string DbPassword { get; set; }
    public string SysUser { get; set; }  
    public string SysPassword { get; set; }


    public override string ToString()
    {
        string name = GetType().Name;
        StringBuilder props = GetType().GetProperties()
            .Select(pair => (pair.Name, Value: pair.GetValue(this, null) ?? "null"))
            .Aggregate(
                new StringBuilder(),
                (sb, pair) => sb.AppendJoin(": ", pair.Name, $"{pair.Value}, "),
                sb => sb.Remove(sb.Length-2, 2));
        return $"{name}: [{props}]";
    }
}
