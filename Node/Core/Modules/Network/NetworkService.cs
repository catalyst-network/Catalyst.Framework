using System;
using System.Threading.Tasks;
using ADL.Node.Core.Helpers.Services;
using ADL.Node.Core.Modules.Network.Peer;

namespace ADL.Node.Core.Modules.Network
{
    /// <summary>
    /// The Peer Service 
    /// </summary>
    public class NetworkService : AsyncServiceBase, INetworkService
    {
        public Network Network { get; set; }
        private string DataDir { get; set; }
        private ISslSettings SslSettings { get; set; }
        private INetworkSettings NetworkSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="networkSettings"></param>
        /// <param name="sslSettings"></param>
        /// <param name="options"></param>
        public NetworkService(INetworkSettings networkSettings, ISslSettings sslSettings, NodeOptions options)
        {
            SslSettings = sslSettings;
            DataDir = options.DataDir;
            NetworkSettings = networkSettings;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p2PSettings"></param>
        /// <param name="sslSettings"></param>
        /// <param name="dataDir"></param>
        /// <returns></returns>
        public override bool StartService()
        {
            Network = Network.GetInstance(NetworkSettings, SslSettings, DataDir);
            Task.Run(async () => await PeerManager.InboundConnectionListener());
            PeerManager.PeerBuilder("127.0.0.1",43069);
            return true;
        }
            
        public override bool StopService()
        {
            PeerManager.Dispose();
            return false;
        }
    }
} 
