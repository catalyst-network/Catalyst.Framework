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

using System.Net;
using Catalyst.Common.Interfaces.IO;
using Catalyst.Common.Interfaces.IO.Inbound;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using Serilog;

namespace Catalyst.Common.IO.Inbound
{
    public class UdpServer : SocketBase<IChannel>, IUdpServer
    {
        protected UdpServer(IUdpChannelFactory channelFactory, ILogger logger)
            : base(channelFactory, logger) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelInitializer"></param>
        /// <param name="listenAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void Bootstrap(IChannelHandler channelInitializer, IPAddress listenAddress, int port)
        {
            Channel = new Bootstrap()
               .Group(WorkerEventLoop)
               .ChannelFactory(ChannelFactory.BuildChannel)
               .Option(ChannelOption.SoBroadcast, true)
               .Handler(new LoggingHandler(LogLevel.DEBUG))
               .Handler(channelInitializer)
               .BindAsync(listenAddress, port)
               .GetAwaiter()
               .GetResult();
        }
    }
}
