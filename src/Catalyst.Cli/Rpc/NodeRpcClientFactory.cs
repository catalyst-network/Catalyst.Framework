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
using System.Security.Cryptography.X509Certificates;
using Catalyst.Common.Interfaces.IO;
using Catalyst.Common.Interfaces.IO.Messaging;
using Catalyst.Common.Interfaces.Rpc;
using Microsoft.Extensions.Configuration;

namespace Catalyst.Cli.Rpc
{
    public sealed class NodeRpcClientFactory : INodeRpcClientFactory
    {
        private readonly IEnumerable<IRpcResponseHandler> _responseHandlers;
        private readonly IConfigurationRoot _configuration;
        private readonly IRpcBusinessEventFactory _businessEventFactory;

        public NodeRpcClientFactory(IEnumerable<IRpcResponseHandler> responseHandlers, 
            IConfigurationRoot configurationRoot,
            IRpcBusinessEventFactory businessEventFactory)
        {
            _responseHandlers = responseHandlers;
            _configuration = configurationRoot;
            _businessEventFactory = businessEventFactory;
        }

        public INodeRpcClient GetClient(X509Certificate certificate, IRpcNodeConfig nodeConfig)
        {
            return new NodeRpcClient(certificate, nodeConfig, _businessEventFactory, _responseHandlers);
        }
    }
}
