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
        public PredefinedIP(String IpAddr) : base()
        {
            _PredefinedIp = IpAddr;
        }

        private String _PredefinedIp;
        private AddressFamily _addrFamily;
        public override IPEndPoint CreateIPEndPoint(AddressFamily ipAddrFamily, int port)
        {
            ipAddress = IPAddress.Parse(_PredefinedIp);

            if (ipAddress.AddressFamily != _addrFamily)
            {
                throw new Exception(_PredefinedIp + " is not "+_addrFamily.ToString());
            }

            IPHostEntry HostEntry = Dns.GetHostEntry(Dns.GetHostName());

            var foundInHost = from a in HostEntry.AddressList where a == ipAddress select a;
            if (foundInHost.Count() == 0)
                throw new Exception(_PredefinedIp + " is out of host addresses");

            return new IPEndPoint(ipAddress, port);
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
