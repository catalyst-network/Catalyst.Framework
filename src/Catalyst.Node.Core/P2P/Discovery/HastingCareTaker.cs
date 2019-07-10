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
using System.Collections.Concurrent;
using Catalyst.Common.Interfaces.P2P.Discovery;

namespace Catalyst.Node.Core.P2P.Discovery
{
    public sealed class HastingCareTaker
    {
        public ConcurrentQueue<IHastingMemento> HastingMementoList { get; }

        public HastingCareTaker()
        {
            HastingMementoList = new ConcurrentQueue<IHastingMemento>();
        }

        /// <summary>
        ///     Adds a new state from the walk to the queue
        /// </summary>
        /// <param name="hastingMemento"></param>
        public void Add(IHastingMemento hastingMemento)
        {
            HastingMementoList.Enqueue(hastingMemento);
        }

        /// <summary>
        ///     Gets the last state of the walk from the queue
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IHastingMemento Get()
        {
            if (HastingMementoList.TryDequeue(out var hastingMemento))
            {
                return hastingMemento;
            }

            throw new Exception();
        }
    }
}
