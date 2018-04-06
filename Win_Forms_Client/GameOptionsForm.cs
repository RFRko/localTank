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
		public GameOptionsForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var speed = numericUpDown1.Value;
			var object_size = numericUpDown2.Value;
			var x = numericUpDown3.Value;
			var y = numericUpDown4.Value;
			var players = numericUpDown5.Value;
			var game_type = comboBox1.SelectedItem.ToString();


			if (speed != 0 
				&& object_size != 0 
				&& x != 0
				&& y != 0
				&& players != 0 && 
				!string.IsNullOrEmpty(game_type))
			{
				if (players < 2)
				{
					label6.Text = "Игроков должно быть больше одного";
					return;
				}
			}
			else label6.Text = "Error: Заполните все поля";

			gameSetings = new GameSetings()
			{
				GameSpeed = (int)speed,
				ObjectsSize = (int)object_size,
				MapSize = new Size((int)x, (int)y),
				MaxPlayersCount = (int)players,
				GameType = (GameType)Enum.Parse(typeof(GameType), game_type)
			};
			Close();
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
