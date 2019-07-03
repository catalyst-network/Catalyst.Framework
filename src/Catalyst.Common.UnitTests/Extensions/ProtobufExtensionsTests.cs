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
using System.Text;
using Catalyst.Common.Extensions;
using Catalyst.Common.Util;
using Catalyst.Common.IO.Messaging;
using Catalyst.Protocol.Common;
using Catalyst.Protocol.IPPN;
using Catalyst.Protocol.Rpc.Node;
using Catalyst.Protocol.Transaction;
using Catalyst.TestUtils;
using FluentAssertions;
using Multiformats.Hash;
using Xunit;

namespace Catalyst.Common.UnitTests.Extensions
{
    public class ProtobufExtensionsTests
    {
        [Fact]
        public static void ShortenedFullName_should_remove_namespace_start()
        {
            TransactionBroadcast.Descriptor.FullName.Should().Be("Catalyst.Protocol.Transaction.TransactionBroadcast");
            TransactionBroadcast.Descriptor.ShortenedFullName().Should().Be("Transaction.TransactionBroadcast");
        }

        [Fact]
        public static void ShortenedProtoFullName_should_remove_namespace_start()
        {
            PingRequest.Descriptor.FullName.Should().Be("Catalyst.Protocol.IPPN.PingRequest");
            typeof(PingRequest).ShortenedProtoFullName().Should().Be("IPPN.PingRequest");
        }

        [Theory]
        [InlineData("MyFunnyRequest", "MyFunnyResponse")]
        [InlineData("Request", "Response")]
        [InlineData("Some.Namespace.ClassRequest", "Some.Namespace.ClassResponse")]
        public static void GetResponseType_should_swap_request_suffix_for_response_suffix(string requestType, string responseType)
        {
            requestType.GetResponseType().Should().Be(responseType);
        }

        [Theory]
        [InlineData("MyFunnyResponse", "MyFunnyRequest")]
        [InlineData("Response", "Request")]
        [InlineData("Some.Namespace.ClassResponse", "Some.Namespace.ClassRequest")]
        public static void GetRequestType_should_swap_request_suffix_for_response_suffix(string responseType, string requestType)
        {
            responseType.GetRequestType().Should().Be(requestType);
        }

        [Fact]
        public static void ToAnySigned_should_happen_new_guid_to_request_if_not_specified()
        {
            //this ensures we won't get Guid.Empty and then a risk of mismatch;
            var wrapped = new PingRequest().ToProtocolMessage(PeerIdHelper.GetPeerId("you"));
            wrapped.CorrelationId.Should().NotBeEquivalentTo(Guid.Empty.ToByteString());
        }

        [Fact]
        public static void ToAnySigned_should_set_the_wrapper_fields()
        {
            var guid = CorrelationId.GenerateCorrelationId();
            var peerId = PeerIdHelper.GetPeerId("blablabla");
            var expectedContent = "content";
            var wrapped = new PeerId()
            {
                ClientId = expectedContent.ToUtf8ByteString()
            }.ToProtocolMessage(peerId, guid);

            wrapped.CorrelationId.ToCorrelationId().Id.Should().Be(guid.Id);
            wrapped.PeerId.Should().Be(peerId);
            wrapped.TypeUrl.Should().Be(PeerId.Descriptor.ShortenedFullName());
            wrapped.FromProtocolMessage<PeerId>().ClientId.Should().Equal(expectedContent.ToUtf8ByteString());
        }

        [Fact]
        public static void ToProtocolMessage_When_Processing_Request_Should_Generate_New_CorrelationId_If_Not_Specified()
        {
            var peerId = PeerIdHelper.GetPeerId("someone");
            var request = new GetPeerCountRequest().ToProtocolMessage(peerId);
            request.CorrelationId.ToCorrelationId().Should().NotBe(default);
        }

        [Fact]
        public static void ToProtocolMessage_When_Processing_Response_Should_Fail_If_No_CorrelationId_Specified()
        {
            var peerId = PeerIdHelper.GetPeerId("someone");
            var response = new GetPeerCountResponse
            {
                PeerCount = 13
            };
            new Action(() => response.ToProtocolMessage(peerId))
               .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Can_Recognize_Gossip_Message()
        {
            var peerIdentifier = PeerIdentifierHelper.GetPeerIdentifier("1");
            var gossipMessage = new TransactionBroadcast().ToProtocolMessage(peerIdentifier.PeerId, CorrelationId.GenerateCorrelationId())
               .ToProtocolMessage(peerIdentifier.PeerId, CorrelationId.GenerateCorrelationId());
            gossipMessage.CheckIfMessageIsBroadcast().Should().BeTrue();

            var nonGossipMessage = new PingRequest().ToProtocolMessage(peerIdentifier.PeerId, CorrelationId.GenerateCorrelationId());
            nonGossipMessage.CheckIfMessageIsBroadcast().Should().BeFalse();

            var secondNonGossipMessage = new PingRequest().ToProtocolMessage(peerIdentifier.PeerId, CorrelationId.GenerateCorrelationId())
               .ToProtocolMessage(peerIdentifier.PeerId, CorrelationId.GenerateCorrelationId());
            secondNonGossipMessage.CheckIfMessageIsBroadcast().Should().BeFalse();
        }

        [Fact]
        public void ToMultihash_Can_Convert_Valid_ByteString_To_Multihash()
        {
            var initialHash = Multihash.Sum(HashType.BLAKE2B_256, Encoding.UTF8.GetBytes("hello"));
            var byteString = initialHash.ToBytes().ToByteString();

            var convertedHash = byteString.ToMultihash();

            convertedHash.Should().Be(initialHash);
        }

        [Fact]
        public void ToMultihashString_Can_Convert_Valid_ByteString_To_String()
        {
            var initialHash = Multihash.Encode("hello", HashType.BLAKE2B_256);
            var byteString = initialHash.ToBytes().ToByteString();

            var multihash = byteString.ToMultihashString();
            multihash.Should().NotBe(null);
        }
    }
}
