using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanki
{
	[Serializable]
	public class RoomStat : IRoomStat
	{
		public Guid Pasport { get; set; }
		public int Players_count { get; set; }
		public Guid Creator_Pasport { get; set; }
	}
}
