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

using Catalyst.Common.Interfaces.IO;
using Dawn;
using DotNetty.Transport.Channels;

namespace Catalyst.Common.IO
{
    public class HandlerWorkerEventLoopGroupFactory : IHandlerWorkerEventLoopGroupFactory
    {
        private readonly int _tcpClientThreads;
        private readonly int _tcpServerThreads;
        private readonly int _udpServerThreads;
        private readonly int _udpClientThreads;

        public HandlerWorkerEventLoopGroupFactory(int tcpClientThreads = 0,
            int tcpServerThreads = 0,
            int udpServerThreads = 0,
            int udpClientThreads = 0)
        {
            _tcpClientThreads = tcpClientThreads;
            _tcpServerThreads = tcpServerThreads;
            _udpClientThreads = udpClientThreads;
            _udpServerThreads = udpServerThreads;
        }

        public MultithreadEventLoopGroup NewTcpClientLoopGroup()
        {
            Guard.Argument(_tcpClientThreads).Positive();
            return new MultithreadEventLoopGroup(_tcpClientThreads);
        }

        public MultithreadEventLoopGroup NewTcpServerLoopGroup()
        {
            Guard.Argument(_tcpServerThreads).Positive();
            return new MultithreadEventLoopGroup(_tcpServerThreads);
        }

        public MultithreadEventLoopGroup NewUdpServerLoopGroup()
        {
            Guard.Argument(_udpServerThreads).Positive();
            return new MultithreadEventLoopGroup(_udpServerThreads);
        }

        public MultithreadEventLoopGroup NewUdpClientLoopGroup()
        {
            Guard.Argument(_udpClientThreads).Positive();
            return new MultithreadEventLoopGroup(_udpClientThreads);
        }
    }
}
