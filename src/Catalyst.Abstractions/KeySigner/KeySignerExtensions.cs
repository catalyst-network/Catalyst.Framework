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
using System.Buffers;
using Catalyst.Abstractions.Cryptography;
using Catalyst.Protocol.Cryptography;
using Google.Protobuf;

namespace Catalyst.Abstractions.KeySigner
{
    public static class KeySignerExtensions
    {
        public static ISignature Sign(this IKeySigner crypto, IMessage message, SigningContext context)
        {
            var messageSize = message.CalculateSize();
            var array = ArrayPool<byte>.Shared.Rent(messageSize);

            using (var output = new CodedOutputStream(array))
            {
                message.WriteTo(output);
            }

            var result = crypto.Sign(array.AsSpan(0, messageSize), context);

            ArrayPool<byte>.Shared.Return(array);

            return result;
        }

        public static bool Verify(this IKeySigner crypto, ISignature signature, IMessage message, SigningContext context)
        {
            var messageSize = message.CalculateSize();
            var array = ArrayPool<byte>.Shared.Rent(messageSize);

            using (var output = new CodedOutputStream(array))
            {
                message.WriteTo(output);
            }

            var result = crypto.Verify(signature, array.AsSpan(0, messageSize), context);

            ArrayPool<byte>.Shared.Return(array);

            return result;
        }
    }
}
