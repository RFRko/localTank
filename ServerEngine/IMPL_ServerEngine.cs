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
		//private List<IEntity> objects = new List<IEntity>();
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

		private List<ITank> DeadCache = new List<ITank>();

		/// <summary>
		/// Метод реализирующий проверку, выполнено ли условие победы в игре
		/// </summary>
		/// <returns>Возвращает закончена ли игра</returns>
		private bool CheckWin(out IEntity winner)
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
                    if (cnt == 1)
                    {
                        winner = tanks.FirstOrDefault(z=>z.Lives>0);
                        return true;
                    }
					break;
				case GameType.FragPerTime:
					break;
				case GameType.FlagDefence:
					break;
			}
            winner = null;
			return false;
		}
		private void MoveAll()
		{
            try
            {
                Parallel.ForEach(bullets, x => this.Move(x));
            }
            catch (AggregateException)
            {

            }
            
            //         for (int i = 0; i < this.bullets.Count; i++)
            //{
            //	if (bullets[i].Command == EntityAction.Move)
            //		this.Move(bullets[i]);
            //}
        }
		/// <summary>
		/// Реализация делегата ProcessMessagesHandler
		/// </summary>
		/// <param name="list">Список пакетов переданый движку на обработку</param>
		private void MessagesHandler(IEnumerable<IPackage> list)
		{

			//foreach(var i in list)
			//{
			//	if (i.MesseggeType == MesseggeType.RequestLogOff)
			//		Disconect(i.Sender_Passport);
			//}

			object locker = new object();
			if (this.status == GameStatus.Start)
			{
                IEntity winner;
				lock (locker)
				{
					list = from t in list.AsParallel()
						   where !(from dead in DeadCache select dead.Tank_ID).Contains((t.Data as ITank).Tank_ID) 
						   select t;

                   this.MoveAll();
                    if (colInd.Next(1, 1000) == 555) this.HealthBlock();
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
					if (this.CheckWin(out winner))
					{
						var room = Owner as IRoom;
                        this.status = GameStatus.EndGame;
						room.Status = GameStatus.EndGame;
						this.SendEndGame(winner);
					}
					this.Send();
				}
			}
		}

		private void Disconect(Guid passprt)
		{
			//Игрок закрыл игровую форму и вернулся в лоби
		}

		/// <summary>
		/// Метод реализирующий обработку "убитой" сущности
		/// </summary>
		/// <param name="entity">"Убитая" сущность</param>
		private void Death(IEntity entity)
		{

			if (entity is ITank)
			{
				ITank tnk = tanks.FirstOrDefault(t => t.Tank_ID == (entity as ITank)?.Tank_ID);
				if (tnk.HelthPoints > 0) tnk.HelthPoints--;
				if(tnk.HelthPoints==0)
				{
					if (tnk.Lives > 1)
					{
                        tnk.Position = this.Reload();
                        tnk.Lives--;
						tnk.HelthPoints = 5;
					}
                    else if (tnk.Lives == 1)
                    {
                        tnk.Lives--;
                    }
                    if(tnk.Lives<=0)
                    {
                        this.DeadCache.Add(tnk);
                        this.Destroy(tnk);
                        this.tanks.Remove(tnk);
                    }
				}
			}
			else if (entity is IBullet)
			{
				IBullet blt = bullets.FirstOrDefault(b => b.Parent_Id == (entity as IBullet)?.Parent_Id);
				tanks.FirstOrDefault(t => t.Tank_ID == blt.Parent_Id).Can_Shoot = true;
				//this.bullets.Remove(blt);
			}
			else
			{
				//IBlock block = (IBlock)objects.FirstOrDefault(bl => bl?.Position == entity?.Position);
				IBlock blck = blocks.FirstOrDefault(bl => bl.Position == entity.Position);
				if (blck.Can_Be_Destroyed)
				{
                    if (blck.HelthPoints > 0)  blck.HelthPoints--;
                    if (blck.HelthPoints == 0) this.blocks.Remove(blck);
                }
			}

		}
		/// <summary>
		/// Метод реализирующий выстрел
		/// </summary>
		/// <param name="entity">Сущность осуществившая выстрел</param>
		private void Fire(IEntity entity)
		{

			//ITank tank = (ITank)objects.FirstOrDefault(t => (t as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
			ITank tank = tanks.FirstOrDefault(t => t?.Tank_ID == (entity as ITank)?.Tank_ID);
			if (tank.Can_Shoot)
			{
				var bullet = new GameObjectFactory().CreateBullet();
				//var bullet = new Bullet();
				var room = Owner as IRoom;
				bullet.Size = room.GameSetings.Bullet_size;
				bullet.Direction = tank.Direction;
				bullet.Parent_Id = tank.Tank_ID;
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
				//objects.Add(bullet);
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
			//int tankCount = room.Gamers.Count();
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
				obj.blockType = (BlockType)colInd.Next(0, 4);
				switch (obj.blockType)
				{
					case BlockType.Brick:obj.HelthPoints = 2;
						break;
					case BlockType.Brick2:obj.HelthPoints = 2;
						break;
					case BlockType.Concrete:obj.Can_Be_Destroyed = false;
						break;
					case BlockType.Tree:obj.HelthPoints = 1;
						break;
					default:
						break;
				}
				 //obj.Is_Alive = true;
				blocks.Add(obj);
				//objects.Add(obj);
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
			if (entity is ITank)
			{
				ITank tank = tanks.FirstOrDefault(t => t?.Tank_ID == (entity as ITank)?.Tank_ID);
				tank.Direction = entity.Direction;
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
						if (tank.Position.X < width - tank.Position.Width)
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

						if (tank.Position.Y < height - tank.Position.Height)
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
                var hp = blocks.FirstOrDefault(obj => obj.Position.IntersectsWith(tank.Position) == true);
                if (hp != null)
                {
                    blocks.Remove(hp);
                    tank.HelthPoints = 6;
                }
				tank.Command = EntityAction.None;
			}
			else if (entity is IBullet)
			{
				IBullet bullet = bullets.FirstOrDefault(b => b?.Parent_Id == (entity as IBullet)?.Parent_Id);
				var pos = new Point();
				switch (bullet.Direction)
				{
					case Direction.Left:
						pos = new Point(bullet.Position.X - room.GameSetings.GameSpeed-3, bullet.Position.Y);
						bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
						break;

					case Direction.Right:
						pos = new Point(bullet.Position.X + room.GameSetings.GameSpeed+3, bullet.Position.Y);
						bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
						break;

					case Direction.Up:
						pos = new Point(bullet.Position.X, bullet.Position.Y - room.GameSetings.GameSpeed-3);
						bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
						break;

					case Direction.Down:
						pos = new Point(bullet.Position.X, bullet.Position.Y + room.GameSetings.GameSpeed+3);
						bullet.Position = new Rectangle(pos, new Size(room.GameSetings.Bullet_size, room.GameSetings.Bullet_size));
						break;
				}
				this.HitTarget(bullet);
				if (!this.bulletOnBoard(bullet))
                {
                    this.Death(bullet);
                    bullets.Remove(bullet);
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
			var list = new List<IEntity>();
			list.AddRange(blocks);
			list.AddRange(tanks);
			var x = list.FirstOrDefault(s => (s as ITank)?.Tank_ID == (entity as ITank)?.Tank_ID);
			list.Remove(x);
            var t = list.FindAll(s => (s as IBlock)?.blockType == BlockType.Health);
            foreach (var item in t) list.Remove(item);
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
        public void HealthBlock()
        {
            var block = new GameObjectFactory().CreateBlock();
            block.blockType = BlockType.Health;
            block.Position = this.Reload();
            block.HelthPoints = 1;
            block.Can_Be_Destroyed = true;
            this.blocks.Add(block);
        }
		/// <summary>
		/// Реализация попадания пули в другую сущность
		/// </summary>
		/// <param name="bullet">Пуля попавшая в "цель"</param>
		private void HitTarget(IBullet bullet)
		{
			var tmp = tanks.FirstOrDefault(tank => tank.Position.IntersectsWith(bullet.Position));
			if (tmp != null)
			{
				if (tmp.Tank_ID != bullet.Parent_Id)
				{
					this.Death(bullet);
                    bullets.Remove(bullet);
					this.Death(tmp);
				}
			}
			var tmp2 = blocks.FirstOrDefault(bl => bl.Position.IntersectsWith(bullet.Position));
			if (tmp2 != null)
			{
				this.Death(bullet);
                bullets.Remove(bullet);
                this.Death(tmp2);
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
					if (tanks.FirstOrDefault(tank => tank.Position.IntersectsWith(new Rectangle(p, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize))) == true) == null)
					{
						if (blocks.FirstOrDefault(tank => tank.Position.IntersectsWith(new Rectangle(p, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize))) == true) == null)
							if (bullets.FirstOrDefault(tank => tank.Position.IntersectsWith(new Rectangle(p, new Size(room.GameSetings.ObjectsSize, room.GameSetings.ObjectsSize))) == true) == null)
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
		private void SendEndGame(IEntity winner)
		{
			IPackage pack = new Package();
            pack.Data = winner;
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
			//var adress = new Addresssee(room.Gamers.FirstOrDefault(g => g.Passport == tank.Tank_ID).RemoteEndPoint);
			IPackage pack = new Package() { Data = tank, MesseggeType = MesseggeType.TankDeath };
			Owner.Sender.SendMessage(pack, room.Gamers);
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
			obj.Lives = 3;
			obj.HelthPoints = 5;
			//obj.Is_Alive = true;
			obj.Can_Be_Destroyed = true;
			obj.Can_Shoot = true;
			obj.Direction = Direction.Up;
			obj.Position = this.Reload();
			tanks.Add(obj);
			//objects.Add(obj);
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
