using System;
using System.Collections.Generic;
using System.Linq;
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


}
