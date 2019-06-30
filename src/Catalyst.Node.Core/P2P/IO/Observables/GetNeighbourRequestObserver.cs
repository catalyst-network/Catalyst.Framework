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
using System.Linq;
using Catalyst.Common.Config;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Observables;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Observables;
using Catalyst.Common.P2P;
using Catalyst.Protocol.IPPN;
using Dawn;
using DotNetty.Transport.Channels;
using Serilog;
using SharpRepository.Repository;
using SharpRepository.Repository.Specifications;

namespace Catalyst.Node.Core.P2P.IO.Observables
{
    public sealed class GetNeighbourRequestObserver
        : RequestObserverBase<PeerNeighborsRequest, PeerNeighborsResponse>,
            IP2PMessageObserver
    {
        private readonly IRepository<Peer> _repository;

        public GetNeighbourRequestObserver(IPeerIdentifier peerIdentifier,
            IRepository<Peer> repository,
            ILogger logger)
            : base(logger, peerIdentifier)
        { 
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerNeighborsRequest"></param>
        /// <param name="channelHandlerContext"></param>
        /// <param name="senderPeerIdentifier"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        protected override PeerNeighborsResponse HandleRequest(PeerNeighborsRequest peerNeighborsRequest, IChannelHandlerContext channelHandlerContext, IPeerIdentifier senderPeerIdentifier, Guid correlationId)
        {
            Guard.Argument(peerNeighborsRequest, nameof(peerNeighborsRequest)).NotNull();
            Guard.Argument(channelHandlerContext, nameof(channelHandlerContext)).NotNull();
            Guard.Argument(senderPeerIdentifier, nameof(senderPeerIdentifier)).NotNull();
            
            Logger.Debug("PeerNeighborsRequest Message Received");

            var activePeersList = _repository.FindAll(new Specification<Peer>(p => !p.IsAwolPeer)).ToList();
            Guard.Argument(activePeersList).MinCount(1);

            var peerNeighborsResponseMessage = new PeerNeighborsResponse();
            
            for (var i = 0; i < Constants.NumberOfRandomPeers; i++)
            {
                peerNeighborsResponseMessage.Peers.Add(activePeersList.RandomElement().PeerIdentifier.PeerId);
            }

            return peerNeighborsResponseMessage;
        }
    }
}
