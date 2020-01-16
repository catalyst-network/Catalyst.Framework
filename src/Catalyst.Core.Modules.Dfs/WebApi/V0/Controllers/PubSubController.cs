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
using System.Linq;
using System.Threading.Tasks;
using Catalyst.Abstractions.Dfs;
using Catalyst.Abstractions.Dfs.CoreApi;
using Lib.P2P.PubSub;
using Microsoft.AspNetCore.Mvc;

namespace Catalyst.Core.Modules.Dfs.WebApi.V0.Controllers
{
    /// <summary>
    ///     A list of topics.
    /// </summary>
    public class PubsubTopicsDto
    {
        /// <summary>
        ///     A list of topics.
        /// </summary>
        public IEnumerable<string> Strings;
    }

    /// <summary>
    ///     A list of peers.
    /// </summary>
    public class PubsubPeersDto
    {
        /// <summary>
        ///     A list of peer IDs.
        /// </summary>
        public IEnumerable<string> Strings;
    }

    /// <summary>
    ///     A published message.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        ///     The base-64 encoding of the author's peer id.
        /// </summary>
        public string from;

        /// <summary>
        ///     The base-64 encoding of the author's unique sequence number.
        /// </summary>
        public string seqno;

        /// <summary>
        ///     The base-64 encoding of the message data.
        /// </summary>
        public string data;

        /// <summary>
        ///     The topics associated with the message.
        /// </summary>
        public string[] topicIDs;

        /// <summary>
        ///     Create a new instance of the <see cref="MessageDto" />
        ///     from the <see cref="IPublishedMessage" />.
        /// </summary>
        /// <param name="msg">
        ///     A pubsub messagee.
        /// </param>
        public MessageDto(IPublishedMessage msg)
        {
            from = Convert.ToBase64String(msg.Sender.Id.ToArray());
            seqno = Convert.ToBase64String(msg.SequenceNumber);
            data = Convert.ToBase64String(msg.DataBytes);
            topicIDs = msg.Topics.ToArray();
        }
    }

    /// <summary>
    ///     Publishing and subscribing to messages on a topic.
    /// </summary>
    public class PubSubController : IpfsController
    {
        /// <summary>
        ///     Creates a new controller.
        /// </summary>
        public PubSubController(IDfsService dfs) : base(dfs) { }

        /// <summary>
        ///     List all the subscribed topics.
        /// </summary>
        [HttpGet] [HttpPost] [Route("pubsub/ls")]
        public async Task<PubsubTopicsDto> List()
        {
            return new PubsubTopicsDto
            {
                Strings = await IpfsCore.PubSubApi.SubscribedTopicsAsync(Cancel)
            };
        }

        /// <summary>
        ///     List all the peers associated with the topic.
        /// </summary>
        /// <param name="arg">
        ///     The topic name or null/empty for "all topics".
        /// </param>
        [HttpGet] [HttpPost] [Route("pubsub/peers")]
        public async Task<PubsubPeersDto> Peers(string arg)
        {
            var topic = string.IsNullOrEmpty(arg) ? null : arg;
            var peers = await IpfsCore.PubSubApi.PeersAsync(topic, Cancel);
            return new PubsubPeersDto
            {
                Strings = peers.Select(p => p.Id.ToString())
            };
        }

        /// <summary>
        ///     Publish a message to a topic.
        /// </summary>
        /// <param name="arg">
        ///     The first arg is the topic name and second is the message.
        /// </param>
        [HttpGet] [HttpPost] [Route("pubsub/pub")]
        public async Task Publish(string[] arg)
        {
            if (arg.Length != 2)
                throw new ArgumentException("Missing topic and/or message.");
            var message = arg[1].Select(c => (byte) c).ToArray();
            await IpfsCore.PubSubApi.PublishAsync(arg[0], message, Cancel);
        }

        /// <summary>
        ///     Subscribe to messages on the topic.
        /// </summary>
        /// <param name="arg">
        ///     The topic name.
        /// </param>
        [HttpGet] [HttpPost] [Route("pubsub/sub")]
        public async Task Subscribe(string arg)
        {
            await IpfsCore.PubSubApi.SubscribeAsync(arg, message =>
            {
                // Send the published message to the caller.
                var dto = new MessageDto(message);
                StreamJson(dto);
            }, Cancel);

            // Send 200 OK to caller; but do not close the stream
            // so that published messages can be sent.
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            await Response.Body.FlushAsync();

            // Wait for the caller to cancel.
            try
            {
                await Task.Delay(-1, Cancel);
            }
            catch (TaskCanceledException)
            {
                // eat
            }
        }
    }
}
