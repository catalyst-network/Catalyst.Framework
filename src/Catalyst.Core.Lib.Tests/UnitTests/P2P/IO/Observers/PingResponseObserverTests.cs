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
using Catalyst.Abstractions.P2P.IO.Messaging.Dto;
using Catalyst.Abstractions.P2P.Protocols;
using Catalyst.Core.Lib.Extensions;
using Catalyst.Core.Lib.IO.Messaging.Correlation;
using Catalyst.Core.Lib.P2P.IO.Observers;
using Catalyst.Protocol.IPPN;
using Catalyst.TestUtils;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;
using NUnit.Framework;

namespace Catalyst.Core.Lib.Tests.UnitTests.P2P.IO.Observers
{
    public sealed class PingResponseObserverTests : IDisposable
    {
        private readonly PingResponseObserver _observer;

        public PingResponseObserverTests()
        {
            _observer = new PingResponseObserver(Substitute.For<IPeerChallengeRequest>(), Substitute.For<ILogger>());
        }

        [Test]
        public void Observer_Can_Process_PingResponse_Correctly()
        {
            var testScheduler = new TestScheduler();

            var pingResponse = new PingResponse();
            var protocolMessage =
                pingResponse.ToProtocolMessage(MultiAddressHelper.GetAddress("sender"),
                    CorrelationId.GenerateCorrelationId());

            var pingResponseObserver = Substitute.For<IObserver<IPeerClientMessageDto>>();

            var messageStream = MessageStreamHelper.CreateStreamWithMessage(testScheduler, protocolMessage);

            _observer.StartObserving(messageStream);

            using (_observer.MessageStream.Subscribe(pingResponseObserver.OnNext))
            {
                testScheduler.Start();

                pingResponseObserver.Received(1).OnNext(Arg.Any<IPeerClientMessageDto>());
            }
        }

        public void Dispose() { _observer?.Dispose(); }
    }
}
