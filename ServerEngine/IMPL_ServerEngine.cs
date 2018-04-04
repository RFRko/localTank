using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Tanki
{
    public class ServerGameEngine : EngineAbs
    {
        public override ProcessMessageHandler ProcessMessage { get; protected set; }
        public override ProcessMessagesHandler ProcessMessages { get; protected set; }

        public ServerGameEngine() : base() { }
        public ServerGameEngine(IRoom room):base(room)  //room будет значение Owner базового абстрактного класса
		{
			this.ProcessMessages += MessagesHandler;
            this.ProcessMessage = null;
			objectCount = (width * height) / (width + height);
		}
		public int Width { get { return this.width;	} set {	this.width = value;	}}
		public int Height { get { return this.height; } set { this.height = value; } }
        private IList<IPackage> processList = new List<IPackage>();
        private List<IEntity> objects;
        private int width;
        private int height;
        private List<ITank> tanks = new List<ITank>();
        private List<IBlock> blocks = new List<IBlock>();
        private List<IBullet> bullets = new List<IBullet>();
		private int objectCount;
        private ISender sender;



        private bool CheckWin()  //поменял IPackage на void
		{
			return true;
		}
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
		private void MessagesHandler(IEnumerable<IPackage> list)
		{
			processList = new List<IPackage>();
			this.CheckAlive(list);
			foreach(var x in bullets) // могу и Эту чепуху сделать паралельной, она на работу не повлияет
			{
				this.Move(x);
			}
			foreach(var t in list)
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
        }
		private void Death(IEntity entity) 
        {
			var tmp = processList.FirstOrDefault(t => (IEntity)t.Data==entity) as IEntity;
			if (tmp is ITank)
			{
				var tank = tmp as ITank;
				if (tank.Lives > 0)
				{
					tank.Lives--;
					tank.Is_Alive = false;
				}	
			}
            tmp.Is_Alive = false;
        }

		private void Fire(IEntity entity)
		{
			var tmp = processList.FirstOrDefault(t => (IEntity)t.Data == entity) as ITank;
			if (tmp.Can_Shoot)
			{
				var bullet = new object() as IBullet;
				bullet.Direction = tmp.Direction;				
				bullet.Parent_Id = tmp.Tank_ID;
				tanks.FirstOrDefault(t=>t==tmp).Can_Shoot = false;
				bullet.Is_Alive = true;
				bullet.Can_Be_Destroyed = false;
				bullet.Can_Shoot = false;
				bullet.Position = tmp.Position;
				bullet.Command = EntityAction.Move;
				bullets.Add(bullet);
			}
			entity.Command = EntityAction.None;
		}

        private void GenerateMap()
		{
            var room = Owner as IRoom;
			int tankCount = room.Gamers.Count();
            foreach (var t in room.Gamers)
            {
				var obj = new object() as ITank;
				obj.Tank_ID = t.Passport;
				obj.Lives = 5;
				obj.Is_Alive = true;
				obj.Can_Shoot = true;
				obj.Direction = Direction.Up;
				this.Reload(obj);
				tanks.Add(obj);
				objects.Add(obj);
			}
			while(objectCount>0)
			{
				Random colInd = new Random(DateTime.Now.Millisecond - 15);
				Random rowInd = new Random(DateTime.Now.Millisecond + 20);
				int columnIndex = colInd.Next(0, width);
				int rowIndex = rowInd.Next(0, height);
				Point p = new Point(rowIndex, columnIndex);
				var obj = new object() as IBlock;
				if (objects.FirstOrDefault(tmp=>tmp.Position.IntersectsWith(new Rectangle(p,new Size(obj.Size,obj.Size)))==true)==null)
				{
					obj.Position = new Rectangle(p,new Size(obj.Size,obj.Size));
					obj.Can_Be_Destroyed = true;
					obj.Can_Shoot = false;
					obj.Is_Alive = true;
					blocks.Add(obj);
					objects.Add(obj);
				}
			}

		}

		private void Move(IEntity entity)
		{
			var tmp = processList.FirstOrDefault(t => (IEntity)t.Data == entity) as IEntity;
			if (tmp.Is_Alive)
			{
				if (tmp is ITank)
				{
					var tank = tmp as ITank;
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
				}
			}
        }
		private void HitTarget(IBullet bullet)
		{
			var tmp = objects.FirstOrDefault(tank => tank.Position.IntersectsWith(bullet.Position));
			if(tmp!=null)
			{
				tmp.Is_Alive = false;
				bullet.Is_Alive = false;
				tanks.FirstOrDefault(t => t.Tank_ID == bullet.Parent_Id).Can_Shoot = true;
			}
		}
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
		public void Send()
		{       
            var t = new object() as IMap;
            t.Blocks = blocks;
            t.Bullets = bullets;
            t.Tanks = tanks;
			var pack = new object() as IPackage;
			pack.Data = t;
			var adress = Owner as IRoom;
			Owner.Sender.SendMessage(pack, adress.Gamers);
        }



		// Нужно вызывать эту чепуху при новом игроке в комнате, метод ниже мне не подходит, по причине - мне не нужен ендпоинт, мне нужен гуид
		public void NewGamer(IGamer gamer)
		{
			var obj = new object() as ITank;
			obj.Tank_ID = gamer.Passport;  
			obj.Lives = 5;
			obj.Is_Alive = true;
			obj.Can_Shoot = true;
			obj.Direction = Direction.Up;
			this.Reload(obj);
			tanks.Add(obj);
			objects.Add(obj);
		}

		//или сендер здесь и есть IGamer?
        public override void OnNewAddresssee_Handler(object Sender, NewAddressseeData evntData)
        {
            throw new NotImplementedException();
        }
    }
}
