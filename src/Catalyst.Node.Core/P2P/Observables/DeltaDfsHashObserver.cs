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
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging.Dto;
using Catalyst.Common.Interfaces.IO.Observables;
using Catalyst.Common.Interfaces.Modules.Consensus.Delta;
using Catalyst.Common.IO.Observables;
using Catalyst.Protocol.Common;
using Catalyst.Protocol.Delta;
using Multiformats.Hash;
using Serilog;

namespace Catalyst.Node.Core.P2P.Observables
{
    public class DeltaDfsHashObserver : BroadcastObserverBase<DeltaDfsHashBroadcast>, IP2PMessageObserver
    {
        private readonly IDeltaHashProvider _deltaHashProvider;

        public DeltaDfsHashObserver(IDeltaHashProvider deltaHashProvider, ILogger logger) : base(logger)
        {
            _deltaHashProvider = deltaHashProvider;
        }

        public override void HandleBroadcast(IProtocolMessageDto<ProtocolMessage> messageDto)
        {
            try
            {
                var deserialised = messageDto.Payload.FromProtocolMessage<DeltaDfsHashBroadcast>();
                var previousHash = Multihash.Decode(deserialised.PreviousDeltaDfsHash.ToByteArray());
                var newHash = Multihash.Decode(deserialised.DeltaDfsHash.ToByteArray());
                _deltaHashProvider.TryUpdateLatestHash(previousHash.ToString(), newHash.ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Failed to update latest delta hash from incoming broadcast message.");
            }
        }
    }
}
