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
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Catalyst.Abstractions.Consensus.Deltas;
using Catalyst.Abstractions.Hashing;
using Catalyst.Core.Lib.Extensions;
using Google.Protobuf.WellKnownTypes;
using Nito.Comparers;
using Serilog;

namespace Catalyst.Core.Modules.Consensus.Deltas
{
    /// <inheritdoc />
    public class DeltaHashProvider : IDeltaHashProvider
    {
        private readonly IDeltaCache _deltaCache;
        private readonly IHashProvider _hashProvider;
        private readonly ILogger _logger;

        private readonly ReplaySubject<string> _deltaHashUpdatesSubject;
        private readonly SortedList<Timestamp, string> _hashesByTimeDescending;
        private readonly int _capacity;

        public IObservable<string> DeltaHashUpdates => _deltaHashUpdatesSubject.AsObservable();

        public DeltaHashProvider(IDeltaCache deltaCache, 
            IHashProvider hashProvider,
            ILogger logger,
            int capacity = 10_000)
        {
            _deltaCache = deltaCache;
            _hashProvider = hashProvider;
            _logger = logger;
            _deltaHashUpdatesSubject = new ReplaySubject<string>(0);
            var comparer = ComparerBuilder.For<Timestamp>().OrderBy(u => u, @descending: true);
            _capacity = capacity;
            _hashesByTimeDescending = new SortedList<Timestamp, string>(comparer)
            {
                Capacity = _capacity,
            };

            _hashesByTimeDescending.Add(Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
                _deltaCache.GenesisAddress);
        }

        /// <inheritdoc />
        public bool TryUpdateLatestHash(string previousHash, string newHash)
        {
            var newAddress = newHash;
            var previousAddress = previousHash;
            _logger.Debug("New hash {hash} received for previous hash {previousHash}", 
                newAddress, previousAddress);
            var foundNewDelta = _deltaCache.TryGetOrAddConfirmedDelta(newAddress, out var newDelta);
            var foundPreviousDelta = _deltaCache.TryGetOrAddConfirmedDelta(previousAddress, out var previousDelta);

            if (!foundNewDelta 
             || !foundPreviousDelta
             || newDelta.PreviousDeltaDfsHash != _hashProvider.GetBase32HashBytes(previousHash).ToByteString()
             || previousDelta.TimeStamp >= newDelta.TimeStamp)
            {
                _logger.Warning("Failed to update latest hash from {previousHash} to {newHash}",
                    previousAddress, newAddress);
                return false;
            }

            _logger.Debug("Successfully to updated latest hash from {previousHash} to {newHash}",
                previousAddress, newAddress);

            lock (_hashesByTimeDescending)
            {
                _hashesByTimeDescending.Add(newDelta.TimeStamp, newHash);
                if (_hashesByTimeDescending.Count > _capacity)
                {
                    _hashesByTimeDescending.RemoveAt(_capacity);
                }
            }
            
            _deltaHashUpdatesSubject.OnNext(newAddress);

            return true;
        }

        /// <inheritdoc />
        public string GetLatestDeltaHash(DateTime? asOf = null)
        {
            _logger.Verbose("Trying to retrieve latest delta as of {asOf}", asOf);
            if (!asOf.HasValue)
            {
                return _hashesByTimeDescending.FirstOrDefault().Value;
            }

            var timestamp = Timestamp.FromDateTime(asOf.Value);
            var hash = _hashesByTimeDescending
               .SkipWhile(p => p.Key > timestamp)
               .FirstOrDefault();

            //todo: do we want to start walking down
            //the history of hashes and get them from IPFS
            //if they are not found here?
            //https://github.com/catalyst-network/Catalyst.Node/issues/615
            return hash.Value;
        }
    }
}
