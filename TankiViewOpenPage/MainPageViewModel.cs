using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tanki;

namespace TankiViewOpenPage
{
    public class MainPageViewModel
    {
       
        static IPAddress ipaddress = System.Net.IPAddress.Parse("127.0.0.1");
        static int port = 8000;
        static IPEndPoint loginEndPoint = new IPEndPoint(ipaddress, port);

        public IPAddress IpAddress;
        public int Port;
        GameClient startClient = new GameClient(loginEndPoint);



        public void Connect()
        {
            startClient.RUN(new IPEndPoint(IpAddress, Port));
        }
    }
}
