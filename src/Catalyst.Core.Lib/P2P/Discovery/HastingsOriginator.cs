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

using System.Collections.Generic;
using System.Linq;
using Catalyst.Common.Interfaces.IO.Messaging.Correlation;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.Interfaces.P2P.Discovery;
using Catalyst.Common.IO.Messaging.Correlation;

namespace Catalyst.Core.Lib.P2P.Discovery
{
    public sealed class HastingsOriginator : IHastingsOriginator
    {
        private IPeerIdentifier _peer;
        public IList<INeighbour> Neighbours { get; set; }
        public KeyValuePair<ICorrelationId, IPeerIdentifier> ExpectedPnr { get; set; }

        /// <summary>
        ///     if setting a new peer, clean counters
        /// </summary>
        public IPeerIdentifier Peer
        {
            get => _peer;
            set
            {
                if (_peer != null)
                {
                    Neighbours.Clear();
                    ExpectedPnr = new KeyValuePair<ICorrelationId, IPeerIdentifier>();
                }
                
                _peer = value;
            }
        }

        public HastingsOriginator()
        {
            ExpectedPnr = new KeyValuePair<ICorrelationId, IPeerIdentifier>();
            Neighbours = new List<INeighbour>();
        }

        /// <inheritdoc />
        public IHastingMemento CreateMemento()
        {
            return new HastingMemento(Peer, Neighbours);
        }
        
        /// <inheritdoc />
        public void RestoreMemento(IHastingMemento hastingMemento)
        {
            Peer = hastingMemento.Peer;
            
            hastingMemento.Neighbours
               .ToList()
               .ForEach(i =>
                {
                    Neighbours.Add(i);
                });
        }
    }
}
