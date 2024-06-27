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

using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using Catalyst.Abstractions.Hashing;
using Catalyst.Abstractions.Kvm.Models;
using Catalyst.Abstractions.Ledger;
using Catalyst.Abstractions.Repository;
using Catalyst.Core.Lib.Extensions;
using Lib.P2P;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Dirichlet.Numerics;
using Address = Nethermind.Core.Address;

namespace Catalyst.Core.Modules.Web3.Controllers.Handlers
{
    [EthWeb3RequestHandler("eth", "getBlockByHash")]
    public class EthGetBlockByHashHandler : EthWeb3RequestHandler<Keccak, bool, BlockForRpc>
    {
        protected override BlockForRpc Handle(Keccak deltaHash, bool includeFullTxs, IWeb3EthApi api)
        {
            DeltaWithCid deltaWithCid = api.GetDeltaWithCid(deltaHash.ToCid());
            return BuildBlock(api, deltaWithCid, deltaWithCid.Delta.DeltaNumber, api.HashProvider, includeFullTxs);
        }

        private static BlockForRpc BuildBlock(IWeb3EthApi api, DeltaWithCid deltaWithCid, long blockNumber, IHashProvider hashProvider, bool includeFullTxs)
        {
            var (delta, deltaHash) = deltaWithCid;

            Address author;
            var firstCoinBaseEntry = delta.CoinbaseEntries.FirstOrDefault();
            if (firstCoinBaseEntry != null)
            {
                author = new Address(firstCoinBaseEntry.ReceiverKvmAddress.ToByteArray());
            }
            else
            {
                author = Address.Zero;
            }

            var nonce = new byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(nonce, 42);
            BlockForRpc blockForRpc = new BlockForRpc
            {
                ExtraData = new byte[0],
                Miner = author,
                Difficulty = 1,
                Hash = deltaHash,
                Number = blockNumber,
                GasLimit = (long)delta.GasLimit,
                GasUsed = delta.GasUsed,
                Timestamp = new UInt256(delta.TimeStamp.Seconds),
                ParentHash = blockNumber == 0 ? null : Cid.Read(delta.PreviousDeltaDfsHash.ToByteArray()),
                StateRoot = delta.StateRoot.ToKeccak(),
                ReceiptsRoot = Keccak.EmptyTreeHash,
                TransactionsRoot = Keccak.EmptyTreeHash,
                LogsBloom = Bloom.Empty,
                MixHash = Keccak.Zero,
                Nonce = nonce,
                Uncles = new Keccak[0],
                Transactions = includeFullTxs ? (IEnumerable<object>)api.ToTransactionsForRpc(deltaWithCid) : delta.PublicEntries.Select(x => x.GetHash(hashProvider))
            };
            blockForRpc.TotalDifficulty = (UInt256)((long)blockForRpc.Difficulty * (blockNumber + 1));
            blockForRpc.Sha3Uncles = Keccak.OfAnEmptySequenceRlp;
            return blockForRpc;
        }
    }
}
