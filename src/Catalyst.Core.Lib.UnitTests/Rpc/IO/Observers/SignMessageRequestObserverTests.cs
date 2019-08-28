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

using System.Linq;
using System.Threading.Tasks;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging.Dto;
using Catalyst.Common.Interfaces.Modules.KeySigner;
using Catalyst.Common.IO.Messaging.Dto;
using Catalyst.Common.Util;
using Catalyst.Core.Lib.Rpc.IO.Observers;
using Catalyst.Cryptography.BulletProofs.Wrapper;
using Catalyst.Cryptography.BulletProofs.Wrapper.Interfaces;
using Catalyst.Protocol;
using Catalyst.Protocol.Common;
using Catalyst.Protocol.Rpc.Node;
using Catalyst.TestUtils;
using DotNetty.Transport.Channels;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;
using Xunit;

namespace Catalyst.Core.Lib.UnitTests.Rpc.IO.Observers
{
    public sealed class SignMessageRequestObserverTests
    {
        private readonly ILogger _logger;
        private readonly IKeySigner _keySigner;
        private readonly IChannelHandlerContext _fakeContext;
        private readonly ISignature _signature;

        public SignMessageRequestObserverTests()
        {
            _keySigner = Substitute.For<IKeySigner>();
            _signature = Substitute.For<ISignature>();
            _signature.SignatureBytes.Returns(ByteUtil.GenerateRandomByteArray(FFI.SignatureLength));
            _signature.PublicKeyBytes.Returns(ByteUtil.GenerateRandomByteArray(FFI.PublicKeyLength));
            _logger = Substitute.For<ILogger>();
            _fakeContext = Substitute.For<IChannelHandlerContext>();
            var fakeChannel = Substitute.For<IChannel>();
            _fakeContext.Channel.Returns(fakeChannel);

            _keySigner.Sign(default, default).ReturnsForAnyArgs(_signature);
        }

        [Theory]
        [InlineData("Hello Catalyst")]
        [InlineData("")]
        [InlineData("Hello&?!1253Catalyst")]
        public async Task SignMessageRequestObserver_Can_Return_SignMessageResponse(string message)
        {
            var testScheduler = new TestScheduler();
            var messageFactory = new DtoFactory();

            var request = messageFactory.GetDto(
                new SignMessageRequest
                {
                    Message = message.ToUtf8ByteString(),
                    SigningContext = new SigningContext()
                },
                PeerIdentifierHelper.GetPeerIdentifier("sender_key"),
                PeerIdentifierHelper.GetPeerIdentifier("recipient_key")
            );
            
            var messageStream = MessageStreamHelper.CreateStreamWithMessage(_fakeContext, testScheduler, request.Content.ToProtocolMessage(PeerIdentifierHelper.GetPeerIdentifier("sender").PeerId));
            var handler = new SignMessageRequestObserver(PeerIdentifierHelper.GetPeerIdentifier("sender"), _logger, _keySigner);
            handler.StartObserving(messageStream);

            testScheduler.Start();
            //await messageStream.WaitForEndOfDelayedStreamOnTaskPoolSchedulerAsync();

            var receivedCalls = _fakeContext.Channel.ReceivedCalls().ToList();
            receivedCalls.Count.Should().Be(1);
            
            var sentResponseDto = (IMessageDto<ProtocolMessage>) receivedCalls.Single().GetArguments().Single();
            var signResponseMessage = sentResponseDto.FromIMessageDto().FromProtocolMessage<SignMessageResponse>();

            signResponseMessage.OriginalMessage.Should().Equal(message);
            signResponseMessage.Signature.ToByteArray().Should().Equal(_signature.SignatureBytes);
            signResponseMessage.PublicKey.ToByteArray().Should().Equal(_signature.PublicKeyBytes);
        }
    }
}
