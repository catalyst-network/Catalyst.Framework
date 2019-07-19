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

using Catalyst.Common.Interfaces.Modules.KeySigner;
using Catalyst.Cryptography.BulletProofs.Wrapper.Interfaces;
using Catalyst.Cryptography.BulletProofs.Wrapper.Types;
using Catalyst.Protocol;
using Catalyst.Protocol.Common;
using DotNetty.Transport.Channels;
using Google.Protobuf;

namespace Catalyst.Common.IO.Handlers
{
    public sealed class ProtocolMessageVerifyHandler : InboundChannelHandlerBase<ProtocolMessageSigned>
    {
        private readonly IKeySigner _keySigner;

        public ProtocolMessageVerifyHandler(IKeySigner keySigner)
        {
            _keySigner = keySigner;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, ProtocolMessageSigned signedMessage)
        {
            if (Verify(signedMessage))
            {
                if (signedMessage.Message.IsBroadCastMessage())
                {
                    var innerSignedMessage = ProtocolMessageSigned.Parser.ParseFrom(signedMessage.Message.Value);
                    if (!Verify(innerSignedMessage))
                    {
                        return;
                    }
                }

                ctx.FireChannelRead(signedMessage.Message);
            }
        }

        private bool Verify(ProtocolMessageSigned signedMessage)
        {
            var innerSignedMessage = ProtocolMessageSigned.Parser.ParseFrom(signedMessage.Message.Value);
            var innerSignature = MessageToSignature(innerSignedMessage);
            return _keySigner.Verify(innerSignature, innerSignedMessage.Message.ToByteArray());
        }

        private ISignature MessageToSignature(ProtocolMessageSigned signedMessage)
        {
            var sig = signedMessage.Signature.ToByteArray();
            var pub = signedMessage.Message.PeerId.PublicKey.ToByteArray();
            return new Signature(sig, pub);
        }
    }
}
