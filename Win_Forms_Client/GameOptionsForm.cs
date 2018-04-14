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

		private void GameOptionsForm_Load(object sender, EventArgs e)
		{
			var enumlist = Enum.GetNames(typeof(GameType));
			VictoryCondition_cb.DataSource = enumlist;
		}

		private void CreateRoom_btn_Click(object sender, EventArgs e)
		{
			var gameSpeed = (int)GameSpeed_nud.Value;
			var object_size = (int)ObjectSize_nud.Value;
			Size mapSize = new Size(
					(int)MapWidth_nud.Value,
					(int)MapHeight_nud.Value);
			var players_count = (int)NamberOfPlayer_nud.Value;
			var game_type = (GameType)Enum.Parse(typeof(GameType), VictoryCondition_cb.SelectedItem.ToString());

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

		private void Cancel_btn_Click(object sender, EventArgs e)
		{
			ok = false;
			Close();
		}
	}
}
