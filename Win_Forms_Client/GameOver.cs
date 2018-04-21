using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanki
{
	public partial class GameOver : Form
	{
		public bool exit = false;
		public GameOver(bool swich, string Massage)
		{
			InitializeComponent();
			
			if (swich)
			{
				WatchGame_btn.Visible = false;
				WatchGame_btn.Enabled = false;
			}
			label1.Text = Massage;
		}
		private void ReturnToLobby_btn_Click(object sender, EventArgs e)
		{
			exit = true;
			Close();
		}

		private void WatchGame_btn_Click(object sender, EventArgs e)
		{
			exit = false;
			Close();
		}
	}
}
