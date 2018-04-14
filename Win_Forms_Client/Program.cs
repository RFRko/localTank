using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanki
{
	static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main()
		{


            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5000); Any дает 0.0.0.0:5000

            ISystemSettings sysSettings = new SystemSettings()
            {
                MaxRoomNumber = 2,
                HostListeningPort = 11001,
                RoomPortMin = 50001,
                RoomPortMax = 50099,
                ClientPortMin = 51001,
                ClientPortMax = 51099
            };


            IIpEPprovider epProvider = new PredefinedIP("", "127.0.0.1");

            DateTime rndSeedBase = DateTime.Now;            
            Int32 rndSeed = rndSeedBase.Hour + rndSeedBase.Minute + rndSeedBase.Second+ rndSeedBase.Millisecond;
            Int32 clientPort = new Random(rndSeed).Next(sysSettings.ClientPortMin, sysSettings.ClientPortMax);

            IPEndPoint lockal_client_EP = epProvider.CreateIPEndPoint(AddressFamily.InterNetwork, clientPort);
            GameClient gameClient = new GameClient(lockal_client_EP);

			gameClient.RUN();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ConnectionForm(gameClient));
		}
	}
}
