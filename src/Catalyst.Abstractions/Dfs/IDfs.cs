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
using System.Threading;
using System.Threading.Tasks;
using Catalyst.Abstractions.Dfs.BlockExchange;
using Catalyst.Abstractions.Dfs.CoreApi;
using Catalyst.Abstractions.Dfs.Migration;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.Options;
using Lib.P2P;
using Lib.P2P.Protocols;
using Lib.P2P.PubSub;
using Lib.P2P.Routing;
using Nito.AsyncEx;

namespace Catalyst.Abstractions.Dfs
{
    /// <summary>
    ///   Provides read-write access to a distributed file system.
    /// </summary>
    public interface IDfs : ICoreApi, IService, IDisposable
    {
        bool IsStarted { get; }

        MigrationManager MigrationManager { get; set; }
        
        /// <summary>
        ///   Provides access to the local peer.
        /// </summary>
        /// <returns>
        ///   A task that represents the asynchronous operation. The task's result is
        ///   a <see cref="Peer"/>.
        /// </returns>
        AsyncLazy<Peer> LocalPeer { get; }
        
        DfsOptions Options { get; set; }
        
        /// <summary>
        ///   Determines latency to a peer.
        /// </summary>
        AsyncLazy<Ping1> PingService { get; }
        
        /// <summary>
        ///   Manages communication with other peers.
        /// </summary>
        AsyncLazy<Swarm> SwarmService { get; }

        /// <summary>
        ///   Manages publishng and subscribing to messages.
        /// </summary>
        AsyncLazy<NotificationService> PubSubService { get; }

        /// <summary>
        ///   Exchange blocks with other peers.
        /// </summary>
        AsyncLazy<IBitswapService> BitswapService { get; }
        
        /// <summary>
        ///   Finds information with a distributed hash table.
        /// </summary>
        AsyncLazy<Dht1> DhtService { get; }

        Task<IKeyApi> KeyChainAsync(CancellationToken cancel = default(CancellationToken));

        Task<Cid> ResolveIpfsPathToCidAsync(string path,
            CancellationToken cancel = default(CancellationToken));
    }
}
