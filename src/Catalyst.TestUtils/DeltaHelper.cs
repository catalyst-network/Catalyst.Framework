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
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.Util;
using Catalyst.Protocol.Wire;
using Catalyst.Protocol.Deltas;
using Catalyst.Protocol.Peer;
using Google.Protobuf.WellKnownTypes;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;
using CandidateDeltaBroadcast = Catalyst.Protocol.Wire.CandidateDeltaBroadcast;

namespace Catalyst.TestUtils
{
    public static class DeltaHelper
    {
        public static Delta GetDelta(byte[] previousDeltaHash = default, 
            byte[] merkleRoot = default,
            byte[] merklePoda = default,
            IMultihashAlgorithm hashAlgorithm = null,
            DateTime? timestamp = default)
        {
            var previousHash = previousDeltaHash ?? ByteUtil.GenerateRandomByteArray(32)
               .ComputeMultihash(hashAlgorithm ?? new BLAKE2B_256());
            return GetDelta(previousHash, merkleRoot, merklePoda, timestamp);
        }

        public static Delta GetDelta(Multihash previousDeltaHash,
            byte[] merkleRoot = default,
            byte[] merklePoda = default,
            DateTime? timestamp = default)
        {
            var previousHash = previousDeltaHash;
            var root = merkleRoot ?? ByteUtil.GenerateRandomByteArray(32);
            var poda = merklePoda ?? ByteUtil.GenerateRandomByteArray(32);
            var nonNullTimestamp = Timestamp.FromDateTime(timestamp?.ToUniversalTime() ?? DateTime.Now.ToUniversalTime());

            var delta = new Delta
            {
                PreviousDeltaDfsHash = previousHash.ToBytes().ToByteString(),
                MerkleRoot = root.ToByteString(),
                MerklePoda = poda.ToByteString(),
                TimeStamp = nonNullTimestamp
            };

            return delta;
        }

        public static CandidateDeltaBroadcast GetCandidateDelta(byte[] previousDeltaHash = null, 
            byte[] hash = null,
            PeerId producerId = null,
            IMultihashAlgorithm hashAlgorithm = null)
        {
            var candidateHash = hash ?? ByteUtil.GenerateRandomByteArray(32).ComputeMultihash(hashAlgorithm ?? new BLAKE2B_256());
            var previousHash = previousDeltaHash ?? ByteUtil.GenerateRandomByteArray(32).ComputeMultihash(hashAlgorithm ?? new BLAKE2B_256());
            var producer = producerId 
             ?? PeerIdHelper.GetPeerId(publicKey: ByteUtil.GenerateRandomByteArray(32));

            return new CandidateDeltaBroadcast
            {
                Hash = candidateHash.ToByteString(),
                PreviousDeltaDfsHash = previousHash.ToByteString(),
                ProducerId = producer
            };
        }

        public static FavouriteDeltaBroadcast GetFavouriteDelta(byte[] previousDeltaHash = null,
            byte[] hash = null,
            IMultihashAlgorithm hashAlgorithm = null,
            PeerId producerId = null,
            PeerId voterId = null)
        {
            var candidate = GetCandidateDelta(previousDeltaHash, hash, producerId, hashAlgorithm);
            var voter = voterId ?? PeerIdHelper.GetPeerId();

            return new FavouriteDeltaBroadcast
            {
                Candidate = candidate,
                VoterId = voter
            };
        }
    }
}
