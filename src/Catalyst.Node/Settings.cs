using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Catalyst.Helpers.Network;
using Catalyst.Helpers.Util;
using Catalyst.Node.Modules.Core.Consensus;
using Catalyst.Node.Modules.Core.Contract;
using Catalyst.Node.Modules.Core.Dfs;
using Catalyst.Node.Modules.Core.Gossip;
using Catalyst.Node.Modules.Core.Ledger;
using Catalyst.Node.Modules.Core.Mempool;
using Catalyst.Node.Modules.Core.P2P;
using Catalyst.Node.Modules.Core.Rpc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Catalyst.Node
{
    public sealed class Settings
    {
        private static readonly object Mutex = new object();

        /// <summary>
        ///     Settings constructor
        /// </summary>
        /// <param name="options"></param>
        private Settings(NodeOptions options)
        {
            Guard.NotNull(options, nameof(options));
            var userConfig = LoadConfig(options.DataDir, options.Network);

            Dfs = new DfsSettings(userConfig.GetSection("Dfs"));
            Rpc = new RpcSettings(userConfig.GetSection("Rpc"));
            Peer = new P2PSettings(userConfig.GetSection("Peer"));
            Gossip = new GossipSettings(userConfig.GetSection("Gossip"));
            Ledger = new LedgerSettings(userConfig.GetSection("Ledger"));
            Mempool = new MempoolSettings(userConfig.GetSection("Mempool"));
            Contract = new ContractSettings(userConfig.GetSection("Contract"));
            Consensus = new ConsensusSettings(userConfig.GetSection("Consensus"));
        }

        public IRpcSettings Rpc { get; set; }
        public IDfsSettings Dfs { get; set; }
        public IP2PSettings Peer { get; set; }
        public IGossipSettings Gossip { get; set; }
        public ILedgerSettings Ledger { get; set; }
        public IMempoolSettings Mempool { get; set; }
        private static Settings Instance { get; set; }
        public IContractSettings Contract { get; set; }
        public IConsensusSettings Consensus { get; set; }

        /// <summary>
        ///     Get a thread safe settings singleton.
        /// </summary>
        /// <returns></returns>
        public static Settings GetInstance(NodeOptions options)
        {
            Guard.NotNull(options, nameof(options));

            if (Instance == null)
                lock (Mutex)
                {
                    if (Instance == null) Instance = new Settings(options);
                }

            return Instance;
        }

        /// <summary>
        ///     Loads config files from defined dataDir
        /// </summary>
        /// <param name="dataDir"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        private static IConfiguration LoadConfig(string dataDir, string network)
        {
            Guard.NotNull(dataDir, nameof(dataDir));
            Guard.NotNull(network, nameof(network));

            string networkConfigFile;

            switch (network)
            {
                case "devnet":
                    networkConfigFile = "/devnet.json";
                    break;
                case "mainnet":
                    networkConfigFile = "/mainnet.json";
                    break;
                case "testnet":
                    networkConfigFile = "/testnet.json";
                    break;
                default:
                    networkConfigFile = "/devnet.json";
                    break;
            }

            return new ConfigurationBuilder()
                .AddJsonFile(dataDir + networkConfigFile)
                .Build()
                .GetSection("ApplicationConfiguration");
        }

        /// <summary>
        ///     Serialises setting section to a json string.
        /// </summary>
        /// <returns></returns>
        public string SerializeSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, serializerSettings);
        }
    }

    /// <summary>
    ///     Persistence settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class LedgerSettings : ILedgerSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal LedgerSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            Type = section.GetSection("Persistence").Value;
            Chain = section.GetSection("Paths").GetSection("Chain").Value;
            Index = section.GetSection("Paths").GetSection("Index").Value;
        }

        public string Type { get; set; }
        public string Chain { get; set; }
        public string Index { get; set; }
    }

