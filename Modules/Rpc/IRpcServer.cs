using Grpc.Core;
using ADL.Protocol.Rpc.Node;
using System.Threading.Tasks;

namespace ADL.Rpc
{
    public interface IRpcServer
    {
        void CreateServer(string bindAddress, int port);
        
        Task<PongResponse> Ping(PingRequest request, ServerCallContext context);

        Task<VersionResponse> Version(VersionRequest request, ServerCallContext context);
        
//        Task<GetInfoRequest> GetInfo();
    }
}
