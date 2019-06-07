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

using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Catalyst.Common.Interfaces.IO;
using Catalyst.Common.IO.Outbound;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Messaging.Handlers;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace Catalyst.Node.Core.P2P
{
    public sealed class PeerClient : UdpClient, IPeerClient
    {
        public PeerClient(IUdpChannelFactory channelFactory, IPeerIdentifier peerIdentifier) 
            : this(channelFactory, peerIdentifier.IpEndPoint) { }
        
        public PeerClient(IUdpChannelFactory channelFactory, IPEndPoint ipEndPoint)
            : base(channelFactory, Log.Logger.ForContext(MethodBase.GetCurrentMethod().DeclaringType))
        {
            Bootstrap(new OutboundChannelInitializerBase<IChannel>(
                new List<IChannelHandler>
                {
                    new ProtoDatagramHandler()
                },
                ipEndPoint.Address
            ), ipEndPoint);
        }

        public Task SendMessageAsync(IByteBufferHolder datagramPacket)
        {
            return Channel.WriteAndFlushAsync(datagramPacket);
        }
    }
}