//        
    /// <summary>
    ///     Consensus settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class ConsensusSettings : IConsensusSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal ConsensusSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            NDepth = section.GetSection("nDepth").Value;
        }

        public string NDepth { get; set; }
    }

    /// <summary>
    ///     Contract settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class ContractSettings : IContractSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal ContractSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            StorageType = section.GetSection("StorageType").Value;
        }

        public string StorageType { get; set; }
    }

    /// <summary>
    ///     Dfs settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class DfsSettings : IDfsSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal DfsSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            StorageType = section.GetSection("StorageType").Value;
            ConnectRetries = ushort.Parse(section.GetSection("ConnectRetries").Value);
            IpfsVersionApi = section.GetSection("IpfsVersionApi").Value;
        }

        public string StorageType { get; set; }
        public ushort ConnectRetries { get; set; }
        public string IpfsVersionApi { get; set; }
    }

    /// <summary>
    ///     Gossip settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class GossipSettings : IGossipSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal GossipSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            Instances = section.GetSection("instances").Value;
        }

        public string Instances { get; set; }
    }

    /// <summary>
    ///     Mempool settings class.
    ///     Holds the local storage locations.
    /// </summary>
    public class MempoolSettings : IMempoolSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal MempoolSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            Type = section.GetSection("Type").Value;
            When = section.GetSection("When").Value;
            Host = EndpointBuilder.BuildNewEndPoint(
                IPAddress.Parse(section.GetSection("Host").Value),
                int.Parse(section.GetSection("Port").Value)
            );
        }

        public string Type { get; set; }
        public string When { get; set; }
        public IPEndPoint Host { get; set; }
    }

    /// <summary>
    ///     Peer settings class.
    /// </summary>
    public class P2PSettings : IP2PSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal P2PSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            Network = section.GetSection("Network").Value;
            Port = int.Parse(section.GetSection("Port").Value);
            Magic = uint.Parse(section.GetSection("Magic").Value);
            PfxFileName = section.GetSection("PfxFileName").Value;
            Announce: bool.Parse(section.GetSection("Announce").Value);
            SslCertPassword = section.GetSection("SslCertPassword").Value;
            MaxPeers = ushort.Parse(section.GetSection("MaxPeers").Value);
            AddressVersion = byte.Parse(section.GetSection("AddressVersion").Value);
            PeerPingInterval = ushort.Parse(section.GetSection("PeerPingInterval").Value);
            AcceptInvalidCerts = bool.Parse(section.GetSection("AcceptInvalidCerts").Value);
            BindAddress = IPAddress.Parse(section.GetSection("BindAddress").Value).ToString();
            MutualAuthentication = bool.Parse(section.GetSection("MutualAuthentication").Value);
            PeerLifetimeInterval = ushort.Parse(section.GetSection("PeerLifetimeInterval").Value);
            SeedList = section.GetSection("SeedList").GetChildren().Select(p => p.Value).ToList();
            AnnounceServer: EndpointBuilder.BuildNewEndPoint(section.GetSection("AnnounceServer").Value);
        }

        public string Network { get; set; }
        public bool Announce { get; set; }
        public IPEndPoint AnnounceServer { get; set; }
        public bool MutualAuthentication { get; set; }
        public bool AcceptInvalidCerts { get; set; }
        public ushort MaxPeers { get; set; }
        public ushort PeerPingInterval { get; set; }
        public ushort PeerLifetimeInterval { get; set; }
        public int Port { get; set; }
        public uint Magic { get; set; }
        public string BindAddress { get; set; }
        public string PfxFileName { get; set; }
        public List<string> SeedList { get; set; }
        public byte AddressVersion { get; set; }
        public string SslCertPassword { get; set; }
    }

    /// <summary>
    ///     RPC settings class.
    /// </summary>
    public class RpcSettings : IRpcSettings
    {

        /// <summary>
        ///     Set attributes
        /// </summary>
        /// <param name="section"></param>
        protected internal RpcSettings(IConfiguration section)
        {
            Guard.NotNull(section, nameof(section));
            Port = int.Parse(section.GetSection("Port").Value);
            BindAddress = IPAddress.Parse(section.GetSection("BindAddress").Value);
        }

        public int Port { get; set; }
        public IPAddress BindAddress { get; set; }
    }
}