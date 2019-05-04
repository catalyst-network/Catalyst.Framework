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
using Catalyst.Common.Enums.Messages;
using Catalyst.Common.Interfaces.Cli.Options;
using Catalyst.Common.P2P;
using Catalyst.Node.Core.Rpc.Messaging;
using Catalyst.Protocol.Rpc.Node;
using Dawn;

namespace Catalyst.Cli.Commands
{
    public partial class Commands
    {
        /// <inheritdoc cref="GetInfoCommand" />
        public bool GetInfoCommand(IGetInfoOptions opts)
        {
            Guard.Argument(opts).NotNull().Compatible<IGetInfoOptions>();

            var node = GetConnectedNode(opts.NodeId);
            Guard.Argument(node)
               .NotNull(
                    "Node cannot be null. The shell must be able to connect to a valid node to be able to send the request.");

            var nodeConfig = GetNodeConfig(opts.NodeId);
            Guard.Argument(nodeConfig).NotNull("The node configuration cannot be null");

            try
            {
                var request = new RpcMessageFactory<GetInfoRequest>().GetMessage(
                    new GetInfoRequest
                    {
                        Query = true
                    },
                    new PeerIdentifier(Encoding.ASCII.GetBytes(nodeConfig.PublicKey), nodeConfig.HostAddress,
                        nodeConfig.Port),
                    _peerIdentifier,
                    DtoMessageType.Ask);

                node.SendMessage(request);
            }
            catch (Exception e)
            {
                _logger.Debug(e.Message);
                throw;
            }

            return true;
        }
    }
}
