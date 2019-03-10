using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Catalyst.Node.Common.Interfaces {
    public interface ISessionHandler : IChannelHandler
    {    
        void ExceptionCaught(IChannelHandlerContext context, Exception exception);
        void ChannelRegistered(IChannelHandlerContext context);
        void ChannelUnregistered(IChannelHandlerContext context);
        void ChannelActive(IChannelHandlerContext context);
        void ChannelInactive(IChannelHandlerContext context);
        void ChannelRead(IChannelHandlerContext context, object message);
        void ChannelReadComplete(IChannelHandlerContext context);
        void ChannelWritabilityChanged(IChannelHandlerContext context);
        void HandlerAdded(IChannelHandlerContext context);
        void HandlerRemoved(IChannelHandlerContext context);
        void UserEventTriggered(IChannelHandlerContext context, object evt);
        Task WriteAsync(IChannelHandlerContext context, object message);
        void Flush(IChannelHandlerContext context);
        Task BindAsync(IChannelHandlerContext context, EndPoint localAddress);
        Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress);
        Task DisconnectAsync(IChannelHandlerContext context);
        Task CloseAsync(IChannelHandlerContext context);
        Task DeregisterAsync(IChannelHandlerContext context);
        void Read(IChannelHandlerContext context);
        bool IsSharable { get; }
    }
}