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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Catalyst.Common.Config;
using Catalyst.Common.Interfaces.FileSystem;
using Catalyst.Common.Interfaces.Modules.Dfs;
using Dawn;
using Ipfs;
using Ipfs.Registry;
using Multiformats.Hash.Algorithms;

namespace Catalyst.Modules.Lib.Dfs
{
    /// <summary>
    ///     A very naive implementation of the IDfs interface that simply relies on the file system.
    ///     This can only result in a 'Distributed' file system if the <see cref="_baseFolder" /> happens
    ///     to be shared network path. Otherwise, this can be used in integration tests, to ensure
    ///     the tests can be run locally and offline.
    /// </summary>
    /// <remarks>
    ///     The hashing algorithm is also a simple one (<see cref="BLAKE2B_32" />>) to save time
    ///     in integration tests
    /// </remarks>
    /// <inheritdoc cref="IDfs" />
    public class FileSystemDfs : IDfs
    {
        private readonly DirectoryInfo _baseFolder;
        private readonly IFileSystem _fileSystem;
        private readonly HashingAlgorithm _hashingAlgorithm;

        public FileSystemDfs(HashingAlgorithm hashingAlgorithm,
            IFileSystem fileSystem,
            string baseFolder = null)
        {
            Guard.Argument(hashingAlgorithm, nameof(hashingAlgorithm))
               .Require(h => h.DigestSize <= 191, h =>
                    "The hashing algorithm needs to produce file names smaller than 255 base 64 characters or 191 bytes" +
                    $"but the default length for {hashingAlgorithm.GetType().Name} is {h.DigestSize}.");

            var dfsBaseFolder = baseFolder ?? Path.Combine(fileSystem.GetCatalystDataDir().FullName,
                Constants.DfsDataSubDir);

            _baseFolder = new DirectoryInfo(dfsBaseFolder);
            if (!_baseFolder.Exists)
            {
                _baseFolder.Create();
            }

            _hashingAlgorithm = hashingAlgorithm;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public async Task<string> AddTextAsync(string utf8Content, CancellationToken cancellationToken = default)
        {
            var bytes = Encoding.UTF8.GetBytes(utf8Content);
            var contentHash = MultiHash.ComputeHash(bytes, _hashingAlgorithm.Name);
            var filePath = Path.Combine(_baseFolder.FullName, contentHash.ToString());

            await _fileSystem.File.WriteAllTextAsync(
                filePath,
                utf8Content, Encoding.UTF8, cancellationToken);

            return contentHash.ToString();
        }

        /// <inheritdoc />
        public async Task<string> ReadTextAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _fileSystem.File.ReadAllTextAsync(Path.Combine(_baseFolder.FullName, id), Encoding.UTF8,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> AddAsync(Stream content,
            string name = "",
            CancellationToken cancellationToken = default)
        {
            var contentHash = MultiHash.ComputeHash(content, _hashingAlgorithm.Name);
            var filePath = Path.Combine(_baseFolder.FullName, contentHash.ToString());

            using (var file = _fileSystem.File.Create(filePath))
            {
                content.Position = 0;
                await content.CopyToAsync(file, cancellationToken);
            }

            return contentHash.ToString();
        }

        /// <inheritdoc />
        public async Task<Stream> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_fileSystem.File.OpenRead(Path.Combine(_baseFolder.FullName, id)))
               .ConfigureAwait(false);
        }

        ///// <inheritdoc />
        //public async Task<string> AddAsync(byte[] content,
        //    string name = "",
        //    CancellationToken cancellationToken = default)
        //{
        //    var contentHash = MultiHash.ComputeHash(content, _hashingAlgorithm.Name);
        //    var filePath = Path.Combine(_baseFolder.FullName, contentHash.ToString());
        //    await _fileSystem.File.WriteAllBytesAsync(filePath, content, cancellationToken);
        //    return contentHash.ToString();
        //}
    }
}
