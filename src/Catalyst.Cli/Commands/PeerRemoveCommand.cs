using Catalyst.Common.Interfaces.Cli.Commands;
using Catalyst.Common.Interfaces.Cli.Options;
using Catalyst.Common.Network;
using Catalyst.Protocol.Rpc.Node;
using Google.Protobuf;
using Nethereum.RLP;
using System.Net;
using Catalyst.Cli.Options;

namespace Catalyst.Cli.Commands
{
    public class PeerRemoveCommand : MessageCommand<RemovePeerRequest, RemovePeerOptions>
    {
        public PeerRemoveCommand(ICommandContext commandContext) : base(commandContext) { }

        protected override RemovePeerRequest GetMessage(RemovePeerOptions option)
        {
            return new RemovePeerRequest
            {
                PeerIp = ByteString.CopyFrom(IPAddress.Parse(option.Ip).To16Bytes()),
                PublicKey = string.IsNullOrEmpty(option.PublicKey)
                    ? ByteString.Empty
                    : ByteString.CopyFrom(option.PublicKey.ToBytesForRLPEncoding())
            };
        }
    }
}
