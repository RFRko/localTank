using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public abstract class IPEndPointProviderAbs : IIpEPprovider
    {
        public IPEndPointProviderAbs() { }
        protected IPAddress ipAddress { get; set; }

        public abstract IPEndPoint CreateIPEndPoint(AddressFamily ipAddrFamily, int port);
    }


    public class FirstAvailableIP : IPEndPointProviderAbs
    {
        public FirstAvailableIP() : base() {}

        private AddressFamily _addrFamily;
        public override IPEndPoint CreateIPEndPoint(AddressFamily ipAddrFamily, int port)
        {
            IPHostEntry HostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach(var a in HostEntry.AddressList)
            {
                if (a.AddressFamily == _addrFamily)
                {
                    ipAddress = a;
                    break;
                }

            }

            return new IPEndPoint(ipAddress, port);
        }
    }


    public class PredefinedIP : IPEndPointProviderAbs
    {
        public PredefinedIP(String IpV6addr, String IpV4addr) : base()
        {
            _PredefinedIpV6 = IpV6addr;
            _PredefinedIpV4 = IpV4addr;

        }

        private String _PredefinedIpV6;
        private String _PredefinedIpV4;
        
        public override IPEndPoint CreateIPEndPoint(AddressFamily ipAddrFamily, int port)
        {
            IPAddress addr = null;
            String strAddr = null;
            IPEndPoint res = null;

            if (ipAddrFamily == AddressFamily.InterNetworkV6)
                strAddr = _PredefinedIpV6;

            if (ipAddrFamily == AddressFamily.InterNetwork)
                strAddr = _PredefinedIpV4;

            if (strAddr != null && strAddr != String.Empty)
            {
                if (!IPAddress.TryParse(strAddr, out addr))
                    throw new Exception(strAddr + "Not Valid " + ipAddrFamily.ToString());

                IPHostEntry HostEntry = Dns.GetHostEntry(Dns.GetHostName());

                //var foundInHost = from a in HostEntry.AddressList where a == ipAddress select a;
                //if (foundInHost.Count() == 0)
                //    throw new Exception(strAddr + " is out of host addresses");

                res = new IPEndPoint(addr, port);

            }

            return res;
        }
    }

    //Не реализован
    public class IPfromConfigFile : IPEndPointProviderAbs
    {
        public IPfromConfigFile():base() { }

        public override IPEndPoint CreateIPEndPoint(AddressFamily ipAddrFamily, int port)
        {
            throw new NotImplementedException();
        }
    }

}
