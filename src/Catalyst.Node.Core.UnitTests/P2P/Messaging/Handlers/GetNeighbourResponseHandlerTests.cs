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
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Autofac;
using Catalyst.Common.Config;
using Catalyst.Common.Extensions;
using Catalyst.Common.Interfaces.IO.Messaging;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.IO.Inbound;
using Catalyst.Common.UnitTests.TestUtils;
using Catalyst.Node.Core.P2P.Messaging.Handlers;
using Catalyst.Protocol.IPPN;
using DotNetty.Transport.Channels;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Catalyst.Node.Core.UnitTest.P2P.Messaging.Handlers
{
    public sealed class GetNeighbourResponseHandlerTests : ConfigFileBasedTest
    {
        private IChannelHandlerContext _fakeContext;
        private readonly ILogger _logger;
        private readonly IReputableCache _subbedReputableCache;

        public GetNeighbourResponseHandlerTests(ITestOutputHelper output) : base(output)
        {
            _logger = Substitute.For<ILogger>();
            _fakeContext = Substitute.For<IChannelHandlerContext>();
            _subbedReputableCache = Substitute.For<IReputableCache>();
        }

        [Fact]
        public void CanResolveGetNeighbourResponseHandlerFromContainer()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile(Path.Combine(Constants.ConfigSubFolder, Constants.ComponentsJsonConfigFile))
               .AddJsonFile(Path.Combine(Constants.ConfigSubFolder, Constants.SerilogJsonConfigFile))
               .AddJsonFile(Path.Combine(Constants.ConfigSubFolder, Constants.NetworkConfigFile(Network.Dev)))
               .Build();
            
            ConfigureContainerBuilder(config, true, true);

            var container = ContainerBuilder.Build();
            using (container.BeginLifetimeScope(CurrentTestName))
            {
                var p2PMessageHandlers = container.Resolve<IEnumerable<IP2PMessageHandler>>();
                IEnumerable<IP2PMessageHandler> getNeighbourResponseHandler = p2PMessageHandlers.OfType<GetNeighbourResponseHandler>();
                getNeighbourResponseHandler.First().Should().BeOfType(typeof(GetNeighbourResponseHandler));
            }
        }

        [Fact]
        public void CanHandlerGetNeighbourRequestHandlerCorrectly()
        {
            var neighbourResponseHandler = new GetNeighbourResponseHandler(_subbedReputableCache, _logger);
            var peerNeighbourResponseMessage = new PeerNeighborsResponse();
            
            var fakeContext = Substitute.For<IChannelHandlerContext>();
            var channeledAny = new ChanneledAnySigned(fakeContext, peerNeighbourResponseMessage.ToAnySigned(PeerIdHelper.GetPeerId(), Guid.NewGuid()));
            var observableStream = new[] {channeledAny}.ToObservable();
            neighbourResponseHandler.StartObservingMessageStreams(observableStream);
            neighbourResponseHandler.ReputableCache.ReceivedWithAnyArgs(1);
        }

        [Fact]
        public void PeerDiscoveryCanHandlePeerNeighbourMessageSubscriptions()
        {
            var subbedPeerDiscovery = Substitute.For<IPeerDiscovery>();
            var peerNeighbourResponseMessage = new PeerNeighborsResponse();
            
            var fakeContext = Substitute.For<IChannelHandlerContext>();
            var channeledAny = new ChanneledAnySigned(fakeContext, peerNeighbourResponseMessage.ToAnySigned(PeerIdHelper.GetPeerId(), Guid.NewGuid()));
            var observableStream = new[] {channeledAny}.ToObservable();
            subbedPeerDiscovery.StartObservingMessageStreams(observableStream);
            subbedPeerDiscovery.GetNeighbourResponseStream.ReceivedWithAnyArgs(1);
        }
    }
}
