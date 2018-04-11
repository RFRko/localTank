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
	public partial class GameOptionsForm : Form
	{
		IGameSetings gameSetings;
		bool ok;

		public GameOptionsForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var gameSpeed = (int)numericUpDown1.Value;
			var object_size = (int)numericUpDown2.Value;
			Size MapSize = new Size(
					(int)numericUpDown3.Value,
					(int)numericUpDown4.Value);
			var players_count = numericUpDown5.Value;
			var game_type = Enum.Parse(typeof(GameType), comboBox1.SelectedText);

			if (gameSpeed != 0 &&
				object_size != 0 &&
				MapSize.Height != 0 &&
				MapSize.Width != 0 &&
				players_count < 0)
			{
				if (gameSpeed < 10) { label6.Text = "Скрорость должна быть меньше 10"; return; }
				if (object_size < 10) { label6.Text = "Размер должен быть меньше 10"; return; }
				if (players_count < 2) { label6.Text = "Ироков должно быть больше 2"; return; }
			}
			else label6.Text = "Заполните все поля";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void GameOptionsForm_Load(object sender, EventArgs e)
		{
			var enumlist = Enum.GetNames(typeof(GameType));
			comboBox1.DataSource = enumlist;
		}
	}
}
