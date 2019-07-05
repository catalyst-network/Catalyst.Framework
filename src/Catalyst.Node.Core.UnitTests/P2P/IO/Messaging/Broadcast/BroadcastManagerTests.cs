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
using System.Threading.Tasks;
using Catalyst.Common.Config;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging.Correlation;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.Interfaces.P2P.Messaging.Broadcast;
using Catalyst.Common.IO.Messaging.Broadcast;
using Catalyst.Common.IO.Messaging.Correlation;
using Catalyst.Common.IO.Messaging.Dto;
using Catalyst.Common.P2P;
using Catalyst.Node.Core.P2P.IO.Messaging.Broadcast;
using Catalyst.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using SharpRepository.InMemoryRepository;
using SharpRepository.Repository;
using Xunit;

namespace Catalyst.Node.Core.UnitTests.P2P.IO.Messaging.Broadcast
{
    public sealed class BroadcastManagerTests : IDisposable
    {
        private readonly IRepository<Peer> _peers;
        private readonly IMemoryCache _cache;

        public BroadcastManagerTests()
        {
            _peers = new InMemoryRepository<Peer>();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task Can_Increase_Broadcast_Count_When_Broadcasting()
        {
            PopulatePeers(100);
            var correlationId = await BroadcastMessage();

            _cache.TryGetValue(correlationId.Id, out BroadcastMessage value);
            value.BroadcastCount.Should().Be((uint) Constants.MaxGossipPeersPerRound);
            value.ReceivedCount.Should().Be(0);
        }

        [Fact]
        public async Task Can_Broadcast_Message_When_Not_Enough_Peers_To_Gossip()
        {
            PopulatePeers(Constants.MaxGossipPeersPerRound - 1);
            var correlationId = await BroadcastMessage();

            _cache.TryGetValue(correlationId.Id, out BroadcastMessage value);

            value.BroadcastCount.Should().Be((uint) Constants.MaxGossipPeersPerRound - 1);
            value.ReceivedCount.Should().Be(0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(6)]
        [InlineData(3)]
        public async Task Can_Increase_Received_Count_When_Broadcast_Message_Is_Received(int receivedCount)
        {
            PopulatePeers(100);

            var peerIdentifier = PeerIdentifierHelper.GetPeerIdentifier("1");
            var senderIdentifier = PeerIdentifierHelper.GetPeerIdentifier("sender");
            var messageFactory = new DtoFactory();
            IBroadcastManager broadcastMessageHandler = new BroadcastManager(peerIdentifier, _peers, _cache, Substitute.For<IPeerClient>());

            var messageDto = messageFactory.GetDto(
                TransactionHelper.GetTransaction(),
                peerIdentifier,
                senderIdentifier,
                CorrelationId.GenerateCorrelationId()
            );

            var gossipDto = messageDto.Content.ToProtocolMessage(senderIdentifier.PeerId, messageDto.CorrelationId);

            await broadcastMessageHandler.ReceiveAsync(gossipDto);

            _cache.TryGetValue(messageDto.CorrelationId.Id, out BroadcastMessage value);
            value.ReceivedCount.Should().Be(1);

            for (var i = 0; i < receivedCount; i++)
            {
                await broadcastMessageHandler.ReceiveAsync(gossipDto);
            }

            _cache.TryGetValue(messageDto.CorrelationId.Id, out value);
            value.ReceivedCount.Should().Be((uint) receivedCount + 1);
        }

        private async Task<ICorrelationId> BroadcastMessage()
        {
            var senderPeerIdentifier = PeerIdentifierHelper.GetPeerIdentifier("sender");

            var gossipMessageHandler = new
                BroadcastManager(senderPeerIdentifier, _peers, _cache, Substitute.For<IPeerClient>());

            var gossipMessage = TransactionHelper.GetTransaction().ToProtocolMessage(senderPeerIdentifier.PeerId);

            await gossipMessageHandler.BroadcastAsync(gossipMessage);
            return gossipMessage.CorrelationId.ToCorrelationId();
        }

        private void PopulatePeers(int count)
        {
            for (var i = 10; i < count + 10; i++)
            {
                _peers.Add(new Peer
                {
                    PeerIdentifier = PeerIdentifierHelper.GetPeerIdentifier(i.ToString())
                });
            }

            _peers.Count().Should().Be(count);
        }

        public void Dispose()
        {
            _cache.Dispose();
            _peers.Dispose();
        }
    }
}
