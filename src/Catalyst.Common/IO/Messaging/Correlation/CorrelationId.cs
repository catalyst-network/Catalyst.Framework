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
using Catalyst.Common.Interfaces.IO.Messaging.Correlation;
using Google.Protobuf;

namespace Catalyst.Common.IO.Messaging.Correlation
{
    /// <inheritdoc />
    public sealed class CorrelationId : ICorrelationId
    {
        public Guid Id { get; }
        
        /// <summary>
        ///     Provide a known correlation Id.
        /// </summary>
        public CorrelationId(Guid id) { Id = id; } 
        
        /// <summary>
        ///     Provides a correlation Id from a byte array.
        /// </summary>
        public CorrelationId(byte[] bytes) { Id = new Guid(bytes); }

        /// <summary>
        ///     Provides a correlation Id from a protobuf byte string.
        /// </summary>
        public CorrelationId(ByteString byteString) : this(byteString.ToByteArray()) { }

        /// <summary>
        /// Gets a new CorrelationId
        /// </summary>
        private CorrelationId() { Id = Guid.NewGuid(); }

        /// <summary>
        ///     Static helper to get new CorrelationId.
        /// </summary>
        public static ICorrelationId GenerateCorrelationId()
        {
            return new CorrelationId();
        }

        public override string ToString() { return Id.ToString(); }

        public bool Equals(ICorrelationId other)
        {
            return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Id.Equals(other.Id));
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ICorrelationId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(CorrelationId left, CorrelationId right) { return Equals(left, right); }
        public static bool operator !=(CorrelationId left, CorrelationId right) { return !Equals(left, right); }
    }
}
