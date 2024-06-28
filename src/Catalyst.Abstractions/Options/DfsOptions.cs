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

using System.IO;
using Lib.P2P.Cryptography;
using Makaretu.Dns;
using MultiFormats;

namespace Catalyst.Abstractions.Options
{
    /// <summary>
    ///     Configuration options for the <see cref="Catalyst.Core.Modules.Dfs.Dfs" />.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Options.Options" />
    public class DfsOptions
    {
        /// <summary>
        ///     Repository options.
        /// </summary>
        public RepositoryOptions Repository { get; set; }

        /// <summary>
        ///     KeyChain options.
        /// </summary>
        public KeyChainOptions KeyChain { get; set; }

        /// <summary>
        ///     Provides access to the Domain Name System.
        /// </summary>
        /// <value>
        ///     Defaults to <see cref="Makaretu.Dns.DotClient" />, DNS over TLS.
        /// </value>
        public IDnsClient Dns { get; set; }

        /// <summary>
        ///     Block options.
        /// </summary>
        public BlockOptions Block { get; set; }

        /// <summary>
        ///     Discovery options.
        /// </summary>
        public DiscoveryOptions Discovery { get; set; }

        /// <summary>
        ///     Swarm (network) options.
        /// </summary>
        public SwarmOptions Swarm { get; set; }

        public DfsOptions(BlockOptions blockOptions,
            DiscoveryOptions discoveryOptions,
            RepositoryOptions repositoryOptions,
            KeyChainOptions keyChainOptions,
            SwarmOptions swarmOptions,
            IDnsClient dnsClient)
        {
            Block = blockOptions;
            Discovery = discoveryOptions;
            Repository = repositoryOptions;
            KeyChain = keyChainOptions;
            Swarm = swarmOptions;
            Dns = dnsClient;

            // Do not use the public IPFS network, use a private network of catalyst only nodes.
            var swarmKey = "07a8e9d0c43400927ab274b7fa443596b71e609bacae47bd958e5cd9f59d6ca3";
            Swarm.PrivateNetworkKey = new PreSharedKey
            {
                Value = swarmKey.ToHexBuffer()
            };

            if (Swarm.PrivateNetworkKey == null)
            {
                var path = Path.Combine(Repository.Folder, "swarm.key");
                if (File.Exists(path))
                {
                    using var x = File.OpenText(path);
                    Swarm.PrivateNetworkKey = new PreSharedKey();
                    Swarm.PrivateNetworkKey.Import(x);
                }
            }
        }
    }
}
