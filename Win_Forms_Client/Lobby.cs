using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanki
{
	public partial class Lobby : Form
	{
		GameClient GameClient;
		public Lobby(GameClient gameClient)
		{
			InitializeComponent();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			//обновить
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//новая комната
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//подключится
		}
	}
}
