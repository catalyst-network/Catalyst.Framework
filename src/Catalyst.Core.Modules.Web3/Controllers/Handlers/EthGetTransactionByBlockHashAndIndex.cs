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

using Catalyst.Abstractions.Kvm.Models;
using Catalyst.Abstractions.Ledger;
using Catalyst.Core.Lib.Extensions;
using Lib.P2P;
using Nethermind.Core.Crypto;

namespace Catalyst.Core.Modules.Web3.Controllers.Handlers 
{
    [EthWeb3RequestHandler("eth", "getTransactionByBlockHashAndIndex")]
    public class EthGetTransactionByBlockHashAndIndex : EthWeb3RequestHandler<Keccak, int, TransactionForRpc>
    {
        protected override TransactionForRpc Handle(Keccak deltaHash, int positionIndex, IWeb3EthApi api)
        {
            var delta = api.GetDeltaWithCid(deltaHash.ToCid());

            return api.ToTransactionForRpc(delta, positionIndex);
        }
    }
}
