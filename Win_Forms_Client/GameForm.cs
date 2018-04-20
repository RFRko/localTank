using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanki.Properties;

namespace Tanki
{
	public partial class GameForm : Form
	{
		public GameForm(IClientEngine clientEngine, Size size)
		{
			ClientEngine = clientEngine;
			clientEngine.OnMapChanged += OnMapChangeHandler;
			clientEngine.OnTankDeath += OnTankDeath;
			clientEngine.OnError += ErrorHandler;

			onMapChanged += onMapChangedProc;
			DeathAnimation += onDeathAnimation;

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

			Blocks = new Dictionary<BlockType, Bitmap>
			{
				{ BlockType.Brick, Resources.wall },
				{ BlockType.Brick2, Resources.wall1 },
				{ BlockType.Concrete, Resources.wall3 },
				{ BlockType.Tree, Resources.tree },
				{ BlockType.Health, Resources.life }
			};

			ExplImages = new List<Bitmap>()
			{
				Resources.explosion1,
				Resources.explosion2,
				Resources.explosion3,
				Resources.explosion4,
				Resources.explosion5,
				Resources.explosion6,
				Resources.explosion7,
				Resources.explosion8,
				Resources.explosion9,
				Resources.explosion10,
				Resources.explosion11,
				Resources.explosion12,
				Resources.explosion13,
				Resources.explosion14
			};


			InitializeComponent();
			this.ClientSize = size;
			this.BackColor = Color.Black;
			Message.ForeColor = Color.White;
			Message.TextAlign = ContentAlignment.MiddleCenter;
			Message.Font = new Font("Comic Sans", 20);
			Message.Text = "Wating other players..";
		}

		private IClientEngine ClientEngine;
		private IMap Map;
		private Dictionary<Direction, Bitmap> Enemies;
		private Dictionary<Direction, Bitmap> Player;
		private Dictionary<BlockType, Bitmap> Blocks;
		private List<Bitmap> ExplImages;

		private Action<IMap> onMapChanged;
		private Action<ITank> DeathAnimation;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Map == null) return;

