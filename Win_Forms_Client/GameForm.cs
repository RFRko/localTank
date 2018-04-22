using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
			clientEngine.onGameStart += OnGameStart;
			clientEngine.onDeath += onDeath;
			clientEngine.onEndGame += OnEndGame;

			Endgame += _EndGame;
			Ondeath += _onDeath;
			GameStart += _GameStart;
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
				{ BlockType.Health, Resources.life2 }
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

			Message.Text = "Wating for other players..";
			Message.Location = new Point(
				ClientSize.Width / 2 - Message.Size.Width / 2,
				ClientSize.Height / 2 - Message.Size.Height);

			ToLobby_btn.Visible = false;
			ToLobby_btn.Enabled = false;

			WatchGame_btn.Visible = false;
			WatchGame_btn.Enabled = false;
			WatchGame_btn.Location = new Point(
				ClientSize.Width / 2 + 5, 
				ClientSize.Height / 2 + 30);
		}

		private IClientEngine ClientEngine;
		private IMap Map;
		private Dictionary<Direction, Bitmap> Enemies;
		private Dictionary<Direction, Bitmap> Player;
		private Dictionary<BlockType, Bitmap> Blocks;
		private List<Bitmap> ExplImages;

		private Action<IMap> onMapChanged;
		private Action<ITank> DeathAnimation;
		private Action<string> GameStart;
		private Action<string> Ondeath;
		private Action<string> Endgame;

		private bool animation = false;
		private ITank deadTank = null;
		private int imagecount;
		private int animationSpeed = 20;
		private int dillay;


		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Map == null) return;

			if (animation)
			{
				if (imagecount == 0) animation = false;
				if (animationSpeed % animationSpeed == 0)
				{
					e.Graphics.DrawImage(ExplImages[imagecount], deadTank.Position.Location.X, deadTank.Position.Location.Y, deadTank.Size, deadTank.Size);
					imagecount--;
				}
				else animationSpeed--;
			}

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
			int step;

			foreach (var i in Map.Tanks)
			{
				if (i.HelthPoints > ClientEngine.MaxHealthPoints)
					step = i.Size / i.HelthPoints;
				else step = i.Size / ClientEngine.MaxHealthPoints;
				var size = new Size((step * i.HelthPoints) + 1, i.Size / 5);
				var pnt = new Point(i.Position.Location.X, i.Position.Location.Y + i.Size + (i.Size / 5));
				var hpRect = new Rectangle(pnt, size);
				Rectangle border = new Rectangle(pnt, new Size(i.Size, i.Size / 5));

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
					new Font("Comic Sans MS", i.Size / 4),
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


		private void OnEndGame(object Sender, ErrorData data)
		{
			this.Invoke(Endgame, data.errorText);
		}
		private void _EndGame(string data)
		{
			Message.Text = "Game Ower. Winer: " + data;
			Message.ForeColor = Color.Green;
			Message.Location = new Point(
				ClientSize.Width / 2 - Message.Size.Width / 2,
				ClientSize.Height / 2 - Message.Size.Height);
			Message.Visible = true;

			ToLobby_btn.Click += (i, s) => { this.Close(); };
			ToLobby_btn.Enabled = true;
			ToLobby_btn.Visible = true;
			ToLobby_btn.Location = new Point(
				ClientSize.Width / 2 - ToLobby_btn.Size.Width / 2,
				ClientSize.Height / 2 + 30);

			WatchGame_btn.Enabled = false;
			WatchGame_btn.Visible = false;
		}
		private void OnGameStart(object Sender, ErrorData data)
		{
			try
			{
				this.Invoke(GameStart, data.errorText);
			}
			catch (Exception)
			{

			}
		}
		private void _GameStart(string data)
		{
			Message.Text = "";
			Message.Visible = false;
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
			imagecount = ExplImages.Count - 1;
			dillay = (imagecount * 100) * 2;

			deadTank = tankToDestroy;
			animation = true;
		}
		private void onDeath(object Sender, ErrorData data)
		{
			this.Invoke(Ondeath, data.errorText);
		}
		private void _onDeath(string message)
		{
			Message.Text = "You are dead";
			Message.ForeColor = Color.Red;
			Message.Location = new Point(
				ClientSize.Width / 2 - Message.Size.Width / 2,
				ClientSize.Height / 2 - Message.Size.Height);
			Message.Visible = true;
			
			ToLobby_btn.Click += (i, s) => { this.Close(); };
			ToLobby_btn.Enabled = true;
			ToLobby_btn.Visible = true;
			ToLobby_btn.Location = new Point(
				ClientSize.Width / 2 - ToLobby_btn.Size.Width - 5,
				ClientSize.Height / 2 + 30);


			WatchGame_btn.Enabled = true;
			WatchGame_btn.Visible = true;
			WatchGame_btn.Click += (i, s) => 
			{
				ToLobby_btn.Visible = false;
				WatchGame_btn.Visible = false;
				ToLobby_btn.Enabled = false;
				WatchGame_btn.Enabled = false;
				Message.Text = "";
				Message.Visible = false;
			};
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
			//ClientEngine.StopGame();
		}
	}
}
