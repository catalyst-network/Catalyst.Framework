#region LICENSE

/**
* Copyright (c) 2022 Catalyst Network
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
using System.Threading;
using System.Threading.Tasks;
using Catalyst.Abstractions.P2P;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Modules.Network.Dotnetty.Abstractions;
using Catalyst.Modules.Network.Dotnetty.Abstractions.P2P.IO.Messaging.Broadcast;
using Catalyst.Protocol.Wire;
using Google.Protobuf;
using MultiFormats;

namespace Catalyst.Modules.Network.Dotnetty.P2P
{
    public sealed class DotnettyPeerClient : IPeerClient
    {
        private bool _disposed;

        private readonly IPeerSettings _peerSettings;
        private readonly IBroadcastManager _broadcastManager;
        private readonly IDotnettyUdpClient _dotnettyUdpClient;

        /// <param name="clientChannelFactory">A factory used to build the appropriate kind of channel for a udp client.</param>
        /// <param name="eventLoopGroupFactory"></param>
        /// <param name="peerSettings"></param>
        public DotnettyPeerClient(IDotnettyUdpClient dotnettyUdpClient,
            IBroadcastManager broadcastManager,
            IPeerSettings peerSettings)
        {
            _dotnettyUdpClient = dotnettyUdpClient;
            _broadcastManager = broadcastManager;
            _peerSettings = peerSettings;
        }

        public async Task StartAsync()
        {
            await StartAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _dotnettyUdpClient.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SendMessageToPeersAsync(IMessage message, IEnumerable<MultiAddress> peers)
        {
            var protocolMessage = message.ToProtocolMessage(_peerSettings.Address);
            foreach (var peer in peers)
            {
                await SendMessageAsync(protocolMessage, peer).ConfigureAwait(false);
            }
        }

        public async Task BroadcastAsync(ProtocolMessage message)
        {
            await _broadcastManager.BroadcastAsync(message).ConfigureAwait(false);
        }

        public async Task SendMessageAsync(ProtocolMessage message, MultiAddress recipient)
        {
            await _dotnettyUdpClient.SendMessageAsync(message, recipient).ConfigureAwait(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dotnettyUdpClient.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
