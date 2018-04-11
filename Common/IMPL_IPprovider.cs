using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    public abstract class IPproviderAbs : IIpProvider
    {
        public IPproviderAbs() { }
        protected IPAddress ipAddress { get; set; }
        protected Int32 Port { get; set; }

        public abstract IPEndPoint CreateIPEndPoint();
    }


    public class FirstAvailableIP : IPproviderAbs
    {
        public FirstAvailableIP(AddressFamily addrFamily, Int32 port) : base() { _addrFamily = addrFamily; Port = port; }

        private AddressFamily _addrFamily;
        public override IPEndPoint CreateIPEndPoint()
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

            return new IPEndPoint(ipAddress.Address, Port);
        }
    }


    public class PredefinedIP : IPproviderAbs
    {
        public PredefinedIP(String IpAddr, AddressFamily addrFamily, Int32 port) : base()
        {
            _PredefinedIp = IpAddr;
            _addrFamily = addrFamily;
            Port = port;
        }

        private String _PredefinedIp;
        private AddressFamily _addrFamily;
        public override IPEndPoint CreateIPEndPoint()
        {
            ipAddress = IPAddress.Parse(_PredefinedIp);

            if (ipAddress.AddressFamily != _addrFamily)
            {
                throw new Exception(_PredefinedIp + " is not "+_addrFamily.ToString());
            }

            IPHostEntry HostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var a in HostEntry.AddressList)
            {

            }

            var foundInHost = from a in HostEntry.AddressList where a == ipAddress select a;
            if (foundInHost.Count() == 0)
                throw new Exception(_PredefinedIp + " is out of host addresses");

            return new IPEndPoint(ipAddress.Address, Port);
        }
    }

    //Не реализован
    public class IPfromConfigFile : IPproviderAbs
    {
        public IPfromConfigFile(Int32 Port):base() { }

        public override IPEndPoint CreateIPEndPoint()
        {
            throw new NotImplementedException();
        }
    }

}
