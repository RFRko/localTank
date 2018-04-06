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
		Client client;
		public ConnectionForm()
		{
			InitializeComponent();
			textBox1.Text = "127.0.0.1";
			textBox2.Text = "5000";
			textBox3.Text = "5001";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string s1 = textBox1.Text;
			string s2 = textBox2.Text;
			string s3 = textBox3.Text;

			if (!string.IsNullOrEmpty(s1)
				&& !string.IsNullOrEmpty(s2)
				&& !string.IsNullOrEmpty(s3))
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
				if (!ushort.TryParse(s3, out lockal_port) || lockal_port > 65535)
				{
					label3.Text = "Не коректный локальный порт";
					return;
				}

				IPEndPoint point = new IPEndPoint(iP, remote_port);

				client = new Client(point, lockal_port);

				if (Connect())
				{
					Lobby lobby = new Lobby(client);
					lobby.Show();
					Hide();
				}
			}
			else label3.Text = "Error: Заполните все поля";
		}

		public bool Connect()
		{
			var caption = "Ошибка подключения";
			var message = "Сервер не доступен";
			var buttons = MessageBoxButtons.RetryCancel;
			while (!client.RUN())
			{
				var result = MessageBox.Show(message, caption, buttons);
				if (result == DialogResult.Cancel) { Close(); return false; }
			}
			return true;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
