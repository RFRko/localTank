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
			//this._room = room;
		}

        // ИЗ ТВОЕЙ ИСХОДНОЙ РЕАЛИЗАЦИИ
        //private IRoom _room; - теперь не ненужен, room будет значение Owner базового абстрактного класса
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
            tmp.Is_Alive = false;
        }

		public void Fire(string id) //было IPackage Fire(int id)
		{
            //ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ

            var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;

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

        public void GenerateMap()
		{
            //ПОКА ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ

            var room = Owner as IRoom; // используй так

            foreach (var t in room.Gamers)
            {

            }
        }

		public void Move(string id) // было IPackage Move(int id), ПОСТАВИЛ КАК У ТЕБЯ
		{
            //ПОКА ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ
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
            }
            else if (tmp is IBullet)
            {
                var bullet = tmp as IBullet;
            }
        }

		public void Reload(String id) //ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ
        {
			throw new NotImplementedException();
		}

		public void Send()          //было IEnumerable<IPackage> Send()  ПОСТАВИЛ КАК У ТЕБЯ
		{
            // Миха, если это Send .. то он должен делать Owner.Sender.SendMessage()

            //ПОКА ВЕРНУЛ ТВОЮ РЕАЛИЗАЦИЮ ПОСЛЕ РЕШЕНИЯ КОНФЛИКТОВ
            var t = new object() as IMap;

            t.Blocks = blocks;

            t.Bullets = bullets;

            t.Tanks = tanks;

            return t;

        }
    }
}
