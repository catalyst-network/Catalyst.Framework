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
using System.Reactive.Linq;
using Catalyst.Modules.Network.Dotnetty.Abstractions.IO.Messaging.Dto;
using Catalyst.Modules.Network.Dotnetty.Abstractions.IO.Transport.Channels;
using Catalyst.Modules.Network.Dotnetty.IO.Transport.Channels;
using Catalyst.Protocol.Wire;
using DotNetty.Transport.Channels;
using NSubstitute;

namespace Catalyst.TestUtils
{
    public static class ObservableHelpers
    {
        public static IRpcObservableChannel MockRpcObservableChannel(IObservable<IObserverDto<ProtocolMessage>> replaySubject)
        {
            var mockChannel = Substitute.For<IChannel>();
            var mockEventStream = replaySubject.AsObservable();
            RpcObservableChannel observableChannel = new(mockEventStream, mockChannel);
            return observableChannel;
        }

        public static IP2PObservableChannel MockP2PObservableChannel(IObservable<ProtocolMessage> replaySubject)
        {
            var mockChannel = Substitute.For<IChannel>();
            var mockEventStream = replaySubject.AsObservable();
            P2PObservableChannel observableChannel = new(mockEventStream, mockChannel);
            return observableChannel;
        }
    }
}
