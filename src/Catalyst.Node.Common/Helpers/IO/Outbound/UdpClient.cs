/*
* Copyright(c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node<https: //github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node.If not, see<https: //www.gnu.org/licenses/>.
*/

using System.Net;
using System.Threading.Tasks;
using Catalyst.Node.Common.Interfaces;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Logging;
using Serilog;
using Bootstrap = Catalyst.Node.Common.Helpers.IO.Bootstrap;

namespace Catalyst.Node.Common.Helpers.IO.Outbound
{
    public sealed class UdpClient : AbstractClient
    {
        public UdpClient(ILogger logger) : base(logger) { }

        public override  ISocketClient Bootstrap(IChannelHandler channelInitializer)
        {
            Client = new Bootstrap();
                ((DotNetty.Transport.Bootstrapping.Bootstrap)Client)
               .Group(WorkerEventLoop)
               .Channel<TcpSocketChannel>()
               .Option(ChannelOption.SoBacklog, 100)
               .Handler(new LoggingHandler(LogLevel.INFO))
               .Handler(channelInitializer);
            return this;
        }
        
        public override async Task<ISocketClient> ConnectClient(IPAddress listenAddress, int port)
        {
            Channel = await Client.ConnectAsync(listenAddress, port).ConfigureAwait(false);
            return this;
        } 
    }
}
