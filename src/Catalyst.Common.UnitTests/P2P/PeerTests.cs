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

using System;
using Catalyst.Common.P2P;
using FluentAssertions;
using SharpRepository.InMemoryRepository;
using Xunit;

namespace Catalyst.Common.UnitTests.P2P
{
    public sealed class PeerTests
    {
        [Fact]
        public void EntityStoreAuditsCreateTime()
        {
            var repo = new InMemoryRepository<Peer>();
            var peer = new Peer();
            repo.Add(peer);
            var retrievedPeer = repo.Get(peer.PkId);
            DateTime now = DateTime.UtcNow;
            retrievedPeer.Created.Should().BeSameDateAs(now);
            retrievedPeer.Modified.Should().BeNull();
        }
        
        [Fact]
        public void EntityStoreAuditsModifiedTime()
        {
            var repo = new InMemoryRepository<Peer>();
            var peer = new Peer();
            repo.Add(peer);
            var retrievedPeer = repo.Get(peer.PkId);
            retrievedPeer.Touch();
            repo.Update(retrievedPeer);
            var retrievedmodified = repo.Get(peer.PkId);
            DateTime now = DateTime.UtcNow;
            retrievedmodified.Modified.Should().BeSameDateAs(now);
        }
    }
}
