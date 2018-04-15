using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Tanki
{
	/// <summary>
	/// Реализация игрового движка
	/// </summary>
    public class ServerGameEngine : EngineAbs
    {
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
        }
		/// <summary>
		/// Конструктор игрового движка
		/// </summary>
		/// <param name="room">Значение Owner базового абстрактного класса</param>
		public ServerGameEngine(IRoom room):base(room) 
		{
			this.ProcessMessages += MessagesHandler;
            this.ProcessMessage = null;

            var gameRoom = room as IGameRoom;
            if (gameRoom == null) throw new Exception("Wrong room type");

            gameRoom.OnNewGameStatus += OnNewGameStatus_Handler;
			this.status = GameStatus.WaitForStart;

            this.Width = room.GameSetings.MapSize.Width;
            this.Height = room.GameSetings.MapSize.Height;

            //Это должно быть не тут:  
            // - подписка на OnNewAddresssee происходит в  EngineAbs
            // - GenerateMap - делаем в OnNetProcStarted_EventHandler, 
            //      подписка на OnNetProcStarted происходит в registerdependency рума
            //      OnNetProcStarted - вызывается  в RUN()  NetProcessorAbs
            //room.OnNewAddresssee += OnNewAddresssee_Handler;
            //this.GenerateMap();
        }
        /// <summary>
        /// Ширина игрового поля
        /// </summary>
        public int Width { get { return this.width;	} set {	this.width = value;	}}
		/// <summary>
		/// Высота игрового поля
		/// </summary>
		public int Height { get { return this.height; } set { this.height = value; } }
		/// <summary>
		/// Список всех сущностей на игровом поле
		/// </summary>
        private List<IEntity> objects=new List<IEntity>();
        private int width;
        private int height;
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
					foreach(var tank in tanks)
					{
						if (tank.Lives>0)
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
			Parallel.ForEach(package, item => {
				var entity = item.Data as IEntity;
				if (!entity.Is_Alive)
				{
					if (entity is ITank)
					{
						var tank = entity as ITank;
						if (tank.Lives > 0)
						{
							this.Reload(entity);
						}
						else
						{
							tanks.Remove(entity as ITank);
							objects.Remove(entity);
						}
					}
					if (entity is IBlock)
					{
						blocks.Remove(entity as IBlock);
						objects.Remove(entity);
					}
					if (entity is IBullet)
					{
						bullets.Remove(entity as IBullet);
						objects.Remove(entity);
					}
				}
			});


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
		/// <summary>
		/// Реализация делегата ProcessMessagesHandler
		/// </summary>
		/// <param name="list">Список пакетов переданый движку на обработку</param>
		private void MessagesHandler(IEnumerable<IPackage> list)
		{
			if (this.status == GameStatus.Start)
			{
				this.CheckAlive(list);
				Parallel.ForEach(bullets, x => this.Move(x));
				//foreach (var x in bullets) // могу и Эту чепуху сделать паралельной, она на работу не повлияет
				//{
				//	this.Move(x);
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
			}
        }
		/// <summary>
		/// Метод реализирующий обработку "убитой" сущности
		/// </summary>
		/// <param name="entity">"Убитая" сущность</param>
		private void Death(IEntity entity) 
        {
			var tmp = objects.FirstOrDefault(t => t==entity);
			if (tmp is ITank)
			{
				var tank = tmp as ITank;
				if (tank.Lives > 0)
				{
					tank.Lives--;
				}
				else
				{
					var room = Owner as IRoom;
					var adress = new Addresssee(room.Gamers.FirstOrDefault(g => g.Passport == tank.Tank_ID).RemoteEndPoint);
					Owner.Sender.SendMessage(new Package() {Data="Game Over!",MesseggeType=MesseggeType.Error }, adress);	
				}
			}
			else if(tmp is IBullet)
			{
				var bullet = tmp as IBullet;
				tanks.FirstOrDefault(t => t.Tank_ID == bullet.Parent_Id).Can_Shoot = true;
			}
			tmp.Is_Alive = false;
        }
		/// <summary>
		/// Метод реализирующий выстрел
		/// </summary>
		/// <param name="entity">Сущность осуществившая выстрел</param>
		private void Fire(IEntity entity)
		{
			var tmp = objects.FirstOrDefault(t => t == entity) as ITank;
			if (tmp.Can_Shoot)
			{
				var bullet = new GameObjectFactory().CreateBullet();
				//var bullet = new Bullet();
				var room = Owner as IRoom;
				bullet.Size = room.GameSetings.ObjectsSize; //???
				bullet.Direction = tmp.Direction;				
				bullet.Parent_Id = tmp.Tank_ID;
				tanks.FirstOrDefault(t=>t==tmp).Can_Shoot = false;
				bullet.Is_Alive = true;
				bullet.Can_Be_Destroyed = false;
				bullet.Position = tmp.Position;
				bullet.Command = EntityAction.Move;
				bullets.Add(bullet);
			}
			entity.Command = EntityAction.None;
		}
		/// <summary>
		/// Генерация сущностей на игровом поле
		/// </summary>
        private void GenerateMap()
		{
            var room = Owner as IRoom;
			int tankCount = room.Gamers.Count();
			int objectCount = (this.height * this.width) / (room.GameSetings.ObjectsSize*room.GameSetings.ObjectsSize*room.GameSetings.MaxPlayersCount);
            foreach (var t in room.Gamers)
            {
				this.NewGamer(t);
			}
			while(objectCount>0)
			{
				var obj = new GameObjectFactory().CreateBlock();
				//var obj = new Block();
				obj.Size = room.GameSetings.ObjectsSize;
				this.Reload(obj);
				obj.Can_Be_Destroyed = true;
				obj.Is_Alive = true;
				blocks.Add(obj);
				objects.Add(obj);
				objectCount--;
			}
		}
		/// <summary>
		/// Метод реализирующий движение сущности
		/// </summary>
		/// <param name="entity">Сущность осуществляющая движение</param>
		private void Move(IEntity entity)
		{
			var tmp = objects.FirstOrDefault(t => t == entity);
			if (tmp.Is_Alive)
			{
				if (tmp is ITank)
				{
					var tank = tmp as ITank;
					if (this.canMove(tank))
					{
						switch (tank.Direction)
						{
							case Direction.Left:
								if (tank.Position.X > 0)
								{
									var pos = new Point(tank.Position.X - 1, tank.Position.Y);
									tank.Position = new Rectangle(pos, new Size(tank.Size, tank.Size));
								}
								break;

							case Direction.Right:
								if (tank.Position.X < width)
								{
									var pos = new Point(tank.Position.X + 1, tank.Position.Y);
									tank.Position = new Rectangle(pos, new Size(tank.Size, tank.Size));
								}
								break;

							case Direction.Up:
								if (tank.Position.Y > 0)
								{
									var pos = new Point(tank.Position.X, tank.Position.Y - 1);
									tank.Position = new Rectangle(pos, new Size(tank.Size, tank.Size));
								}
								break;

							case Direction.Down:

								if (tank.Position.Y < height)
								{
									var pos = new Point(tank.Position.X, tank.Position.Y + 1);
									tank.Position = new Rectangle(pos, new Size(tank.Size, tank.Size));
								}
								break;
						}
					}
					tank.Command = EntityAction.None;
				}
				else if (tmp is IBullet)
				{
					var bullet = tmp as IBullet;
					switch (bullet.Direction)
					{
						case Direction.Left:
							if (bullet.Position.X > 0)
							{
								var pos = new Point(bullet.Position.X - 1, bullet.Position.Y);
								bullet.Position = new Rectangle(pos, new Size(bullet.Size, bullet.Size));
							}
							break;

						case Direction.Right:
							if (bullet.Position.X < width)
							{
								var pos = new Point(bullet.Position.X + 1, bullet.Position.Y);
								bullet.Position = new Rectangle(pos, new Size(bullet.Size, bullet.Size));
							}
							break;

						case Direction.Up:
							if (bullet.Position.Y > 0)
							{
								var pos = new Point(bullet.Position.X, bullet.Position.Y - 1);
								bullet.Position = new Rectangle(pos, new Size(bullet.Size, bullet.Size));
							}
							break;

						case Direction.Down:

							if (bullet.Position.Y < height)
							{
								var pos = new Point(bullet.Position.X, bullet.Position.Y + 1);
								bullet.Position = new Rectangle(pos, new Size(bullet.Size, bullet.Size));
							}
							break;
					}
					this.HitTarget(bullet);
					this.bulletOnBoard(bullet);
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
			var tmp = objects.FirstOrDefault(obj => obj.Position.IntersectsWith(entity.Position));
			if (tmp != null)
				return false;
			return true;
		}
		/// <summary>
		/// Предикат, проверяющий наличие пули в игровом поле
		/// </summary>
		/// <param name="bullet"> Пуля</param>
		/// <returns></returns>
		private bool bulletOnBoard(IBullet bullet)
		{
			var Board = new Rectangle(0, 0, this.width, this.height);
			if(Board.Contains(bullet.Position))
			{
				return true;
			}
			this.Death(bullet);
			return false;
		}
		/// <summary>
		/// Реализация попадания пули в другую сущность
		/// </summary>
		/// <param name="bullet">Пуля попавшая в "цель"</param>
		private void HitTarget(IBullet bullet)
		{
			var tmp = objects.FirstOrDefault(tank => tank.Position.IntersectsWith(bullet.Position));
			if(tmp!=null)
			{
				this.Death(tmp);
				this.Death(bullet);
			}
			
		}
		/// <summary>
		/// Гененация расположения для сущности
		/// </summary>
		/// <param name="entity">Сущность требующая разположения на игровом поле</param>
		private void Reload(IEntity entity)
        {
			entity.Position = Rectangle.Empty;
			while (entity.Position != Rectangle.Empty)
			{
				Random colInd = new Random(DateTime.Now.Millisecond - 15);
				Random rowInd = new Random(DateTime.Now.Millisecond + 20);
				int columnIndex = colInd.Next(0, width);
				int rowIndex = rowInd.Next(0, height);
				Point p = new Point(rowIndex, columnIndex);
				if (objects.FirstOrDefault(tank => tank.Position.IntersectsWith(new Rectangle(p, new Size(entity.Size, entity.Size))) == true) == null)
				{
					entity.Position = new Rectangle(p,new Size(entity.Size,entity.Size));
				}
			}
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
			obj.Lives = 5;
			obj.Is_Alive = true;
			obj.Can_Shoot = true;
			obj.Direction = Direction.Up;
			this.Reload(obj);
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
			var room = Owner as IRoom;
			var gamer = room.Gamers.FirstOrDefault(t => t.RemoteEndPoint == evntData.newAddresssee.RemoteEndPoint);
			this.NewGamer(gamer);
        }
		/// <summary>
		/// Обработка события изменения игрового статуса
		/// </summary>
		/// <param name="Sender">Объект изменивший игровой статус</param>
		/// <param name="statusData"> Данные о новом игровом статуса</param>
		public void OnNewGameStatus_Handler(object Sender, GameStatusChangedData statusData)
		{
			this.status = statusData.newStatus;
			if (statusData.newStatus == GameStatus.Start)
				this.SendStartGame();
		}

        public override void OnNetProcStarted_EventHandler(object Sender, NetProcStartedEvntData evntData)
        {
            this.GenerateMap();
        }
    }
}
