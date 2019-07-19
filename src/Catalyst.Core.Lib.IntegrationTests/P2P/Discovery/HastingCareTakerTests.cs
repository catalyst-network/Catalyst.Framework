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

using System.Collections.Generic;
using System.Linq;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.Interfaces.P2P.Discovery;
using Catalyst.Core.Lib.P2P.Discovery;
using Catalyst.TestUtils;
using FluentAssertions;
using Xunit;

namespace Catalyst.Core.Lib.IntegrationTests.P2P.Discovery
{
    public sealed class HastingCareTakerTests
    {
        private IPeerIdentifier _ownNode;

        public HastingCareTakerTests()
        {
            _ownNode = PeerIdentifierHelper.GetPeerIdentifier("own_node");
        }
        
        [Fact]
        public void Can_Add_New_Mementos_To_Caretaker()
        {
            var careTaker = new HastingCareTaker();

            var stack = new Stack<IHastingMemento>();
            stack.Push(DiscoveryHelper.SubMemento(_ownNode));
            
            var history = DiscoveryHelper.MockMementoHistory(stack);
            
            history.ToList().ForEach(m => careTaker.Add(m));

            careTaker.HastingMementoList.Should().Contain(history);
        }

        [Fact]
        public void Taking_From_Memento_List_Takes_LIFO()
        {
            var careTaker = new HastingCareTaker();

            var stack = new Stack<IHastingMemento>();
            stack.Push(DiscoveryHelper.SubMemento(_ownNode));
            
            var history = DiscoveryHelper.MockMementoHistory(stack);
            
            history.ToList().ForEach(m => careTaker.Add(m));

            careTaker.Get().Should().Be(history.Last());
            careTaker.HastingMementoList.First().Should().Be(history.First());
            careTaker.HastingMementoList.Count.Should().Be(history.Count - 1);
        }
    }
}
