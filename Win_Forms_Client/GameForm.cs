using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanki.Properties;

namespace Tanki
{
	public partial class GameForm : Form
	{
		private IClientEngine ClientEngine;
		private IMap Map;
		private Dictionary<Direction, Bitmap> Enemies;
		private Dictionary<Direction, Bitmap> Player;

		public GameForm(IClientEngine clientEngine, Size size)
		{
			ClientEngine = clientEngine;
			clientEngine.OnMapChanged += OnMapChangeHandler;
			onMapChanged += onMapChangedProc;
			clientEngine.OnError += ErrorHandler;

			Enemies = new Dictionary<Direction, Bitmap>()
			{
				{ Direction.Down, Resources.enemy_down },
				{ Direction.Left, Resources.enemy_left },
				{ Direction.Right, Resources.enemy_right },
				{ Direction.Up, Resources.enemy_up },
			};

			Player = new Dictionary<Direction, Bitmap>()
			{
				{ Direction.Down, Resources.player_down },
				{ Direction.Left, Resources.player_left },
				{ Direction.Right, Resources.player_right },
				{ Direction.Up, Resources.player_up },
			};

			InitializeComponent();
			this.ClientSize = size;
			this.BackColor = Color.Black;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
            if (Map == null) return;

			var myPassport = ClientEngine.GetPassport();

			foreach (var i in Map.Tanks)
			{
				if (i.Tank_ID == myPassport)
				{
					e.Graphics.DrawImage(Player[i.Direction], i.Position);
				}
				else 
				{
					e.Graphics.DrawImage(Enemies[i.Direction], i.Position);
				}
				e.Graphics.DrawString
					(
						i.Name,
						new Font("Comic Sans", 15),
						new SolidBrush(Color.Yellow),
						new PointF
						(
							i.Position.Location.X + 10, 
							i.Position.Location.Y - 10
						)
					);
			}

			foreach (var i in Map.Bullets)
				e.Graphics.DrawImage(Resources.Bullet, i.Position);

			foreach (var i in Map.Blocks)
				e.Graphics.DrawImage(Resources.wall, i.Position);
		}

		private void OnMapChangeHandler(object Sender, GameStateChangeData data)
		{
			this.Invoke(onMapChanged, data.newMap);
		}

		private Action<IMap> onMapChanged;

		private void onMapChangedProc(IMap map)
		{
			Map = map;
			Invalidate();
		}

		private void ErrorHandler(object sender, ErrorData e)
		{
			var caption = "Ошибка";
			var message = e.errorText;
			var buttons = MessageBoxButtons.OK;
			MessageBox.Show(message, caption, buttons);
			Close();
		}

		private void GameForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
					{
						ClientEngine.Entity.Direction = Direction.Left;
						ClientEngine.Entity.Command = EntityAction.Move;
						break;
					}
				case Keys.Right:
					{
						ClientEngine.Entity.Direction = Direction.Right;
						ClientEngine.Entity.Command = EntityAction.Move;
						break;
					}
				case Keys.Up:
					{
						ClientEngine.Entity.Direction = Direction.Up;
						ClientEngine.Entity.Command = EntityAction.Move;
						break;
					}
				case Keys.Down:
					{
						ClientEngine.Entity.Direction = Direction.Down;
						ClientEngine.Entity.Command = EntityAction.Move;
						break;
					}
				case Keys.Space:
					{
						ClientEngine.Entity.Command = EntityAction.Fire;
						break;
					}
				default: return;
			}
		}

		private void GameForm_KeyUp(object sender, KeyEventArgs e)
		{
			ClientEngine.Entity.Command = EntityAction.None;
		}
	}
}
