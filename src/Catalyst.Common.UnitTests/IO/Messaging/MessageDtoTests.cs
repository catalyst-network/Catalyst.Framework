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

using Catalyst.Common.Config;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Messaging;
using Catalyst.Common.UnitTests.TestUtils;
using Catalyst.Protocol.IPPN;
using FluentAssertions;
using Google.Protobuf;
using NSubstitute;
using Xunit;

namespace Catalyst.Common.UnitTests.IO.Messaging
{
    public sealed class MessageDtoTests
    {
        private readonly MessageDto<IMessage, P2PMessages> _messageDto;

        public MessageDtoTests()
        {
            var peerIdentifier = Substitute.For<IPeerIdentifier>();
            var pingRequest = Substitute.For<IMessage<PingRequest>>();
            _messageDto = new MessageDto<IMessage, P2PMessages>(
                P2PMessages.PingRequest,
                pingRequest,
                PeerIdentifierHelper.GetPeerIdentifier("im_a_recipient"), 
                peerIdentifier
            );
        }

        [Fact]
        public void CanInitMessageDtoCorrectly()
        {
            Assert.NotNull(_messageDto);
            AssertionExtensions.Should((object) _messageDto).BeOfType(typeof(MessageDto<IMessage, P2PMessages>));
            AssertionExtensions.Should((object) _messageDto.Type).BeEquivalentTo(P2PMessages.PingRequest);
            AssertionExtensions.Should((object) _messageDto.Message).NotBeNull().And.BeAssignableTo(typeof(IMessage<PingRequest>));
            AssertionExtensions.Should((object) _messageDto.Recipient).NotBeNull().And.BeAssignableTo(typeof(IPeerIdentifier));
            AssertionExtensions.Should((object) _messageDto.Sender).NotBeNull().And.BeAssignableTo(typeof(IPeerIdentifier));
        }
    }
}
