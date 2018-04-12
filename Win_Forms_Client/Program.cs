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

            IIpEPprovider epProvider = new PredefinedIP("", "127.0.0.1");
            IPEndPoint lockal_client_EP = epProvider.CreateIPEndPoint(AddressFamily.InterNetwork, 51001);
            GameClient gameClient = new GameClient(lockal_client_EP);

            gameClient.RUN();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ConnectionForm(gameClient));
		}
	}
}
