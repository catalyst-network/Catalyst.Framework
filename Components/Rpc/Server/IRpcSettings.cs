namespace ADL.Rpc.Server
{
    public interface IRpcSettings
    {
        string BindAddress { get; set; }
        ushort Port { get; set; }
        string SslCert { get; set; }
        string SslCertPassword { get; set; }
    }
}