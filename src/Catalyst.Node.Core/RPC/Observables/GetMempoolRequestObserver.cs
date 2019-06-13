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
using System.Collections.Generic;
using System.Text;
using Catalyst.Common.Config;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging;
using Catalyst.Common.Interfaces.IO.Messaging.Dto;
using Catalyst.Common.Interfaces.IO.Observables;
using Catalyst.Common.Interfaces.Modules.Mempool;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Messaging;
using Catalyst.Common.IO.Messaging.Dto;
using Catalyst.Common.IO.Observables;
using Catalyst.Common.P2P;
using Catalyst.Protocol.Common;
using Catalyst.Protocol.Rpc.Node;
using Dawn;
using ILogger = Serilog.ILogger;

namespace Catalyst.Node.Core.RPC.Observables
{
    public sealed class GetMempoolRequestObserver
        : ObserverBase<GetMempoolRequest>,
            IRpcRequestObserver
    {
        private readonly IMempool _mempool;
        private readonly IPeerIdentifier _peerIdentifier;
        private readonly IProtocolMessageFactory _protocolMessageFactory;

        public GetMempoolRequestObserver(IPeerIdentifier peerIdentifier,
            IMempool mempool,
            IProtocolMessageFactory protocolMessageFactory,
            ILogger logger)
            : base(logger)
        {
            _protocolMessageFactory = protocolMessageFactory;
            _mempool = mempool;
            _peerIdentifier = peerIdentifier;
        }

        protected override void Handler(IProtocolMessageDto<ProtocolMessage> messageDto)
        {
            Guard.Argument(messageDto).NotNull();
            
            Logger.Debug("GetMempoolRequestHandler starting ...");
            
            try
            {
                var deserialised = messageDto.Payload.FromProtocolMessage<GetMempoolRequest>();
                
                Guard.Argument(deserialised).NotNull("The shell GetMempoolRequest cannot be null.");
                
                Logger.Debug("Received GetMempoolRequest message with content {0}", deserialised);

                var response = _protocolMessageFactory.GetMessage(new MessageDto(
                        new GetMempoolResponse
                        {
                            Mempool = {GetMempoolContent()}
                        },
                        MessageTypes.Response,
                        new PeerIdentifier(messageDto.Payload.PeerId),
                        _peerIdentifier),
                    messageDto.Payload.CorrelationId.ToGuid());
                
                messageDto.Context.Channel.WriteAndFlushAsync(response).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    "Failed to handle GetInfoRequest after receiving message {0}", messageDto);
                throw;
            }
        }

        private IEnumerable<string> GetMempoolContent()
        {
            var mempoolList = new List<string>();

            try
            {
                var memPoolContentEncoded = _mempool.GetMemPoolContentEncoded();

                foreach (var tx in memPoolContentEncoded)
                {
                    mempoolList.Add(Encoding.Default.GetString(tx));
                }

                return mempoolList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    "Failed to get the mempool content and format it as MapField<string,string> {0}", ex.Message);
                throw;
            }
        }
    }
}
