using Common;
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
	public partial class GameForm : Form
	{
		bool firstmap = true;
		private IClientEngine ClientEngine;

		public GameForm(IClientEngine clientEngine, Size size)
		{
			InitializeComponent();
			this.Size = size;
			ClientEngine = clientEngine;
			clientEngine.OnMapChanged += OnMapChangeHandler;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		private void OnMapChangeHandler(object Sender, GameStateChangeData data)
		{
			if (firstmap)
			{
				var map = data.newMap;
				var myTank = map.Tanks.First((i) => { return i.Tank_ID == ClientEngine.GetPassport(); });
				var should_be_drawn = new List<Game_Rect>();


				firstmap = false;
			}
			else
			{

			}
		}
	}
	public class Game_Rect
	{

	}

	public class Tank_Rect : Game_Rect
	{
		public PictureBox rectangle { get; set; }
		public Label label { get; set; }

		public Tank_Rect(ITank tank)
		{
			rectangle.Location = tank.Position.Location;
			rectangle.Size = new Size (tank.Size, tank.Size);
			label.Text = tank.Name;

			switch (tank.Direction)
			{
				case Direction.Down:
					{
						rectangle.Image = Tanki.Properties.Resources.enemy_down;
						break;
					}
				case Direction.Left:
					{
						rectangle.Image = Tanki.Properties.Resources.enemy_left;
						break;
					}
				case Direction.Right:
					{
						rectangle.Image = Tanki.Properties.Resources.enemy_right;
						break;
					}
				case Direction.Up:
					{
						rectangle.Image = Tanki.Properties.Resources.enemy_up;
						break;
					}
				default: throw new Exception("Undefine Direction");
			}
		}
	}
}
