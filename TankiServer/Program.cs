using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
    class Program
    {
        static void Main(string[] args)
        {

            ISystemSettings sysSettings = new SystemSettings()
            {
                MaxRoomNumber = 2,
                HostListeningPort = 11001,
                RoomPortMin = 50001,
                RoomPortMax = 50099,
                ClientPortMin = 51001,
                ClientPortMax = 51099
            };


            //IIpEPprovider HostEPprovider = new FirstAvailableIP();

            IIpEPprovider HostEPprovider = new PredefinedIP("","127.0.0.1");

            IServer Srv = new GameServer(HostEPprovider,sysSettings);

            //Srv.AddRoom(null, Guid.NewGuid());
            //Srv.AddRoom(null, Guid.NewGuid());


            Srv.RUN();




        }
    }
}
