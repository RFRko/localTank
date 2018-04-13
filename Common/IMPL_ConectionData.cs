using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class ConectionData : IConectionData
	{
		public string PlayerName { get; set; }
		public Guid RoomPasport { get; set; }
		public GameSetings GameSetings { get; set; }
	}

    [Serializable]
    public class RoomsListData: IRoomsStat
    {
        public RoomsListData(IEnumerable<IRoomStat> src)
        {
            foreach(var rs in src)
            {
                _rooms_stat.Add(rs);
            }

        }
        private List<IRoomStat> _rooms_stat = new List<IRoomStat>();

        public IEnumerable<IRoomStat> RoomsStat { get { return _rooms_stat; } }
    }


    [Serializable]
    public class initialConectionData : IinitialConectionData
    {
        public Guid passport { get; set; }
        public IAddresssee manageRoomEndpoint { get; set; }
    }

	  [Serializable]
	  public class RoomInfo : IRoomInfo
	  {
		  public IPEndPoint roomEndpoint { get; set; }
		  public Size mapSize { get; set; }
	  }
}
