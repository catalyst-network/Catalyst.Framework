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

using System.Threading;
using Nethermind.Core;
using Nethermind.Core.Specs;
using Nethermind.Int256;
using Nethermind.Specs.Forks;

namespace Catalyst.Core.Modules.Kvm
{
    /// <inheritdoc />
    public sealed class CatalystGenesisSpec : IReleaseSpec
    {
        private static CatalystGenesisSpec _instance;

        public static CatalystGenesisSpec Instance
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref _instance, () => new CatalystGenesisSpec());
                return _instance;
            }
        }

        private CatalystGenesisSpec() { }

        /// <inheritdoc />
        public long MaximumExtraDataSize { get; } = Istanbul.Instance.MaximumExtraDataSize;

        /// <inheritdoc />
        public long MaxCodeSize { get; } = Istanbul.Instance.MaxCodeSize;

        /// <inheritdoc />
        public long MinGasLimit { get; } = Istanbul.Instance.MinGasLimit;

        /// <inheritdoc />
        public long GasLimitBoundDivisor { get; } = Istanbul.Instance.GasLimitBoundDivisor;

        /// <inheritdoc />
        public UInt256 BlockReward { get; } = Istanbul.Instance.BlockReward;

        /// <inheritdoc />
        public long DifficultyBombDelay { get; } = Istanbul.Instance.DifficultyBombDelay;

        /// <inheritdoc />
        public long DifficultyBoundDivisor { get; } = Istanbul.Instance.DifficultyBoundDivisor;

        /// <inheritdoc />
        public int MaximumUncleCount { get; } = Istanbul.Instance.MaximumUncleCount;

        /// <inheritdoc />
        public bool IsTimeAdjustmentPostOlympic { get; } = Istanbul.Instance.IsTimeAdjustmentPostOlympic;

        /// <inheritdoc />
        public bool IsEip2Enabled { get; } = Istanbul.Instance.IsEip2Enabled;

        /// <inheritdoc />
        public bool IsEip7Enabled { get; } = Istanbul.Instance.IsEip7Enabled;

        /// <inheritdoc />
        public bool IsEip100Enabled { get; } = Istanbul.Instance.IsEip100Enabled;

        /// <inheritdoc />
        public bool IsEip140Enabled { get; } = Istanbul.Instance.IsEip140Enabled;

        /// <inheritdoc />
        public bool IsEip150Enabled { get; } = Istanbul.Instance.IsEip150Enabled;

        /// <inheritdoc />
        public bool IsEip155Enabled { get; } = Istanbul.Instance.IsEip155Enabled;

        /// <inheritdoc />
        public bool IsEip158Enabled { get; } = Istanbul.Instance.IsEip158Enabled;

        /// <inheritdoc />
        public bool IsEip160Enabled { get; } = Istanbul.Instance.IsEip160Enabled;

        /// <inheritdoc />
        public bool IsEip170Enabled { get; } = Istanbul.Instance.IsEip170Enabled;

        /// <inheritdoc />
        public bool IsEip196Enabled { get; } = Istanbul.Instance.IsEip196Enabled;

        /// <inheritdoc />
        public bool IsEip197Enabled { get; } = Istanbul.Instance.IsEip197Enabled;

        /// <inheritdoc />
        public bool IsEip198Enabled { get; } = Istanbul.Instance.IsEip198Enabled;

        /// <inheritdoc />
        public bool IsEip211Enabled { get; } = Istanbul.Instance.IsEip211Enabled;

        /// <inheritdoc />
        public bool IsEip214Enabled { get; } = Istanbul.Instance.IsEip214Enabled;

        /// <inheritdoc />
        public bool IsEip649Enabled { get; } = Istanbul.Instance.IsEip649Enabled;

        /// <inheritdoc />
        public bool IsEip658Enabled { get; } = Istanbul.Instance.IsEip658Enabled;

        /// <inheritdoc />
        public bool IsEip145Enabled { get; } = Istanbul.Instance.IsEip145Enabled;

        /// <inheritdoc />
        public bool IsEip1014Enabled { get; } = Istanbul.Instance.IsEip1014Enabled;

        /// <inheritdoc />
        public bool IsEip1052Enabled { get; } = Istanbul.Instance.IsEip1052Enabled;

        /// <inheritdoc />
        public bool IsEip1283Enabled { get; } = Istanbul.Instance.IsEip1283Enabled;

        /// <inheritdoc />
        public bool IsEip1234Enabled { get; } = Istanbul.Instance.IsEip1234Enabled;

        /// <inheritdoc />
        public bool IsEip1344Enabled { get; } = Istanbul.Instance.IsEip1344Enabled;

        /// <inheritdoc />
        public bool IsEip2028Enabled { get; } = Istanbul.Instance.IsEip2028Enabled;

        /// <inheritdoc />
        public bool IsEip152Enabled { get; } = Istanbul.Instance.IsEip152Enabled;

        /// <inheritdoc />
        public bool IsEip1108Enabled { get; } = Istanbul.Instance.IsEip1108Enabled;

        /// <inheritdoc />
        public bool IsEip1884Enabled { get; } = Istanbul.Instance.IsEip1884Enabled;

        /// <inheritdoc />
        public bool IsEip2200Enabled { get; } = Istanbul.Instance.IsEip2200Enabled;

        public bool IsEip2537Enabled { get; } = Istanbul.Instance.IsEip2537Enabled;

        public string Name => Istanbul.Instance.Name;

        public long? FixedDifficulty => Istanbul.Instance.FixedDifficulty;

        public bool IsEip2565Enabled => Istanbul.Instance.IsEip2565Enabled;

        public bool IsEip2929Enabled => Istanbul.Instance.IsEip2929Enabled;

        public bool IsEip2930Enabled => Istanbul.Instance.IsEip2930Enabled;

        public bool IsEip3198Enabled => Istanbul.Instance.IsEip3198Enabled;

        public bool IsEip3529Enabled => Istanbul.Instance.IsEip3529Enabled;

        public bool IsEip3541Enabled => Istanbul.Instance.IsEip3541Enabled;

        public bool IsEip3607Enabled => Istanbul.Instance.IsEip3607Enabled;

        public bool IsEip3651Enabled => Istanbul.Instance.IsEip3651Enabled;

        public bool IsEip1153Enabled => Istanbul.Instance.IsEip1153Enabled;

        public bool IsEip3855Enabled => Istanbul.Instance.IsEip3855Enabled;

        public bool IsEip5656Enabled => Istanbul.Instance.IsEip5656Enabled;

        public bool IsEip3860Enabled => Istanbul.Instance.IsEip3860Enabled;

        public bool IsEip4895Enabled => Istanbul.Instance.IsEip4895Enabled;

        public bool IsEip4844Enabled => Istanbul.Instance.IsEip4844Enabled;

        public bool IsEip4788Enabled => Istanbul.Instance.IsEip4788Enabled;

        public Address Eip4788ContractAddress => Istanbul.Instance.Eip4788ContractAddress;

        public bool IsEip2935Enabled => Istanbul.Instance.IsEip2935Enabled;

        public bool IsEip7709Enabled => Istanbul.Instance.IsEip7709Enabled;

        public Address Eip2935ContractAddress => Istanbul.Instance.Eip2935ContractAddress;

        public bool IsEip6780Enabled => Istanbul.Instance.IsEip6780Enabled;

        public bool IsRip7212Enabled => Istanbul.Instance.IsRip7212Enabled;

        public ulong WithdrawalTimestamp => Istanbul.Instance.WithdrawalTimestamp;

        public ulong Eip4844TransitionTimestamp => Istanbul.Instance.Eip4844TransitionTimestamp;

        public bool IsEip1559Enabled => Istanbul.Instance.IsEip1559Enabled;

        public long Eip1559TransitionBlock => Istanbul.Instance.Eip1559TransitionBlock;

        public UInt256 ForkBaseFee => Istanbul.Instance.ForkBaseFee;

        public UInt256 BaseFeeMaxChangeDenominator => Istanbul.Instance.BaseFeeMaxChangeDenominator;

        public long ElasticityMultiplier => Istanbul.Instance.ElasticityMultiplier;


        /// <inheritdoc />
        public bool IsEip158IgnoredAccount(Address address) { return address == Address.SystemUser; }
    }
}
