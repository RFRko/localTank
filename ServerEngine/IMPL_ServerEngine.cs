using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace Tanki
{
	/// <summary>
	/// Реализация игрового движка
	/// </summary>
	public class ServerGameEngine : EngineAbs
	{
		private bool mapgen = false;
		/// <summary>
		/// Делегат принимающий сообщение от MessageQueue
		/// </summary>
		public override ProcessMessageHandler ProcessMessage { get; protected set; }
		/// <summary>
		/// Делегат принимающий сообщения от MessageQueue
		/// </summary>
		public override ProcessMessagesHandler ProcessMessages { get; protected set; }
		/// <summary>
		/// Конструктор игрового движка
		/// </summary>
		public ServerGameEngine() : base()
		{
			this.ProcessMessages += MessagesHandler;
			this.ProcessMessage = null;
			this.status = GameStatus.WaitForStart;
		}
		/// <summary>
		/// Конструктор игрового движка
		/// </summary>
		/// <param name="room">Значение Owner базового абстрактного класса</param>
		public ServerGameEngine(IRoom room) : base(room)
		{
			this.ProcessMessages += MessagesHandler;
			this.ProcessMessage = null;

			//var gameRoom = room as IGameRoom;
			//if (gameRoom == null) throw new Exception("Wrong room type");

			//gameRoom.OnNewGameStatus += OnNewGameStatus_Handler;
			this.status = GameStatus.WaitForStart;

			//this.Width = room.GameSetings.MapSize.Width;
			//this.Height = room.GameSetings.MapSize.Height;

			//Это должно быть не тут:  
			// - подписка на OnNewAddresssee происходит в  EngineAbs
			// - GenerateMap - делаем в OnNetProcStarted_EventHandler, 
			//      подписка на OnNetProcStarted происходит в registerdependency рума
			//      OnNetProcStarted - вызывается  в RUN()  NetProcessorAbs
			if (room != null)
			{
				room.OnNewAddresssee += OnNewAddresssee_Handler;
				this.GenerateMap();
			}
		}



		public override void OnRegistered_EventHandler(object Sender, RegEngineData evntData)
		{
			base.OnRegistered_EventHandler(Sender, evntData);

			var gameRoom = Owner as IGameRoom;
			if (gameRoom == null) throw new Exception("Wrong room type");

			gameRoom.OnNewGameStatus += OnNewGameStatus_Handler;
			gameRoom.OnNotifyJoinedPlayer += OnNotifyJoinedPlayer_Handler;
			gameRoom.OnNotifyStartGame += NotifyStartGame_Handler;
		}


		/// <summary>
		/// Ширина игрового поля
		/// </summary>
		public int Width { get { return this.width; } set { this.width = value; } }
		/// <summary>
		/// Высота игрового поля
		/// </summary>
		public int Height { get { return this.height; } set { this.height = value; } }
		/// <summary>
		/// Список всех сущностей на игровом поле
		/// </summary>
		private List<IEntity> objects = new List<IEntity>();
		private int width;
		private int height;
		Random colInd = new Random(DateTime.Now.Millisecond - 15);
		Random rowInd = new Random(DateTime.Now.Millisecond + 20);
		/// <summary>
		/// Список всех танков на игровом поле
		/// </summary>
		private List<ITank> tanks = new List<ITank>();
		/// <summary>
		/// Статус игры
		/// </summary>
		private GameStatus status;
		/// <summary>
		/// Список всех блоков на игровом поле
		/// </summary>
		private List<IBlock> blocks = new List<IBlock>();
		/// <summary>
		/// Список всех пуль на игровом поле
		/// </summary>
		private List<IBullet> bullets = new List<IBullet>();

		/// <summary>
		/// Метод реализирующий проверку, выполнено ли условие победы в игре
		/// </summary>
		/// <returns>Возвращает закончена ли игра</returns>
		private bool CheckWin()
		{
			var t = Owner as IRoom;
			switch (t.GameSetings.GameType)
			{
				case GameType.LastAlive:
					int cnt = 0;
					foreach (var tank in tanks)
					{
						if (tank.Lives > 0)
							cnt++;
					}
					if (cnt == 1) return true;
					break;
				case GameType.Time:
					break;
				case GameType.FlagDefence:
					break;
			}
			return false;
		}
		/// <summary>
		/// Метод реализирующий проверку списка сущностей на наличие убитых
		/// </summary>
		/// <param name="package"> Список сущностей, который подлежит проверке на "мертвых"</param>
		private void CheckAlive(IEnumerable<IPackage> package)
		{
			foreach (var item in package)
			{
				var entity = item.Data as IEntity;
				if (!entity.Is_Alive)
				{
					if (entity is ITank)
					{
						var tank = tanks.FirstOrDefault(t=>t.Tank_ID==(entity as ITank).Tank_ID);
						var tnk = objects.FirstOrDefault(t => (t as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
						if (tank.Lives > 0)
						{
							tank.Position = this.Reload();
							tnk.Position = tank.Position;
							tank.Is_Alive = true;
							tnk.Is_Alive = true;
						}
						else
						{
							tanks.Remove(tank);
							objects.Remove(tnk);
						}
					}
				}
			}

			//foreach (var item in package)   РЕАЛИЗАЦИЯ НЕ ПАРАЛЕЛЬНО, НА ВСЯКИЙ СЛУЧАЙ
			//{
			//	var entity = item.Data as IEntity;
			//	if (!entity.Is_Alive)
			//	{
			//		if (entity is ITank)
			//		{
			//			var tank = entity as ITank;
			//			if (tank.Lives > 0)
			//			{
			//				this.Reload(entity);
			//			}
			//			else
			//			{
			//				tanks.Remove(entity as ITank);
			//				objects.Remove(entity);
			//			}
			//		}
			//		if (entity is IBlock)
			//		{
			//			blocks.Remove(entity as IBlock);
			//			objects.Remove(entity);
			//		}
			//		if (entity is IBullet)
			//		{
			//			bullets.Remove(entity as IBullet);
			//			objects.Remove(entity);
			//		}
			//	}
			//}
		}
		private void CheckBulletAlive()
		{
			for(int i=0;i<this.bullets.Count;i++)
			{
				if (!bullets[i].Is_Alive)
				{
					var blt = objects.FirstOrDefault(b => (b as IBullet)?.Parent_Id == (bullets[i] as IBullet)?.Parent_Id);
					this.bullets.Remove(bullets[i]);
					this.objects.Remove(blt);
				}
			}
		}
		private void MoveAll()
		{
			for(int i=0;i<this.objects.Count;i++)
			{
				if (objects[i].Command == EntityAction.Move)
					this.Move(objects[i]);
			}
			//foreach(var item in this.objects)
			//{
			//	if (item.Command == EntityAction.Move)
			//		this.Move(item);
			//}
		}
		/// <summary>
		/// Реализация делегата ProcessMessagesHandler
		/// </summary>
		/// <param name="list">Список пакетов переданый движку на обработку</param>
		private void MessagesHandler(IEnumerable<IPackage> list)
		{
			if (this.status == GameStatus.Start)
			{
				this.CheckBulletAlive();
				this.CheckAlive(list);
				this.MoveAll();
				//Parallel.ForEach(bullets, x => this.Move(x));
				//if (bullets.Count > 0)
				//{
				//	foreach (var x in bullets) // могу и Эту чепуху сделать паралельной, она на работу не повлияет
				//	{
				//		this.Move(x);
				//	}
				//}
					foreach (var t in list)
					{
						var tmp = t.Data as IEntity;
						if (tmp.Command == EntityAction.Move)
						{
							this.Move(tmp);
						}
						if (tmp.Command == EntityAction.Fire)
						{
							this.Fire(tmp);
						}
					}
				if (this.CheckWin())
				{
					var room = Owner as IRoom;
					room.Status = GameStatus.EndGame;
					this.SendEndGame();
				}
				this.Send();
			}
		}
		/// <summary>
		/// Метод реализирующий обработку "убитой" сущности
		/// </summary>
		/// <param name="entity">"Убитая" сущность</param>
		private void Death(IEntity entity)
		{
			if (entity.Is_Alive == true)
			{
				if (entity is ITank)
				{
					ITank tank = (ITank)objects.FirstOrDefault(t => (t as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
					ITank tnk = tanks.FirstOrDefault(t => t.Tank_ID == (entity as ITank)?.Tank_ID);
					if (tnk.Lives > 0) { tnk.Lives--; tnk.Position = this.Reload(); tank.Position = tnk.Position; }
					else
					{
						this.Destroy(tank);
					}
					tank.Is_Alive = false;
					tnk.Is_Alive = false;
				}
				else if (entity is IBullet)
				{
					IBullet bullet = (IBullet)objects.FirstOrDefault(b => (b as IBullet)?.Parent_Id == (entity as IBullet)?.Parent_Id);
					IBullet blt = bullets.FirstOrDefault(b => b.Parent_Id == (entity as IBullet)?.Parent_Id);
					tanks.FirstOrDefault(t => t.Tank_ID == bullet.Parent_Id).Can_Shoot = true;
					bullet.Is_Alive = false;
					blt.Is_Alive = false;
					bullet.Command = EntityAction.None;
					//this.objects.Remove(bullet);
					//this.bullets.Remove(blt);
				}
				else
				{
					IBlock block = (IBlock)objects.FirstOrDefault(bl => bl?.Position == entity?.Position);
					IBlock blck = blocks.FirstOrDefault(bl => bl.Position == entity.Position);
					block.Is_Alive = false;
					blck.Is_Alive = false;
					this.objects.Remove(block);
					this.blocks.Remove(blck);
				}
			}
		}
		/// <summary>
		/// Метод реализирующий выстрел
		/// </summary>
		/// <param name="entity">Сущность осуществившая выстрел</param>
		private void Fire(IEntity entity)
		{

			ITank tank = (ITank)objects.FirstOrDefault(t => (t as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
			if (tank.Can_Shoot)
			{
				var bullet = new GameObjectFactory().CreateBullet();
				//var bullet = new Bullet();
				var room = Owner as IRoom;
				bullet.Size = room.GameSetings.Bullet_size;
				bullet.Direction = tank.Direction;
				bullet.Parent_Id = tank.Tank_ID;
				bullet.Speed = room.GameSetings.GameSpeed;
				tanks.FirstOrDefault(t => t.Tank_ID == tank.Tank_ID).Can_Shoot = false;
				bullet.Is_Alive = true;
				bullet.Can_Be_Destroyed = true;
				bullet.Command = EntityAction.Move;
				switch (bullet.Direction)
				{
					case Direction.Left:
						bullet.Position = new Rectangle(new Point(tank.Position.Left - bullet.Size - 1, tank.Position.Top + (tank.Size / 2) - (bullet.Size / 2)), new Size(bullet.Size, bullet.Size));
						break;
					case Direction.Right:
						bullet.Position = new Rectangle(new Point(tank.Position.Right + 1, tank.Position.Top + (tank.Size / 2) - (bullet.Size / 2)), new Size(bullet.Size, bullet.Size));
						break;
					case Direction.Up:
						bullet.Position = new Rectangle(new Point(tank.Position.Left + (tank.Size / 2) - (bullet.Size / 2), tank.Position.Top - bullet.Size - 1), new Size(bullet.Size, bullet.Size));
						break;
					case Direction.Down:
						bullet.Position = new Rectangle(new Point(tank.Position.Left + (tank.Size / 2) - (bullet.Size / 2), tank.Position.Bottom + 1), new Size(bullet.Size, bullet.Size));
						break;
				}
				//if(bullets.FirstOrDefault(s=>s.Parent_Id==bullet.Parent_Id)==null)
				bullets.Add(bullet);
				//if(objects.FirstOrDefault(s=>(s as IBullet)?.Parent_Id==bullet.Parent_Id)==null)
				objects.Add(bullet);
			}
			entity.Command = EntityAction.None;
		}
		/// <summary>
		/// Генерация сущностей на игровом поле
		/// </summary>
		private void GenerateMap()
		{
			var rnd = new Random();
			var room = Owner as IRoom;
			this.Width = room.GameSetings.MapSize.Width;
			this.Height = room.GameSetings.MapSize.Height;
			int tankCount = room.Gamers.Count();
			int objectCount = (this.height * this.width) / (10 * room.GameSetings.ObjectsSize * room.GameSetings.ObjectsSize);
			//         foreach (var t in room.Gamers)
			//         {
			//	this.NewGamer(t);
			//}
			while (objectCount > 0)
			{
				var obj = new GameObjectFactory().CreateBlock();
				//var obj = new Block();
				obj.Size = room.GameSetings.ObjectsSize;
				obj.Position = this.Reload();
				obj.Can_Be_Destroyed = true;
				obj.blockType = (BlockType)new Random().Next(0, 4);
				obj.Is_Alive = true;
				blocks.Add(obj);
				objects.Add(obj);
				objectCount--;
			}

			this.mapgen = true;
		}
		/// <summary>
		/// Метод реализирующий движение сущности
		/// </summary>
		/// <param name="entity">Сущность осуществляющая движение</param>
		private void Move(IEntity entity)
		{
			var room = Owner as IRoom;
			//IEntity tmp;
			//if (entity is ITank) tmp = objects.FirstOrDefault(t => (t as ITank).Tank_ID == (entity as ITank).Tank_ID);

			//var tmp = objects.FirstOrDefault(t => t == entity);
			if (entity.Is_Alive)
			{
				if (entity is ITank)
				{
					ITank tank = (ITank)objects.FirstOrDefault(t => (t as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
					tank.Direction = entity.Direction;
					//var tank = tmp as ITank;

					switch (entity.Direction)
					{
						case Direction.Left:
							if (tank.Position.X > 0)
							{
								var pos = new Point(tank.Position.X - room.GameSetings.GameSpeed, tank.Position.Y);
								var rect = new Rectangle(pos, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize));
								var oldrect = tank.Position;
								tank.Position = rect;
								if (!canMove(tank))
									tank.Position = oldrect;
							}
							break;

						case Direction.Right:
							if (tank.Position.X < width)
							{
								var pos = new Point(tank.Position.X + room.GameSetings.GameSpeed, tank.Position.Y);
								var rect = new Rectangle(pos, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize));
								var oldrect = tank.Position;
								tank.Position = rect;
								if (!canMove(tank))
									tank.Position = oldrect;
							}
							break;

						case Direction.Up:
							if (tank.Position.Y > 0)
							{
								var pos = new Point(tank.Position.X, tank.Position.Y - room.GameSetings.GameSpeed);
								var rect = new Rectangle(pos, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize));
								var oldrect = tank.Position;
								tank.Position = rect;
								if (!canMove(tank))
									tank.Position = oldrect;
							}
							break;

						case Direction.Down:

							if (tank.Position.Y < height)
							{
								var pos = new Point(tank.Position.X, tank.Position.Y + room.GameSetings.GameSpeed);
								var rect = new Rectangle(pos, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize));
								var oldrect = tank.Position;
								tank.Position = rect;
								if (!canMove(tank))
									tank.Position = oldrect;
							}
							break;
					}
					tank.Command = EntityAction.None;
				}
				else if (entity is IBullet)
				{
					IBullet bullet = (IBullet)objects.FirstOrDefault(b => (b as IBullet)?.Parent_Id == (entity as IBullet)?.Parent_Id);
					var pos = new Point();
					switch (bullet.Direction)
					{
						case Direction.Left:
							pos = new Point(bullet.Position.X - room.GameSetings.GameSpeed, bullet.Position.Y);
							bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
							break;

						case Direction.Right:
							pos = new Point(bullet.Position.X + room.GameSetings.GameSpeed, bullet.Position.Y);
							bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
							break;

						case Direction.Up:
							pos = new Point(bullet.Position.X, bullet.Position.Y - room.GameSetings.GameSpeed);
							bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
							break;

						case Direction.Down:
							pos = new Point(bullet.Position.X, bullet.Position.Y + room.GameSetings.GameSpeed);
							bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
							break;
					}
					this.HitTarget(bullet);
					if (!this.bulletOnBoard(bullet))
						this.Death(bullet);
				}
			}
		}
		/// <summary>
		/// Предикат определяющий, может ли объект произвести движение
		/// </summary>
		/// <param name="entity"> Объект пытающийся произвести движение</param>
		/// <returns></returns>
		private bool canMove(IEntity entity)
		{
			var list = new List<IEntity>(objects);
			var x = list.FirstOrDefault(s => (s as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
			list.Remove(x);
			var tmp = list.FirstOrDefault(obj => obj.Position.IntersectsWith(entity.Position));

			//if (entity is IBullet)
			//{
			//	tmp = objects.FirstOrDefault(obj => obj.Position.IntersectsWith(entity.Position) && (obj as IBullet)?.Parent_Id != (entity as IBullet)?.Parent_Id);
			//}

			return tmp != null ? false : true;
		}
		/// <summary>
		/// Предикат, проверяющий наличие пули в игровом поле
		/// </summary>
		/// <param name="bullet"> Пуля</param>
		/// <returns></returns>
		private bool bulletOnBoard(IBullet bullet)
		{
			var Board = new Rectangle(0, 0, this.width, this.height);
			if (Board.Contains(bullet.Position))
			{
				return true;
			}
			else
				return false;
		}
		/// <summary>
		/// Реализация попадания пули в другую сущность
		/// </summary>
		/// <param name="bullet">Пуля попавшая в "цель"</param>
		private void HitTarget(IBullet bullet)
		{
			var tmp = objects.FirstOrDefault(tank => tank.Position.IntersectsWith(bullet.Position));
			if (tmp is ITank)
			{
				var tank = tmp as ITank;
				if (tank.Tank_ID != bullet.Parent_Id)
				{
					this.Death(tmp);
					this.Death(bullet);
				}
			}
			else if (tmp is IBlock)
			{
				this.Death(tmp);
				this.Death(bullet);
			}

		}
		/// <summary>
		/// Гененация расположения для сущности
		/// </summary>
		/// <param name="entity">Сущность требующая разположения на игровом поле</param>
		private Rectangle Reload()
		{
			var room = Owner as IRoom;
			Rectangle rect = Rectangle.Empty;
			//entity.Position = Rectangle.Empty;
			while (rect == Rectangle.Empty)
			{
				//Random colInd = new Random(DateTime.Now.Millisecond - 15);
				//Random rowInd = new Random(DateTime.Now.Millisecond + 20);
				int columnIndex = colInd.Next(0, width);
				int rowIndex = rowInd.Next(0, height);
				if (columnIndex >= 0 && columnIndex <= room.GameSetings.MapSize.Width - room.GameSetings.ObjectsSize && rowIndex >= 0 && rowIndex <= room.GameSetings.MapSize.Height - room.GameSetings.ObjectsSize)
				{
					Point p = new Point(rowIndex, columnIndex);
					if (objects.FirstOrDefault(tank => tank.Position.IntersectsWith(new Rectangle(p, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize))) == true) == null)
					{
						rect = new Rectangle(p, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize));
					}
				}
			}
			return rect;
		}
		/// <summary>
		/// Метод реализирующий передачу данных на сендер
		/// </summary>
		public void Send()
		{
			IMap t = new Map();
			t.Blocks = blocks;
			t.Bullets = bullets;
			t.Tanks = tanks;
			IPackage pack = new Package();
			pack.Data = t;
			pack.MesseggeType = MesseggeType.Map;
			var adress = Owner as IRoom;
			Owner.Sender.SendMessage(pack, adress.Gamers);
		}
		/// <summary>
		/// Метод реализирующий уведомление игроков о конце игры
		/// </summary>
		private void SendEndGame()
		{
			IPackage pack = new Package();
			pack.MesseggeType = MesseggeType.EndGame;
			var adress = Owner as IRoom;
			Owner.Sender.SendMessage(pack, adress.Gamers);
		}
		private void SendStartGame()
		{
			IPackage pack = new Package();
			pack.MesseggeType = MesseggeType.StartGame;
			var adress = Owner as IRoom;
			Owner.Sender.SendMessage(pack, adress.Gamers);
		}
		private void Destroy(ITank tank)
		{
			var room = Owner as IRoom;
			var adress = new Addresssee(room.Gamers.FirstOrDefault(g => g.Passport == tank.Tank_ID).RemoteEndPoint);
			IPackage pack = new Package() {Data=tank,MesseggeType=MesseggeType.TankDeath};
			Owner.Sender.SendMessage(pack, adress);
		}

		// Нужно вызывать эту чепуху при новом игроке в комнате, метод ниже мне не подходит, по причине - мне не нужен ендпоинт, мне нужен гуид
		/// <summary>
		/// Метод интегрирующий нового игрока на игровое поле
		/// </summary>
		/// <param name="gamer"> Новый игрок</param>
		public void NewGamer(IGamer gamer)
		{
			var obj = new GameObjectFactory().CreateTank();
			//var obj = new Tank();
			var room = Owner as IRoom;
			obj.Size = room.GameSetings.ObjectsSize;
			obj.Tank_ID = gamer.Passport;
			obj.Name = gamer.Name;
			obj.Lives = 5;
			obj.Is_Alive = true;
			obj.Can_Be_Destroyed = true;
			obj.Can_Shoot = true;
			obj.Direction = Direction.Up;
			obj.Position = this.Reload();
			tanks.Add(obj);
			objects.Add(obj);
		}
		/// <summary>
		/// Обработка события добавления нового игрока
		/// </summary>
		/// <param name="Sender">Объект вызвавший добавление нового игрока</param>
		/// <param name="evntData">Данные о подключении</param>
		public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData)
		{
			if (this.mapgen == false)
				this.GenerateMap();
			var room = Owner as IRoom;
			var gamer = evntData.newAddresssee as IGamer;
			//var gamer = room.Gamers.FirstOrDefault(t => t.RemoteEndPoint == evntData.newAddresssee.RemoteEndPoint);
			this.NewGamer(gamer);
			//this.Send();
		}
		/// <summary>
		/// Обработка события изменения игрового статуса
		/// </summary>
		/// <param name="Sender">Объект изменивший игровой статус</param>
		/// <param name="statusData"> Данные о новом игровом статуса</param>
		public void OnNewGameStatus_Handler(object Sender, GameStatusChangedData statusData)
		{

			//this.status = statusData.newStatus;
			if (statusData.newStatus == GameStatus.Start)
				this.SendStartGame();
		}

		public override void OnBeforNetProcStarted_EventHandler(object Sender, NetProcBeforStartedEvntData evntData)
		{
			//this.GenerateMap();
		}

		public override void OnNetProcStarted_EventHandler(object Sender, NetProcStartedEvntData evntData)
		{
			// Nothing to do required yet
		}

		public override void OnAddressseeHolderFull_Handler(object Sender, AddressseeHolderFullData evntData)
		{

		}

		public void OnNotifyJoinedPlayer_Handler(object Sender, NotifyJoinedPlayerData evntData)
		{
			this.Send();
		}

		public void NotifyStartGame_Handler(Object Sender, NotifyStartGameData evntData)
		{
			var room = Owner as IRoom;
			if (evntData.EnforceStartGame)
			{
				this.status = GameStatus.Start;
				this.SendStartGame();
			}
			// // РЕАЛИЗОВАТЬ рассылку сообщения о старте игры всем клиентам
			//this.Send();

		}

	}
}
