using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Catalyst.Helpers.Logger;
using System.Linq;

namespace Catalyst.Helpers.Network
{
    public static class Ip
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetPublicIp()
        {
            var url = "http://checkip.dyndns.org";
            var req = WebRequest.Create(url);
            var resp = req.GetResponse();
            var sr = new StreamReader(resp.GetResponseStream() ?? throw new Exception());
            var response = sr.ReadToEnd().Trim();
            var a = response.Split(':');
            var a2 = a[1].Substring(1);
            var a3 = a2.Split('<');
            var a4 = a3[0];
            return IPAddress.Parse(a4);
        }

        /// <summary>
        ///     Defines a valid range of ports clients can operate on
        ///     We shouldn't let clients run on privileged ports and ofc cant operate over the highest post
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool ValidPortRange(int port)
        {
            if (port < 1025 || port > 65535) return false;//@TODO hook this into guard util and then re-throw the exceptions
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipOrHost"></param>
        /// <returns></returns>
        public static IPAddress BuildIPAddress(string ipOrHost)
        {
            return IPAddress.TryParse(ipOrHost, out IPAddress address)
                ? address
                : System.Net.Dns.GetHostAddressesAsync(ipOrHost).Result.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        ///     given an ip in a string format should validate and return a IPAddress object.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IPAddress ValidateIp(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip));

            IPAddress validIp;
            try
            {
                validIp = IPAddress.Parse(ip);
            }
            catch (ArgumentNullException e)
            {
                LogException.Message("Catalyst.Catalyst.Helpers.Network.Ip.ValidateIp", e);
                throw;
            }
            catch (FormatException e)
            {
                LogException.Message("Catalyst.Catalyst.Helpers.Network.Ip.ValidateIp", e);
                throw;
            }
            catch (SocketException e)
            {
                LogException.Message("Catalyst.Catalyst.Helpers.Network.Ip.ValidateIp", e);
                throw;
            }
            catch (Exception e)
            {
                LogException.Message("Catalyst.Catalyst.Helpers.Network.Ip.ValidateIp", e);
                throw;
            }

            if (validIp == null) throw new ArgumentNullException(nameof(ip));
            return validIp;
        }
    }
}