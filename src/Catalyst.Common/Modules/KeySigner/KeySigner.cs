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
using Catalyst.Common.Config;
using Catalyst.Common.Interfaces.Cryptography;
using Catalyst.Common.Interfaces.Keystore;
using Catalyst.Cryptography.BulletProofs.Wrapper.Interfaces;
using Catalyst.Common.Interfaces.Modules.KeySigner;
using Catalyst.Common.Interfaces.Registry;
using Catalyst.Cryptography.BulletProofs.Wrapper.Exceptions;
using Catalyst.Cryptography.BulletProofs.Wrapper.Types;

namespace Catalyst.Common.Modules.KeySigner
{
    public class KeySigner : IKeySigner
    {
        private readonly IKeyStore _keyStore;
        private readonly ICryptoContext _cryptoContext;
        private readonly IKeyRegistry _keyRegistry;
        private readonly KeyRegistryKey _defaultKey = KeyRegistryKey.DefaultKey;

        /// <summary>Initializes a new instance of the <see cref="KeySigner"/> class.</summary>
        /// <param name="keyStore">The key store.</param>
        /// <param name="cryptoContext">The crypto context.</param>
        /// /// <param name="keyRegistry">The key registry.</param>
        public KeySigner(IKeyStore keyStore, ICryptoContext cryptoContext, IKeyRegistry keyRegistry)
        {
            _keyStore = keyStore;
            _cryptoContext = cryptoContext;
            _keyRegistry = keyRegistry;
            InitialiseKeyRegistry();
        }

        private void InitialiseKeyRegistry()
        {
            if (!TryPopulateKeyRegistryWithDefault())
            {
                GenerateKeyAndPopulateRegistryWithDefault();
            }   
        }

        private async void GenerateKeyAndPopulateRegistryWithDefault()
        {
            IPrivateKey privateKey = _cryptoContext.GeneratePrivateKey();

            await _keyStore.KeyStoreGenerateAsync(privateKey, _defaultKey);

            if (!TryPopulateKeyRegistryWithDefault())
            {
                throw new SignatureException("Failed to populate registry with new key");
            }
        }

        /// <inheritdoc/>
        IKeyStore IKeySigner.KeyStore => _keyStore;

        /// <inheritdoc/>
        ICryptoContext IKeySigner.CryptoContext => _cryptoContext;

        public ISignature Sign(byte[] data, KeyRegistryKey keyIdentifier)
        {
            var privateKey = _keyRegistry.GetItemFromRegistry(keyIdentifier);
            return Sign(data, privateKey);
        }

        public ISignature Sign(byte[] data)
        {
            return Sign(data, KeyRegistryKey.DefaultKey);
        }

        private ISignature Sign(byte[] data, IPrivateKey privateKey)
        {
            if (privateKey != null)
            {
                return _cryptoContext.Sign(privateKey, data);
            }

            return new Signature(new byte[64], new byte[32]);
        }

        /// <inheritdoc/>
        public bool Verify(ISignature signature, byte[] message)
        {
            return _cryptoContext.Verify(signature, message);
        }

        /// <inheritdoc/>
        public void ExportKey()
        {
            throw new NotImplementedException();
        }

        private bool TryPopulateKeyRegistryWithDefault()
        {
            var key = _keyStore.KeyStoreDecrypt(_defaultKey);
            return key != null && _keyRegistry.AddItemToRegistry(_defaultKey, key);
        }    
    }
}