			Draw_Blocks(e);
			Draw_Bullets(e);
			Draw_Tanks(e);
		}

		private void Draw_Bullets(PaintEventArgs e)
		{
			foreach (var i in Map.Bullets)
				e.Graphics.DrawImage(Resources.Bullet, i.Position);
		}
		private void Draw_Blocks(PaintEventArgs e)
		{
			foreach (var i in Map.Blocks)
				e.Graphics.DrawImage(Blocks[i.blockType], i.Position);
		}
		private void Draw_Tanks(PaintEventArgs e)
		{
			StringFormat sf = new StringFormat();
			var myPassport = ClientEngine.GetPassport();

			foreach (var i in Map.Tanks)
			{

				if (i.Tank_ID == myPassport)
					e.Graphics.DrawImage(Player[i.Direction], i.Position);
				else
					e.Graphics.DrawImage(Enemies[i.Direction], i.Position);

				Draw_HP_Lines(e);
				Draw_Lifes(e);
				Draw_Names(e);
			}
		}
		private void Draw_HP_Lines(PaintEventArgs e)
		{
			SolidBrush Brush;

			foreach (var i in Map.Tanks)
			{
				var step = i.Size / ClientEngine.MaxHealthPoints;
				var size = new Size(step * i.HelthPoints, i.Size / 5);
				var pnt = new Point(i.Position.Location.X, i.Position.Location.Y + i.Size + (i.Size / 5));
				var hpRect = new Rectangle(pnt, size);
				var border = new Rectangle(pnt, new Size(step * ClientEngine.MaxHealthPoints, i.Size / 5));

				if (i.HelthPoints > ClientEngine.MaxHealthPoints)
					Brush = new SolidBrush(Color.Purple);
				else if (i.HelthPoints <= ClientEngine.MaxHealthPoints / 4)
					Brush = new SolidBrush(Color.DarkRed);
				else if (i.HelthPoints <= ClientEngine.MaxHealthPoints / 3)
					Brush = new SolidBrush(Color.Red);
				else if (i.HelthPoints <= ClientEngine.MaxHealthPoints / 2)
					Brush = new SolidBrush(Color.OrangeRed);
				else if (i.HelthPoints <= ClientEngine.MaxHealthPoints / 1.5)
					Brush = new SolidBrush(Color.Orange);
				else Brush = new SolidBrush(Color.Green);

				e.Graphics.FillRectangle(Brush, hpRect);
				e.Graphics.DrawRectangle(new Pen(Color.White), border);
			}
		}
		private void Draw_Names(PaintEventArgs e)
		{
			var sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;
			string name;

			foreach (var i in Map.Tanks)
			{
				if (i.Name.Length > 10)
				{
					name = i.Name.Substring(0, 10);
					name += "..";
				}
				else name = i.Name;

				e.Graphics.DrawString
				(
					name,
					new Font("Comic Sans", i.Size / 4),
					new SolidBrush(Color.Yellow),
					new Point
					(
						i.Position.Location.X + i.Size / 2,
						i.Position.Location.Y - i.Size / 3
					),
					sf
				);
			}
		}
		private void Draw_Lifes(PaintEventArgs e)
		{
			foreach (var i in Map.Tanks)
			{
				e.Graphics.DrawString
				(
					i.Lives.ToString(),
					new Font("Comic Sans", i.Size / 5),
					new SolidBrush(Color.Yellow),
					new Point
					(
						i.Position.Location.X - i.Size / 4,
						i.Position.Location.Y + i.Size + (i.Size / 5)
					)
				);
			}
		}


		private void OnMapChangeHandler(object Sender, GameStateChangeData data)
		{
			try
			{
				this.Invoke(onMapChanged, data.newMap);
			}
			catch (Exception e)
			{
				
			}
		}
		private void onMapChangedProc(IMap map)
		{
			Map = map;
			Invalidate();
		}
		private void OnTankDeath(object Sender, DestroyableTank data)
		{
			this.Invoke(DeathAnimation, data.tankToDestroy);
		}
		private void onDeathAnimation(ITank tankToDestroy)
		{

		}
		private void onGameOwer (object Sender, ErrorData data)
		{
			Message.Text = data.errorText;
		}
		private void onDeath(object Sender, ErrorData data)
		{
			Message.ForeColor = Color.Red;
			Message.Text = data.errorText;
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
            if (ClientEngine.Entity == null) return;

            var newEntity = ClientEngine.Entity;
			switch (e.KeyCode)
			{
				case Keys.Left:
					{
						newEntity.Direction = Direction.Left;
						newEntity.Command = EntityAction.Move;
						ClientEngine.Entity = newEntity;
						break;
					}
				case Keys.Right:
					{
						newEntity.Direction = Direction.Right;
						newEntity.Command = EntityAction.Move;
						ClientEngine.Entity = newEntity;
						break;
					}
				case Keys.Up:
					{
						newEntity.Direction = Direction.Up;
						newEntity.Command = EntityAction.Move;
						ClientEngine.Entity = newEntity;
						break;
					}
				case Keys.Down:
					{
						newEntity.Direction = Direction.Down;
						newEntity.Command = EntityAction.Move;
						ClientEngine.Entity = newEntity;
						break;
					}
				case Keys.Space:
					{
						newEntity.Command = EntityAction.Fire;
						ClientEngine.Entity = newEntity;
						break;
					}
				default: return;
			}
		}
		private void GameForm_KeyUp(object sender, KeyEventArgs e)
		{
            if (ClientEngine.Entity == null) return;

			var newEntity = ClientEngine.Entity;
			newEntity.Command = EntityAction.None;
			ClientEngine.Entity = newEntity;
		}

		private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			ClientEngine.StopGame();
		}
	}
}
