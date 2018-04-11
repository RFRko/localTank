using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tanki
{
	public partial class ConnectionForm : Form
	{
		GameClient gameClient;

		public ConnectionForm(GameClient GameClient)
		{
			gameClient = GameClient;
			InitializeComponent();
			textBox1.Text = "127.0.0.1";
			textBox2.Text = "11001";
		}
		public bool Connect(IPEndPoint point)
		{
			var caption = "Ошибка подключения";
			var message = "Сервер не доступен";
			var buttons = MessageBoxButtons.RetryCancel;
			while (!gameClient.Connect(point))
			{
				var result = MessageBox.Show(message, caption, buttons);
				if (result == DialogResult.Cancel) { Close(); return false; }
			}
			return true;
		}
		private void button2_Click(object sender, EventArgs e)
		{
			string s1 = textBox1.Text;
			string s2 = textBox2.Text;

			if (!string.IsNullOrEmpty(s1)
				&& !string.IsNullOrEmpty(s2))
			{
				IPAddress iP;
				ushort remote_port;
				ushort lockal_port;

				if (!IPAddress.TryParse(s1, out iP))
				{
					label3.Text = "Не коректный Ip";
					return;
				}
				if (!ushort.TryParse(s2, out remote_port) || remote_port > 65535)
				{
					label3.Text = "Не коректный порт сервера";
					return;
				}

				IPEndPoint point = new IPEndPoint(iP, remote_port);
				Lobby lobby = new Lobby(gameClient.Engine as IClientEngine);
				if (Connect(point))
				{
					lobby.Show();
					Hide();
				}
				else { Close(); }
			}
			else label3.Text = "Error: Заполните все поля";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
