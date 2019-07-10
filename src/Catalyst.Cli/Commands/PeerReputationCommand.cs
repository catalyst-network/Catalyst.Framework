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

using Catalyst.Cli.CommandTypes;
using Catalyst.Cli.Options;
using Catalyst.Common.Interfaces.Cli.Commands;
using Catalyst.Common.Network;
using Catalyst.Common.Util;
using Catalyst.Protocol.Rpc.Node;
using Google.Protobuf;
using Nethereum.RLP;
using System.Net;

namespace Catalyst.Cli.Commands
{
    public sealed class PeerReputationCommand : BaseMessageCommand<GetPeerReputationRequest, PeerReputationOptions>
    {
        public PeerReputationCommand(ICommandContext commandContext) : base(commandContext) { }

        protected override GetPeerReputationRequest GetMessage(PeerReputationOptions option)
        {
            return new GetPeerReputationRequest
            {
                PublicKey = option.PublicKey.ToBytesForRLPEncoding().ToByteString(),
                Ip = ByteString.CopyFrom(IPAddress.Parse(option.IpAddress).To16Bytes())
            };
        }
    }
}
