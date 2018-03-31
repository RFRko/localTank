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
		//private IRoom _room;
		private IList<IPackage> processList = new List<IPackage>();
		private List<IEntity> objects;

        public ServerGameEngine() : base() { }
        public ServerGameEngine(IRoom room):base(room)  //room будет значение Owner базового абстрактного класса
		{
			this.ProcessMessages += MessagesHandler;
            this.ProcessMessage = null;
			//this._room = room;
		}

		public IPackage CheckWin()
		{
			throw new NotImplementedException();
		}
		private void MessagesHandler(IEnumerable<IPackage> list)
		{
			processList = new List<IPackage>();
			foreach(var t in list)
			{
				
			}
		}

		public IPackage Death(int id)
		{
			throw new NotImplementedException();
		}

		public IPackage Fire(int id)
		{
			throw new NotImplementedException();
		}

		public IPackage GenerateMap()
		{
			objects = new List<IEntity>();
			int colIndMin = 0;
			int colIndMax = 20;
			int rowIndMin = 0;
			int rowIndMax = 20;
			int decorCount = 10;

            var Room = Owner as IRoom;

			int players = Room.Gamers.Count();
			while(players>0&&decorCount>0)
			{
				Random colInd = new Random(DateTime.Now.Millisecond - 15);
				Random rowInd = new Random(DateTime.Now.Millisecond + 20);
				int columnIndex = colInd.Next(colIndMin, colIndMax);
				int rowIndex = rowInd.Next(rowIndMin, rowIndMax);
				bool state = false;
				foreach(var z in objects)
				{
					if(z.Position==new Point(columnIndex,rowIndex))
					{
						state =true;
					}
				}
				if(!state)
				{
					// ???
				}
			}

			var x = objects as IPackage;
			return x;
		}

		public IPackage Move(int id)
		{
			throw new NotImplementedException();
		}

		public IPackage Reload(int id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPackage> Send()
		{
			throw new NotImplementedException();
		}
	}
}
