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
		}

        private IList<IPackage> processList = new List<IPackage>();
        private List<IEntity> objects;
        private int width = 20;
        private int height = 20;
        private List<ITank> tanks = new List<ITank>();
        private List<IBlock> blocks = new List<IBlock>();
        private List<IBullet> bullets = new List<IBullet>();
        private int objectCount = 10;
        private ISender sender;



        public void CheckWin()  //поменял IPackage на void
		{
			throw new NotImplementedException();
		}
		private void MessagesHandler(IEnumerable<IPackage> list)
		{
			processList = new List<IPackage>();
			foreach(var t in list)
			{
                //ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ
                var tmp = t.Data as IEntity;

                if (tmp.Command == EntityAction.Move)

                {

                    this.Move(t.Sender_id);

                }

                if (tmp.Command == EntityAction.Fire)

                {

                    this.Fire(t.Sender_id);

                }

            }
        }

		public void Death(string id) 
        {
            var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
			if (tmp is ITank)
			{
				var tank = tmp as ITank;
				if (tank.Lives > 0)
				{
					tank.Lives--;
					this.Reload(id);
				}	
			}
            tmp.Is_Alive = false;
        }

		public void Fire(string id)
		{ 
			var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
			if (tmp.Can_Shoot)
			{
				var bullet = new object() as IBullet;

				bullet.Direction = tmp.Direction;

				bullet.Parent_Id = id;

				tanks.FirstOrDefault(t => t.Tank_ID == id).Can_Shoot = false;

				bullet.Is_Alive = true;

				bullet.Can_Be_Destroyed = false;

				bullet.Can_Shoot = false;

				bullet.Position = tmp.Position;

				bullet.Command = EntityAction.Move;

				bullets.Add(bullet);
			}

        }

        public void GenerateMap()
		{
            //ПОКА ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ

            var room = Owner as IRoom; // используй так
			int tankCount = room.Gamers.Count();
            foreach (var t in room.Gamers)
            {
				var obj = new object() as ITank;
				obj.Tank_ID = t.id;
				obj.Lives = 5;
				obj.Is_Alive = true;
				obj.Can_Shoot = true;
				obj.Direction = Direction.Up;
				tanks.Add(obj);
				while(obj.Position!=null)
				{
					Random colInd = new Random(DateTime.Now.Millisecond - 15);
					Random rowInd = new Random(DateTime.Now.Millisecond + 20);
					int columnIndex = colInd.Next(0, width);
					int rowIndex = rowInd.Next(0, height);
					Point p = new Point(rowIndex, columnIndex);
					if(tanks.FirstOrDefault(tank=>tank.Position==p)==null)
					{
						obj.Position = p;
					}	
				}
				objects.Add(obj);
			}
			while(objectCount>0)
			{
				Random colInd = new Random(DateTime.Now.Millisecond - 15);
				Random rowInd = new Random(DateTime.Now.Millisecond + 20);
				int columnIndex = colInd.Next(0, width);
				int rowIndex = rowInd.Next(0, height);
				Point p = new Point(rowIndex, columnIndex);
				if(objects.FirstOrDefault(block=>block.Position==p)==null)
				{
					var obj = new object() as IBlock;
					obj.Position = p;
					obj.Can_Be_Destroyed = true;
					obj.Can_Shoot = false;
					obj.Is_Alive = true;
					blocks.Add(obj);
					objects.Add(obj);
				}
			}

		}

		public void Move(string id)
		{ 
            var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
            if (tmp is ITank)
            {
                var tank = tmp as ITank;
                switch (tank.Direction)
                {
                    case Direction.Left:
                        if (tank.Position.X > 0)
                        {
                            var pos = new Point(tank.Position.X - 1, tank.Position.Y);
                            tank.Position = pos;
                        }
                        break;

                    case Direction.Right:
                        if (tank.Position.X < width)
                        {
                            var pos = new Point(tank.Position.X + 1, tank.Position.Y);
                            tank.Position = pos;
                        }
                        break;

                    case Direction.Up:
                        if (tank.Position.Y > 0)
                        {
                            var pos = new Point(tank.Position.X, tank.Position.Y - 1);
                            tank.Position = pos;
                        }
                        break;

                    case Direction.Down:

                        if (tank.Position.Y < height)
                        {
                            var pos = new Point(tank.Position.X, tank.Position.Y + 1);
                            tank.Position = pos;
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
							bullet.Position = pos;
						}
						break;

					case Direction.Right:
						if (bullet.Position.X < width)
						{
							var pos = new Point(bullet.Position.X + 1, bullet.Position.Y);
							bullet.Position = pos;
						}
						break;

					case Direction.Up:
						if (bullet.Position.Y > 0)
						{
							var pos = new Point(bullet.Position.X, bullet.Position.Y - 1);
							bullet.Position = pos;
						}
						break;

					case Direction.Down:

						if (bullet.Position.Y < height)
						{
							var pos = new Point(bullet.Position.X, bullet.Position.Y + 1);
							bullet.Position = pos;
						}
						break;
				}
            }
        }

		public void Reload(String id) //ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ
        {
			
		}

		public void Send()
		{       
            var t = new object() as IMap;

            t.Blocks = blocks;

            t.Bullets = bullets;

            t.Tanks = tanks;
			var pack =new object() as IPackage;
			pack.Data = t;
			var adress = Owner as IRoom;
			Owner.Sender.SendMessage(pack, adress.Gamers);

		}
    }
}
