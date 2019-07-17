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
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.Cli;
using Catalyst.Common.IO.Messaging.Correlation;
using Catalyst.Common.IO.Messaging.Dto;
using Catalyst.Common.Network;
using Catalyst.Common.Util;
using Catalyst.Node.Rpc.Client.IO.Observers;
using Catalyst.Protocol;
using Catalyst.Protocol.Rpc.Node;
using Catalyst.TestUtils;
using DotNetty.Transport.Channels;
using FluentAssertions;
using Google.Protobuf;
using Nethereum.RLP;
using NSubstitute;
using Serilog;
using Xunit;

namespace Catalyst.Node.Rpc.Client.UnitTests.IO.Observers
{
    /// <summary>
    /// Tests the CLI for peer blacklisting response
    /// </summary>
    public sealed class GetPeerBlackListingResponseObserverTests : IDisposable
    {
        private readonly IUserOutput _output;
        private readonly IChannelHandlerContext _fakeContext;

        private readonly ILogger _logger;
        private PeerBlackListingResponseObserver _observer;

        /// <summary>
        /// Initializes a new instance of the <see>
        ///     <cref>GetPeerBlackListingResponseObserverTest</cref>
        /// </see>
        /// class. </summary>
        public GetPeerBlackListingResponseObserverTests()
        {
            _logger = Substitute.For<ILogger>();
            _fakeContext = Substitute.For<IChannelHandlerContext>();
            _output = Substitute.For<IUserOutput>();
        }

        /// <summary>
        /// RPCs the client can handle get peer blacklisting response.
        /// </summary>
        /// <param name="blacklist">The black list flag.</param>
        /// <param name="publicKey">The publicKey of the peer whose blacklist flag to change</param>
        /// <param name="ip">The IP Address of the peer whose blacklist flag to change</param>
        [Theory]
        [InlineData("true", "198.51.100.22", "cne2+eRandomValuebeingusedherefprtestingIOp")]
        [InlineData("false", "198.51.100.14", "uebeingusedhere44j6jhdhdhandomValfprtestingItn")]
        public async Task RpcClient_Can_Handle_GetBlackListingResponse(bool blacklist, string ip, string publicKey)
        {
            var setPeerBlackListResponse = await TestGetBlackListResponse(blacklist, ip, publicKey).ConfigureAwait(false);
            setPeerBlackListResponse.Should().NotBeNull();
            setPeerBlackListResponse.Blacklist.Should().Be(blacklist);
            setPeerBlackListResponse.PublicKey.Should().BeEquivalentTo(publicKey.PublicKeyToProtobuf());
            setPeerBlackListResponse.Ip.Should().BeEquivalentTo(ip.IpAddressToProtobuf());
        }

        /// <summary>
        /// RPCs the client can handle get peer blacklisting response non existent peers.
        /// </summary>
        [Fact]
        public async Task RpcClient_Can_Handle_GetBlackListingResponseNonExistentPeers()
        {
            var blacklist = false;
            var ip = string.Empty;
            var publicKey = string.Empty;

            var setPeerBlackListResponse = await TestGetBlackListResponse(blacklist, ip, publicKey).ConfigureAwait(false);
            setPeerBlackListResponse.Should().NotBeNull();
            setPeerBlackListResponse.Blacklist.Should().Be(blacklist);
            setPeerBlackListResponse.PublicKey.Should().BeEquivalentTo(publicKey.ToUtf8ByteString());
            setPeerBlackListResponse.Ip.Should().BeEquivalentTo(ip.ToUtf8ByteString());
        }

        private async Task<SetPeerBlackListResponse> TestGetBlackListResponse(bool blacklist, string ip, string publicKey)
        {
            var response = new DtoFactory().GetDto(new SetPeerBlackListResponse
            {
                Blacklist = blacklist,
                Ip = string.IsNullOrEmpty(ip) ? ip.ToUtf8ByteString() : ip.IpAddressToProtobuf(),
                PublicKey = string.IsNullOrEmpty(publicKey) ? publicKey.ToUtf8ByteString() : publicKey.PublicKeyToProtobuf()
            },
                PeerIdentifierHelper.GetPeerIdentifier("sender"),
                PeerIdentifierHelper.GetPeerIdentifier("recipient"),
                CorrelationId.GenerateCorrelationId());

            var messageStream = MessageStreamHelper.CreateStreamWithMessage(_fakeContext,
                response.Content.ToProtocolMessage(PeerIdentifierHelper.GetPeerIdentifier("sender").PeerId,
                    response.CorrelationId)
            );

            SetPeerBlackListResponse messageStreamResponse = null;

            _observer = new PeerBlackListingResponseObserver(_output, _logger);
            _observer.StartObserving(messageStream);

            _observer.MessageResponseStream.Where(x => x.Message.GetType() == typeof(SetPeerBlackListResponse)).SubscribeOn(NewThreadScheduler.Default).Subscribe((RpcClientMessageDto) =>
            {
                messageStreamResponse = (SetPeerBlackListResponse)RpcClientMessageDto.Message;
            });

            await messageStream.WaitForEndOfDelayedStreamOnTaskPoolSchedulerAsync();

            return messageStreamResponse;
        }

        public void Dispose()
        {
            _observer?.Dispose();
        }
    }
}
