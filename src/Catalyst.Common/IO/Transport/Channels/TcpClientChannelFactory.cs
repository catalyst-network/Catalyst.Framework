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
using System.Security.Cryptography.X509Certificates;
using Catalyst.Common.Interfaces.IO.EventLoop;
using Catalyst.Common.Interfaces.IO.Transport.Channels;
using Catalyst.Common.IO.Transport.Bootstrapping;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Catalyst.Common.IO.Transport.Channels
{
    public abstract class TcpClientChannelFactory : ITcpClientChannelFactory
    {
        private readonly int _backLogValue;

        protected abstract List<IChannelHandler> Handlers { get; }
        protected TcpClientChannelFactory(int backLogValue = 100) { _backLogValue = backLogValue; }
        
        public abstract IObservableChannel BuildChannel(IEventLoopGroupFactory eventLoopGroupFactory,
            IPAddress targetAddress,
            int targetPort,
            X509Certificate2 certificate = null);

        protected IChannel Bootstrap(IEventLoopGroupFactory handlerEventLoopGroupFactory,
            IPAddress targetAddress, 
            int targetPort,
            X509Certificate2 certificate = null)
        {
            var channelHandler = new ClientChannelInitializerBase<ISocketChannel>(Handlers,
                handlerEventLoopGroupFactory,
                targetAddress,
                certificate);

            return new Bootstrap()
               .Group(handlerEventLoopGroupFactory.GetOrCreateSocketIoEventLoopGroup())
               .ChannelFactory(() => new TcpSocketChannel())
               .Option(ChannelOption.SoBacklog, _backLogValue)
               .Handler(new LoggingHandler(LogLevel.DEBUG))
               .Handler(channelHandler)
               .ConnectAsync(targetAddress, targetPort)
               .GetAwaiter()
               .GetResult();
        }
    }
}
