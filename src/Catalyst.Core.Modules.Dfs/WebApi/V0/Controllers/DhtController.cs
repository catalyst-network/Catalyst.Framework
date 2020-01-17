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
using System.Threading.Tasks;
using Catalyst.Abstractions.Dfs.CoreApi;
using Microsoft.AspNetCore.Mvc;

// TODO: need MultiAddress.WithOutPeer (should be in DFS code)

namespace Catalyst.Core.Modules.Dfs.WebApi.V0.Controllers
{
    /// <summary>
    ///   Information from the Distributed Hash Table.
    /// </summary>
    public sealed class DhtPeerDto
    {
        /// <summary>
        ///   The ID of the peer that provided the response.
        /// </summary>
        internal string Id;

        /// <summary>
        ///   Unknown.
        /// </summary>
        public int Type; // TODO: what is the type?

        /// <summary>
        ///   The peer that has the information.
        /// </summary>
        internal IEnumerable<DhtPeerResponseDto> Responses;

        /// <summary>
        ///   Unknown.
        /// </summary>
        public string Extra = string.Empty;
    }

    /// <summary>
    ///   Information on a peer that has the information.
    /// </summary>
    internal sealed class DhtPeerResponseDto
    {
        /// <summary>
        ///   The peer ID.
        /// </summary>
        public string Id;

        /// <summary>
        ///   The listening addresses of the peer.
        /// </summary>
        public IEnumerable<string> Addrs;
    }

    /// <summary>
    ///   Distributed Hash Table.
    /// </summary>
    /// <remarks>
    ///   The DHT is a place to store, not the value, but pointers to peers who have 
    ///   the actual value.
    /// </remarks>
    public sealed class DhtController : IpfsController
    {
        /// <summary>
        ///   Creates a new controller.
        /// </summary>
        public DhtController(ICoreApi dfs) : base(dfs) { }

        /// <summary>
        ///   Query the DHT for all of the multiaddresses associated with a Peer ID.
        /// </summary>
        /// <param name="arg">
        ///   The peer ID to find.
        /// </param>
        /// <returns>
        ///   Information about the peer.
        /// </returns>
        [HttpGet, HttpPost, Route("dht/findpeer")]
        public async Task<DhtPeerDto> FindPeer(string arg)
        {
            var peer = await IpfsCore.DhtApi.FindPeerAsync(arg, Cancel);
            return new DhtPeerDto
            {
                Id = peer.Id.ToBase58(),
                Responses = new[]
                {
                    new DhtPeerResponseDto
                    {
                        Id = peer.Id.ToBase58(),
                        Addrs = peer.Addresses.Select(a => a.WithoutPeerId().ToString())
                    }
                }
            };
        }

        /// <summary>
        ///  Find peers in the DHT that can provide a specific value, given a key.
        /// </summary>
        /// <param name="arg">
        ///   The CID key,
        /// </param>
        /// <param name="limit">
        ///   The maximum number of providers to find.
        /// </param>
        /// <returns>
        ///   Information about the peer providers.
        /// </returns>
        [HttpGet, HttpPost, Route("dht/findprovs")]
        public async Task<IEnumerable<DhtPeerDto>> FindProviders(string arg,
            [ModelBinder(Name = "num-providers")] int limit = 20)
        {
            var peers = await IpfsCore.DhtApi.FindProvidersAsync(arg, limit, null, Cancel);
            return peers.Select(peer => new DhtPeerDto
            {
                Id = peer.Id.ToBase58(), // TODO: should be the peer ID that answered the query
                Responses = new[]
                {
                    new DhtPeerResponseDto
                    {
                        Id = peer.Id.ToBase58(),
                        Addrs = peer.Addresses.Select(a => a.WithoutPeerId().ToString())
                    }
                }
            });
        }
    }
}
