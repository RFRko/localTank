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
		public GameSetings gameSetings;
		public bool ok;

		public GameOptionsForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var gameSpeed = (int)numericUpDown1.Value;
			var object_size = (int)numericUpDown2.Value;
			Size mapSize = new Size(
					(int)numericUpDown3.Value,
					(int)numericUpDown4.Value);
			var players_count = (int)numericUpDown5.Value;
			var game_type = (GameType)Enum.Parse(typeof(GameType), comboBox1.SelectedText);

			gameSetings = new GameSetings()
			{
				GameSpeed = gameSpeed,
				ObjectsSize = object_size,
				MapSize = mapSize,
				MaxPlayersCount = players_count,
				GameType = game_type
			};
			ok = true;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ok = false;
			Close();
		}

		private void GameOptionsForm_Load(object sender, EventArgs e)
		{
			var enumlist = Enum.GetNames(typeof(GameType));
			comboBox1.DataSource = enumlist;
		}
	}
}
