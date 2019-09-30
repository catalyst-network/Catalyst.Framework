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

using Catalyst.Core.Lib.DAO;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.Util;
using Catalyst.Protocol.Transaction;

namespace Catalyst.TestUtils.ProtocolHelpers
{
    public static class RangeProofHelper
    {
        public static RangeProof GetRangeProof()
        {
            return new RangeProof
            {
                ValueCommitment =
                {
                    ByteUtil.GenerateRandomByteArray(16).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(16).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(16).ToByteString()
                },
                AggregatedVectorPolynomialL =
                {
                    ByteUtil.GenerateRandomByteArray(8).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(8).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(8).ToByteString()
                },
                AggregatedVectorPolynomialR =
                {
                    ByteUtil.GenerateRandomByteArray(8).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(8).ToByteString(),
                    ByteUtil.GenerateRandomByteArray(8).ToByteString()
                },
                ProofOfShareMu = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                BitCommitment = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                PerBitBlindingFactorCommitment = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                PolyCommitmentT1 = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                PolyCommitmentT2 = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                ProofOfShareTau = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                APrime0 = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                BPrime0 = ByteUtil.GenerateRandomByteArray(32).ToByteString(),
                T = ByteUtil.GenerateRandomByteArray(64).ToByteString()
            };
        }

        public static RangeProofDao GetRangeProofDao()
        {
            return new RangeProofDao().ToDao(GetRangeProof());
        }
    }
}
