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
using System.Net;
using System.Reflection;
using Catalyst.Abstractions.Dfs;
using Catalyst.Abstractions.Keystore;
using Catalyst.Abstractions.P2P;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.Network;
using Catalyst.Protocol.Network;
using Catalyst.Protocol.Peer;
using Dawn;
using Lib.P2P;
using Microsoft.Extensions.Configuration;
using MultiFormats;

namespace Catalyst.Core.Lib.P2P
{
    /// <summary>
    ///     Peer settings class.
    /// </summary>
    public sealed class PeerSettings : IPeerSettings
    {
        private readonly NetworkType _networkType;
        public NetworkType NetworkType => _networkType;
        public string PublicKey { get; }
        public int Port { get; }
        public string PayoutAddress { get; }
        public IPAddress BindAddress { get; }
        public IList<string> SeedServers { get; }
        public IPEndPoint[] DnsServers { get; }
        public MultiAddress PeerId { set; get; }

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="rootSection"></param>
        public PeerSettings(IConfigurationRoot rootSection, Peer localPeer)
        {
            Guard.Argument(rootSection, nameof(rootSection)).NotNull();

            var section = rootSection.GetSection("CatalystNodeConfiguration").GetSection("Peer");
            Enum.TryParse(section.GetSection("Network").Value, out _networkType);

            var pksi = Convert.FromBase64String(localPeer.PublicKey);
            PublicKey = pksi.GetPublicKeyBytesFromPeerId().ToBase58();

            Port = int.Parse(section.GetSection("Port").Value);
            PayoutAddress = section.GetSection("PayoutAddress").Value;
            BindAddress = IPAddress.Parse(section.GetSection("BindAddress").Value);
            SeedServers = section.GetSection("SeedServers").GetChildren().Select(p => p.Value).ToList();
            DnsServers = section.GetSection("DnsServers")
               .GetChildren()
               .Select(p => EndpointBuilder.BuildNewEndPoint(p.Value)).ToArray();

            var publicIpAddress = IPAddress.Parse(section.GetSection("PublicIpAddress").Value);

            PeerId = new MultiAddress("/ip4/192.168.0.181/tcp/4001/ipfs/18n3naE9kBZoVvgYMV6saMZdwu2yu3QMzKa2BDkb5C5pcuhtrH1G9HHbztbbxA8tGmf4");

            //PublicKey.BuildPeerIdFromBase58Key(publicIpAddress, Port);
        }
    }
}
