using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5001);
			GameClient gameClient = new GameClient(endPoint);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ConnectionForm(gameClient));
		}
	}
}
