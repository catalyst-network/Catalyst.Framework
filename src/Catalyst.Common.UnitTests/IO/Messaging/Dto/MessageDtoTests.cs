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

using Catalyst.Common.Interfaces.IO.Messaging.Dto;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Messaging.Dto;
using Catalyst.Protocol.IPPN;
using Catalyst.TestUtils;
using FluentAssertions;
using Google.Protobuf;
using NSubstitute;
using Xunit;

namespace Catalyst.Common.UnitTests.IO.Messaging.Dto
{
    public sealed class MessageDtoTests
    {
        private readonly IOutboundDto _outboundDto;

        public MessageDtoTests()
        {
            var pingRequest = Substitute.For<IMessage<PingRequest>>();
            _outboundDto = new OutboundDto(pingRequest, PeerIdentifierHelper.GetPeerIdentifier("Sender_Key"), PeerIdentifierHelper.GetPeerIdentifier("Recipient_Key"));
        }

        [Fact]
        public void CanInitMessageDtoCorrectly()
        {
            Assert.NotNull(_outboundDto);

            _outboundDto.Should().BeOfType<OutboundDto>();
            _outboundDto.Message.Should().NotBeNull().And.BeAssignableTo(typeof(IMessage<PingRequest>));
            _outboundDto.Recipient.Should().NotBeNull().And.BeAssignableTo(typeof(IPeerIdentifier));
            _outboundDto.Sender.Should().NotBeNull().And.BeAssignableTo(typeof(IPeerIdentifier));
        }
    }
}
