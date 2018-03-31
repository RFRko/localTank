using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Tanki
{
	public class ServerEngine : IServerEngine
	{
		public ProcessMessageHandler ProcessMessage { get; }
		private IRoom _room;
		private IList<IPackage> processList = new List<IPackage>();
		private List<IEntity> objects;
		private int width = 20;
		private int height = 20;
		private List<ITank> tanks = new List<ITank>();
		private List<IBlock> blocks = new List<IBlock>();
		private List<IBullet> bullets = new List<IBullet>();
		private int objectCount = 10;
		private ISender sender;
		public ServerEngine(IRoom room)
		{
			this.ProcessMessage = MessageHandler;
			this._room = room;

		}

		private void CheckWin()
		{
			throw new NotImplementedException();
		}
		private void MessageHandler(IEnumerable<IPackage> list)
		{
			processList = new List<IPackage>();
			foreach(var t in list)
			{
				var tmp = t.Data as IEntity;
				if (tmp.Command==EntityAction.Move)
				{
					this.Move(t.Sender_id);
				}
				if(tmp.Command==EntityAction.Fire)
				{
					this.Fire(t.Sender_id);
				}
			}
		}

		private void Death(string id)
		{
			var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
			tmp.Is_Alive = false;
			
		}

		private void Fire(string id)
		{
			var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
			var bullet = new object() as IBullet;
			bullet.Direction = tmp.Direction;
			bullet.Parent_Id = id;
			//tanks.FirstOrDefault()
			bullet.Is_Alive = true;
			bullet.Can_Be_Destroyed = false;
			bullet.Can_Shoot = false;
			bullet.Position = tmp.Position;
			bullet.Command = EntityAction.Move;
			bullets.Add(bullet);
		}

		private void GenerateMap()
		{
			foreach(var t in _room.gamerList)
			{

			}
		}

		private void Move(string id)
		{
			var tmp = processList.FirstOrDefault(t => t.Sender_id == id).Data as IEntity;
			if(tmp is ITank)
			{
				var tank = tmp as ITank;
				switch (tank.Direction)
				{
					case Direction.Left:
						if(tank.Position.X>0)
						{
							var pos = new Point(tank.Position.X - 1,tank.Position.Y);
							tank.Position = pos;
						}
						break;
					case Direction.Right:
						if(tank.Position.X<width)
						{
							var pos = new Point(tank.Position.X + 1, tank.Position.Y);
							tank.Position = pos;
						}
						break;
					case Direction.Up:
						if(tank.Position.Y>0)
						{
							var pos = new Point(tank.Position.X, tank.Position.Y-1);
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
			else if(tmp is IBullet)
			{
				var bullet = tmp as IBullet;
			}
		}

		private void Reload(string id)
		{
			throw new NotImplementedException();
		}

		public IMap Send()
		{
			var t = new object() as IMap;
			t.Blocks = blocks;
			t.Bullets = bullets;
			t.Tanks = tanks;
			return t;
		}
		
	}
}
