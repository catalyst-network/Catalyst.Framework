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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Catalyst.Abstractions.IO.Messaging.Dto;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.Network;
using Catalyst.Core.Lib.P2P.Models;
using Catalyst.Abstractions.P2P.Repository;
using Catalyst.Core.Modules.Rpc.Server.IO.Observers;
using Catalyst.Protocol.Rpc.Node;
using Catalyst.Protocol.Wire;
using Catalyst.TestUtils;
using DotNetty.Transport.Channels;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;
using SharpRepository.InMemoryRepository;
using NUnit.Framework;
using Catalyst.Core.Lib.P2P.Repository;
using MultiFormats;
using Catalyst.Abstractions.P2P;

namespace Catalyst.Core.Lib.Tests.UnitTests.Rpc.IO.Observers
{
    /// <summary>
    ///     Tests the get peer info calls
    /// </summary>
    public sealed class GetPeerInfoRequestObserverTests
    {
        private ILogger _logger;
        private IChannelHandlerContext _fakeContext;
        private IPeerRepository _peerRepository;
        private ILibP2PPeerClient _peerClient;

        [SetUp]
        public void Init()
        {
            _logger = Substitute.For<ILogger>();
            _fakeContext = Substitute.For<IChannelHandlerContext>();
            _peerClient = Substitute.For<ILibP2PPeerClient>();

            _peerRepository = new PeerRepository(new InMemoryRepository<Peer, string>());
            _peerRepository.Add(GetPeerTestData());
        }

        public IEnumerable<Peer> GetPeerTestData()
        {
            yield return new Peer
            {
                Address =
                    PeerIdHelper.GetPeerId("publickey-1", IPAddress.Parse("172.0.0.1"), 9090),
                Reputation = 0,
                LastSeen = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
            yield return new Peer
            {
                Address =
                    PeerIdHelper.GetPeerId("publickey-2", IPAddress.Parse("172.0.0.2"), 9090),
                Reputation = 1,
                LastSeen = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
            yield return new Peer
            {
                Address =
                    PeerIdHelper.GetPeerId("publickey-3", IPAddress.Parse("172.0.0.3"), 9090),
                Reputation = 2,
                LastSeen = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
        }

        /// <summary>
        ///     Tests the get peer info request and response via RPC.
        ///     Peer is expected to be found in this case
        /// </summary>
        /// <param name="publicKey">Public key of the peer whose reputation is of interest</param>
        /// <param name="ipAddress">Ip address of the peer whose reputation is of interest</param>
        [TestCase("publickey-1", "172.0.0.1")]
        [TestCase("publickey-2", "172.0.0.2")]
        public void TestGetPeerInfoRequestResponse(string publicKey, string ipAddress)
        {
            var peerId = PeerIdHelper.GetPeerId(publicKey, ipAddress, 9090);
            var responseContent = GetPeerInfoTest(peerId);
            responseContent.PeerInfo.Count().Should().Be(1);

            foreach (var peerInfo in responseContent.PeerInfo)
            {
                peerInfo.PeerId.ToString().Should().Be(peerId.ToString());
            }
        }

        /// <summary>
        ///     Tests the get peer info request and response via RPC.
        ///     Peer is NOT expected to be found in this case, as they do not exist
        /// </summary>
        /// <param name="publicKey">Public key of the peer whose reputation is of interest</param>
        /// <param name="ipAddress">Ip address of the peer whose reputation is of interest</param>
        [TestCase("this-pk-should-not-exist", "172.0.0.1")]
        [TestCase("this-pk-should-not-exist", "172.0.0.3")]
        [TestCase("publickey-1", "0.0.0.0")]
        [TestCase("publickey-3", "0.0.0.0")]
        public void TestGetPeerInfoRequestResponseForNonExistantPeers(string publicKey, string ipAddress)
        {
            var peerId = PeerIdHelper.GetPeerId(publicKey, ipAddress, 12345);
            var responseContent = GetPeerInfoTest(peerId);
            responseContent.PeerInfo.Count.Should().Be(0);
        }

        /// <summary>
        ///     Tests the data/communication through protobuf
        /// </summary>
        /// <returns></returns>
        private GetPeerInfoResponse GetPeerInfoTest(MultiAddress peerId)
        {
            var testScheduler = new TestScheduler();

            var senderPeerIdentifier = PeerIdHelper.GetPeerId("sender");
            var getPeerInfoRequest = new GetPeerInfoRequest {PeerId = peerId.ToString()};

            var protocolMessage =
                getPeerInfoRequest.ToProtocolMessage(senderPeerIdentifier);

            var messageStream =
                MessageStreamHelper.CreateStreamWithMessage(_fakeContext, testScheduler, protocolMessage);

            var peerSettings = senderPeerIdentifier.ToSubstitutedPeerSettings();
            var handler = new GetPeerInfoRequestObserver(peerSettings, _logger, _peerRepository, _peerClient);

            handler.StartObserving(messageStream);

            testScheduler.Start();

            var receivedCalls = _peerClient.ReceivedCalls().ToList();
            receivedCalls.Count.Should().Be(1);

            var sentResponseDto = (IMessageDto<ProtocolMessage>) receivedCalls[0].GetArguments().Single();

            return sentResponseDto.Content.FromProtocolMessage<GetPeerInfoResponse>();
        }
    }
}
