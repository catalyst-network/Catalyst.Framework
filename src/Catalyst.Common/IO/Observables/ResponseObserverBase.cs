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
using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Catalyst.Common.Config;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging.Dto;
using Catalyst.Common.Interfaces.IO.Observables;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.Interfaces.P2P.Messaging.Dto;
using Catalyst.Common.Util;
using Catalyst.Protocol.Common;
using Google.Protobuf;
using Serilog;

namespace Catalyst.Common.IO.Observables
{
    public abstract class ResponseObserverBase<TProto> : ObserverBase, IResponseObserver where TProto : IMessage
    {
        public IPeerIdentifier PeerIdentifier { get; }

        protected ResponseObserverBase(ILogger logger) : base(logger) { }
        
        public abstract void HandleResponse(IProtocolMessageDto<ProtocolMessage> messageDto);

        public override void StartObserving(IObservable<IProtocolMessageDto<ProtocolMessage>> messageStream)
        {
            if (MessageSubscription != null)
            {
                throw new ReadOnlyException($"{GetType()} is already listening to a message stream");
            }

            var filterMessageType = typeof(TProto).ShortenedProtoFullName() ?? throw new ArgumentNullException(nameof(messageStream));
            
            MessageSubscription = messageStream
               .Where(m => m.Payload?.TypeUrl != null 
                 && m.Payload?.TypeUrl == filterMessageType 
                 && !m.Equals(NullObjects.ProtocolMessageDto) 
                 && (bool) m.Payload?.TypeUrl.EndsWith(MessageTypes.Response.Name)
                )
               .SubscribeOn(TaskPoolScheduler.Default)
               .Subscribe(OnNext, OnError, OnCompleted);
        }
        
        public override void OnNext(IProtocolMessageDto<ProtocolMessage> messageDto)
        {
            Logger.Debug("Pre Handle Message Called");
            ChannelHandlerContext = messageDto.Context;
            HandleResponse(messageDto);
        }
    }
}
