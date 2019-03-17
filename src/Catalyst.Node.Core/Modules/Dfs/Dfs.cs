/*
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Catalyst.Node.Common.Interfaces;
using Catalyst.Node.Common.Interfaces.Modules.Dfs;
using Ipfs;

namespace Catalyst.Node.Core.Modules.Dfs
{

    public class Dfs : IDfs
    {
        private readonly IIpfsConnector _ipfsConnector;

        public Dfs(IIpfsConnector ipfsConnector)
        {
            _ipfsConnector = ipfsConnector;
        }
        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _ipfsConnector.Service.StartAsync();
        }

        /// <inheritdoc />
        public async Task<IFileSystemNode> AddFileAsync(string filename, CancellationToken cancellationToken = default)
        {
            return await _ipfsConnector.FileSystem.AddFileAsync(filename, cancel: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        {
            return await _ipfsConnector.FileSystem.ReadAllTextAsync(path, cancellationToken);
        }
    }
}
