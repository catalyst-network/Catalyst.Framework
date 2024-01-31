#region LICENSE

/**
* Copyright (c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System.Text;
using Catalyst.Core.Lib.Dag;
using Catalyst.Core.Modules.Dfs.LinkedData;
using NUnit.Framework;

namespace Catalyst.Core.Modules.Dfs.Tests.IntegrationTests.LinkedData
{
    public class ProtobufFormatTest
    {
        private ILinkedDataFormat formatter = new ProtobufFormat();

        [Test]
        public void Empty()
        {
            var data = new byte[0];
            var node = new DagNode(data);

            var cbor = formatter.Deserialise(node.ToArray());
            Assert.That(data, Is.EqualTo(cbor["data"].GetByteString()));
            Assert.That(0, Is.EqualTo(cbor["links"].Values.Count));

            var node1 = formatter.Serialize(cbor);
            Assert.That(node.ToArray(), Is.EqualTo(node1));
        }

        [Test]
        public void DataOnly()
        {
            var data = Encoding.UTF8.GetBytes("abc");
            var node = new DagNode(data);

            var cbor = formatter.Deserialise(node.ToArray());
            Assert.That(data, Is.EqualTo(cbor["data"].GetByteString()));
            Assert.That(0, Is.EqualTo(cbor["links"].Values.Count));

            var node1 = formatter.Serialize(cbor);
            Assert.That(node.ToArray(), Is.EqualTo(node1));
        }

        [Test]
        public void LinksOnly()
        {
            var a = Encoding.UTF8.GetBytes("a");
            var anode = new DagNode(a);
            var alink = anode.ToLink("a");

            var b = Encoding.UTF8.GetBytes("b");
            var bnode = new DagNode(b);
            var blink = bnode.ToLink();

            var node = new DagNode(null, new[] {alink, blink});
            var cbor = formatter.Deserialise(node.ToArray());

            Assert.That(2, Is.EqualTo(cbor["links"].Values.Count));

            var link = cbor["links"][0];
            Assert.That("QmYpoNmG5SWACYfXsDztDNHs29WiJdmP7yfcMd7oVa75Qv", Is.EqualTo(link["Cid"]["/"].AsString()));
            Assert.That("", Is.EqualTo(link["Name"].AsString()));
            Assert.That(3, Is.EqualTo(link["Size"].AsInt32()));

            link = cbor["links"][1];
            Assert.That("QmQke7LGtfu3GjFP3AnrP8vpEepQ6C5aJSALKAq653bkRi", Is.EqualTo(link["Cid"]["/"].AsString()));
            Assert.That("a", Is.EqualTo(link["Name"].AsString()));
            Assert.That(3, Is.EqualTo(link["Size"].AsInt32()));

            var node1 = formatter.Serialize(cbor);
            Assert.That(node.ToArray(), Is.EqualTo(node1));
        }
    }
}
