﻿using System.IO;
using System.Linq;
using Lib.P2P;
using PeterO.Cbor;

namespace Catalyst.Core.Modules.Dfs.LinkedData
{
    /// <summary>
    ///   Linked data as a protobuf message.
    /// </summary>
    /// <remarks>
    ///   This is the original legacy format used by the IPFS <see cref="Catalyst.Core.Modules.Dfs.DagNode"/>. 
    /// </remarks>
    public class ProtobufFormat : ILinkedDataFormat
    {
        /// <inheritdoc />
        public CBORObject Deserialise(byte[] data)
        {
            using (var ms = new MemoryStream(data, false))
            {
                var node = new DagNode(ms);
                var links = node.Links
                   .Select(link => CBORObject.NewMap()
                       .Add("Cid", CBORObject.NewMap()
                           .Add("/", link.Id.Encode())
                        )
                       .Add("Name", link.Name)
                       .Add("Size", link.Size))
                   .ToArray();
                var cbor = CBORObject.NewMap()
                   .Add("data", node.DataBytes)
                   .Add("links", links);
                return cbor;
            }
        }

        /// <inheritdoc />
        public byte[] Serialize(CBORObject data)
        {
            var links = data["links"].Values
               .Select(link => new DagLink(
                    link["Name"].AsString(),
                    Cid.Decode(link["Cid"]["/"].AsString()),
                    link["Size"].AsInt64()));
            var node = new DagNode(data["data"].GetByteString(), links);
            using (var ms = new MemoryStream())
            {
                node.Write(ms);
                return ms.ToArray();
            }
        }
    }
}
